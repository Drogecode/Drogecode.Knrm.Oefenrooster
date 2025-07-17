using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Setting;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.AspNetCore.SignalR.Client;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class GlobalConfiguration : IDisposable
{
    [Inject] private IStringLocalizer<GlobalConfiguration> L { get; set; } = default!;
    [Inject] private IDialogService DialogProvider { get; set; } = default!;
    [Inject] private ConfigurationRepository ConfigurationRepository { get; set; } = default!;
    [Inject] private UserRepository UserRepository { get; set; } = default!;
    [Inject] private FunctionRepository FunctionRepository { get; set; } = default!;
    [Inject] private ICustomerSettingsClient CustomerSettingsClient { get; set; } = default!;
    [Inject] private ISharePointClient SharePointClient { get; set; } = default!;
    [Inject] private IReportActionClient ReportActionClient { get; set; } = default!;
    [Inject] private IPreComClient PreComClient { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private CustomStateProvider AuthenticationStateProvider { get; set; } = default!;
    [CascadingParameter] private DrogeCodeGlobal Global { get; set; } = default!;
    private DateTime _lastSyncDateTime = DateTime.UtcNow;
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private readonly RefreshModel _refreshModel = new();
    private readonly CancellationTokenSource _cls = new();

    private string? _settingCalenderPrefix;
    private string? _preComAvailableText;
    private int? _preComDaysInFuture;
    private bool? _clickedUpdate;
    private bool? _usersSynced;
    private bool? _specialDatesUpdated;
    private bool? _settingTrainingToCalendar;
    private bool? _delaySyncingTrainingToOutlook;
    private bool? _performanceEnabled;
    private bool _clickedSyncHistorical;
    private bool _clickedDeDuplicatePreCom;
    private bool _clickedSyncUsers;
    private bool _loaded;
    private Guid _userId;
    private GetHistoricalResponse? _syncHistorical;
    private DbCorrectionResponse? _dbCorrection;
    private KillDbResponse? _killDb;
    private DeleteResponse? _deDuplicatePreCom;
    private HubConnection? _hubConnection;

    protected override void OnInitialized()
    {
        _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
        Global.VisibilityChangeAsync += VisibilityChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _settingTrainingToCalendar = (await CustomerSettingsClient.GetBoolSettingAsync(SettingName.TrainingToCalendar, _cls.Token)).Value;
            _delaySyncingTrainingToOutlook = (await CustomerSettingsClient.GetBoolSettingAsync(SettingName.DelaySyncingTrainingToOutlook, _cls.Token)).Value;
            _settingCalenderPrefix = (await CustomerSettingsClient.GetStringSettingAsync(SettingName.CalendarPrefix, _cls.Token)).Value;
            _preComAvailableText = (await CustomerSettingsClient.GetStringSettingAsync(SettingName.PreComAvailableText, _cls.Token)).Value;
            _preComDaysInFuture = (await CustomerSettingsClient.GetIntSettingAsync(SettingName.PreComDaysInFuture, _cls.Token)).Value;
            _performanceEnabled = (await ConfigurationRepository.GetPerformanceSettingAsync( _cls.Token)).PerformanceEnabled;
            _users = await UserRepository.GetAllUsersAsync(true, true, false, _cls.Token);
            _functions = await FunctionRepository.GetAllFunctionsAsync(false, _cls.Token);
            await SetUser();
            await ConfigureHub();
            _loaded = true;
            StateHasChanged();
        }
    }

    private async Task<bool> SetUser()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userClaims = authState.User;
        if (userClaims?.Identity?.IsAuthenticated ?? false)
        {
            if (!Guid.TryParse(userClaims.Identities.FirstOrDefault()!.Claims.FirstOrDefault(x => x.Type.Equals("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value, out _userId))
                return false;
        }
        else
        {
            // Should never happen.
            return false;
        }

        return true;
    }

    private async Task ConfigureHub()
    {
        try
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/hub/configuration"))
                .WithAutomaticReconnect()
                .Build();
            _hubConnection.On<ConfigurationUpdatedHub>($"configuration", async (type) =>
            {
                if (_cls.Token.IsCancellationRequested)
                    return;
                var stateHasChanged = true;
                DebugHelper.WriteLine($"Got {type.ConfigurationUpdated} request");
                switch (type.ConfigurationUpdated)
                {
                    case ConfigurationUpdated.UsersOnlineChanged:
                        if (_lastSyncDateTime.CompareTo(DateTime.UtcNow.AddSeconds(-50)) < 1)
                        {
                            _lastSyncDateTime = DateTime.UtcNow;
                            _users = await UserRepository.GetAllUsersAsync(true, true, false, _cls.Token);
                        }

                        break;
                    default:
                        stateHasChanged = false;
                        break;
                }

                if (stateHasChanged)
                    StateHasChanged();
            });
            await _hubConnection.StartAsync(_cls.Token);
        }
        catch (HttpRequestException)
        {
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception e)
        {
            DebugHelper.WriteLine(e);
        }
    }

    private async Task VisibilityChanged()
    {
        _users = await UserRepository.GetAllUsersAsync(true, true, false, _cls.Token);
        _functions = await FunctionRepository.GetAllFunctionsAsync(false, _cls.Token);
    }

    private async Task PatchTrainingToCalendar(bool isChecked)
    {
        _settingTrainingToCalendar = isChecked;
        var body = new PatchSettingBoolRequest(SettingName.TrainingToCalendar, isChecked);
        await CustomerSettingsClient.PatchBoolSettingAsync(body, _cls.Token);
    }
    private async Task PatchDelaySyncingTrainingToOutlook(bool isChecked)
    {
        _delaySyncingTrainingToOutlook = isChecked;
        var body = new PatchSettingBoolRequest(SettingName.DelaySyncingTrainingToOutlook, isChecked);
        await CustomerSettingsClient.PatchBoolSettingAsync(body, _cls.Token);
    }

    private async Task PatchCalenderPrefix(string newValue)
    {
        _settingCalenderPrefix = newValue;
        var body = new PatchSettingStringRequest(SettingName.CalendarPrefix, newValue);
        await CustomerSettingsClient.PatchStringSettingAsync(body, _cls.Token);
    }

    private async Task PatchPreComAvailableText(string newValue)
    {
        _preComAvailableText = newValue;
        var body = new PatchSettingStringRequest(SettingName.PreComAvailableText, newValue);
        await CustomerSettingsClient.PatchStringSettingAsync(body, _cls.Token);
    }

    private async Task PatchPreComDaysInFuture(int? newValue)
    {
        if (newValue is null) return;
        _preComDaysInFuture = newValue;
        var body = new PatchSettingIntRequest(SettingName.PreComDaysInFuture, newValue.Value);
        await CustomerSettingsClient.PatchIntSettingAsync(body, _cls.Token);
    }

    private async Task UpdateDatabase()
    {
        _clickedUpdate = await ConfigurationRepository.UpgradeDatabaseAsync();
        StateHasChanged();
    }

    private async Task SyncUsers()
    {
        _clickedSyncUsers = true;
        _usersSynced = null;
        StateHasChanged();
        _usersSynced = await UserRepository.SyncAllUsersAsync();
        if (_usersSynced == true)
        {
            _users = await UserRepository.GetAllUsersAsync(true, true, false, _cls.Token);
        }

        _clickedSyncUsers = false;
        await RefreshMeAsync();
    }

    private async Task UpdateSpecialDates()
    {
        _specialDatesUpdated = await ConfigurationRepository.UpdateSpecialDates();
        StateHasChanged();
    }

    private async Task RunDbCorrection()
    {
        _dbCorrection = await ConfigurationRepository.DbCorrection(_cls.Token);
        StateHasChanged();
    }

    private async Task RunSyncHistorical()
    {
        _clickedSyncHistorical = true;
        StateHasChanged();
        _syncHistorical = await SharePointClient.SyncHistoricalAsync(_cls.Token);
        _clickedSyncHistorical = false;
        StateHasChanged();
    }

    private async Task KillDb()
    {
        _killDb = await ReportActionClient.KillDbAsync(_cls.Token);
        StateHasChanged();
    }

    private async Task DeDuplicatePreCom()
    {
        _clickedDeDuplicatePreCom = true;
        StateHasChanged();
        _deDuplicatePreCom = await PreComClient.DeleteDuplicatesAsync(_cls.Token);
        _clickedDeDuplicatePreCom = false;
        StateHasChanged();
    }

    private Task AddUser()
    {
        var parameters = new DialogParameters<AddUserDialog>
        {
            { x => x.Functions, _functions },
            { x => x.Refresh, _refreshModel },
        };
        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        return DialogProvider.ShowAsync<AddUserDialog>(L["Add user"], parameters, options);
    }

    private Task ChangeUser(DrogeUser user)
    {
        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        var parameters = new DialogParameters { { "User", user }, { "Functions", _functions }, { "Refresh", _refreshModel } };
        return DialogProvider.ShowAsync<EditUserDialog>(L["Edit user"], parameters, options);
    }

    private async Task RefreshMeAsync()
    {
        _users = await UserRepository.GetAllUsersAsync(true, true, false, _cls.Token);
        StateHasChanged();
    }

    public void Dispose()
    {
        _refreshModel.RefreshRequestedAsync -= RefreshMeAsync;
        Global.VisibilityChangeAsync -= VisibilityChanged;
        _cls.Cancel();
    }

    private async Task PatchPerformanceEnabled(bool isChecked)
    {
        await ConfigurationRepository.PatchPerformanceSettingAsync(new PatchPerformanceSettingRequest() { PerformanceEnabled = isChecked }, _cls.Token);
        _performanceEnabled = isChecked;
        StateHasChanged();
    }
}