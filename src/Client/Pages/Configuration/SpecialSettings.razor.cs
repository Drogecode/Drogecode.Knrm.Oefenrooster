using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
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
    [CascadingParameter] private DrogeCodeGlobal Global { get; set; } = default!;
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private readonly RefreshModel _refreshModel = new();
    private readonly CancellationTokenSource _cls = new();

    private bool? _clickedUpdate;
    private bool? _usersSynced;
    private bool? _specialDatesUpdated;
    private bool? _settingTrainingToCalendar;
    private DbCorrectionResponse? _dbCorrection;


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
        _dbCorrection = await ConfigurationRepository.DbCorrection();
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
