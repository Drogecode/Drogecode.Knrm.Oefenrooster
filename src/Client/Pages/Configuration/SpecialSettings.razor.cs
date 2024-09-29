using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class SpecialSettings : IDisposable
{
    [Inject] private IStringLocalizer<SpecialSettings> L { get; set; } = default!;
    [Inject] private IDialogService DialogProvider { get; set; } = default!;
    [Inject] private ConfigurationRepository ConfigurationRepository { get; set; } = default!;
    [Inject] private UserRepository UserRepository { get; set; } = default!;
    [Inject] private FunctionRepository FunctionRepository { get; set; } = default!;
    [Inject] private ICustomerSettingsClient CustomerSettingsClient { get; set; } = default!;
    [Inject] private ISharePointClient SharePointClient { get; set; } = default!;
    [Inject] private IReportActionClient ReportActionClient { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private CustomStateProvider AuthenticationStateProvider { get; set; } = default!;
    [CascadingParameter] private DrogeCodeGlobal Global { get; set; } = default!;
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private readonly RefreshModel _refreshModel = new();
    private readonly CancellationTokenSource _cls = new();

    private bool? _clickedUpdate;
    private bool? _usersSynced;
    private bool? _specialDatesUpdated;
    private bool? _settingTrainingToCalendar;
    private bool _clickedSyncHistorical;
    private Guid _userId;
    private GetHistoricalResponse? _syncHistorical;
    private DbCorrectionResponse? _dbCorrection;
    private KillDbResponse? _killDb;
    private HubConnection? _hubConnection;

    protected override void OnInitialized()
    {
        _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
        Global.VisibilityChangeAsync += VisibilityChanged;
    }

    protected override async Task OnParametersSetAsync()
    {
        _settingTrainingToCalendar = await CustomerSettingsClient.GetTrainingToCalendarAsync();
        _users = await UserRepository.GetAllUsersAsync(true, true, false, _cls.Token);
        _functions = await FunctionRepository.GetAllFunctionsAsync(false, _cls.Token);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetUser();
            await ConfigureHub();
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
                .WithUrl(Navigation.ToAbsoluteUri("/hub/refresh"))
                .WithAutomaticReconnect()
                .Build();
            _hubConnection.On<ItemUpdated>($"Refresh_{_userId}", async (type) =>
            {
                try
                {
                    if (_cls.Token.IsCancellationRequested)
                        return;
                    var stateHasChanged = true;
                    DebugHelper.WriteLine($"Got {type} request");
                    switch (type)
                    {
                        case ItemUpdated.UsersOnlineChanged:
                            _users = await UserRepository.GetAllUsersAsync(true, true, false, _cls.Token);
                            break;
                        default:
                            stateHasChanged = false;
                            DebugHelper.WriteLine("Missing type, ignored");
                            break;
                    }

                    if (stateHasChanged)
                        StateHasChanged();
                }
                catch (HttpRequestException)
                {
                    DebugHelper.WriteLine("c HttpRequestException");
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception e)
                {
                    DebugHelper.WriteLine("On message received from hub");
                    DebugHelper.WriteLine(e);
                    throw;
                }
            });
            await _hubConnection.StartAsync(_cls.Token);
        }
        catch (HttpRequestException)
        {
            DebugHelper.WriteLine("d HttpRequestException");
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception e)
        {
            DebugHelper.WriteLine("Failed to setup hub");
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
        await CustomerSettingsClient.PatchTrainingToCalendarAsync(isChecked);
    }

    private async Task UpdateDatabase()
    {
        _clickedUpdate = await ConfigurationRepository.UpgradeDatabaseAsync();
        StateHasChanged();
    }

    private async Task SyncUsers()
    {
        _usersSynced = null;
        _usersSynced = await UserRepository.SyncAllUsersAsync();
        if (_usersSynced == true)
        {
            _users = await UserRepository.GetAllUsersAsync(true, true, false, _cls.Token);
            await RefreshMeAsync();
        }
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

    private void AddUser()
    {
        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        var parameters = new DialogParameters { { "Functions", _functions }, { "Refresh", _refreshModel } };
        DialogProvider.Show<AddUserDialog>(L["Add user"], parameters, options);
    }

    private void ChangeUser(DrogeUser user)
    {
        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        var parameters = new DialogParameters { { "User", user }, { "Functions", _functions }, { "Refresh", _refreshModel } };
        DialogProvider.Show<EditUserDialog>(L["Edit user"], parameters, options);
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
}