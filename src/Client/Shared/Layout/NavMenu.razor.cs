using Blazored.LocalStorage;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;

public sealed partial class NavMenu : IDisposable
{
    [Inject] private IStringLocalizer<NavMenu> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IDialogService DialogProvider { get; set; } = default!;
    [Inject] private VehicleRepository VehicleRepository { get; set; } = default!;
    [Inject] private ConfigurationRepository ConfigurationRepository { get; set; } = default!;
    [Inject] private UserRepository UserRepository { get; set; } = default!;
    [Inject] private ScheduleRepository ScheduleRepository { get; set; } = default!;
    [Inject] private TrainingTypesRepository TrainingTypesRepository { get; set; } = default!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;

    private CancellationTokenSource _cls = new();
    private UserMenuSettings? _userMenuSettings;
    private string _uriCalendar = "/planner/calendar";
    private string _uriSchedule = "/planner/schedule";
    private string _uriPlannerUser = "/planner/user";
    private string _sharePointUrl = string.Empty;
    private string _lplhUrl = string.Empty;
    private bool _useFullLinkExpanded;
    private bool _configurationExpanded;

    protected override async Task OnInitializedAsync()
    {
        Navigation.LocationChanged += RefreshForSubMenu;
        await UserRepository.GetCurrentUserAsync(); //Force creation of user.
    }

    protected override async Task OnParametersSetAsync()
    {
        if (AuthenticationState is not null)
        {
            var authState = await AuthenticationState;
            var loginHint = authState.User.FindFirst(c => c.Type == "login_hint")?.Value;
            if (string.IsNullOrEmpty(loginHint))
            {
                _sharePointUrl = "https://dorus1824.sharepoint.com";
                _lplhUrl = "https://dorus1824.sharepoint.com/:b:/r/sites/KNRM/Documenten/EHBO/LPLH/20181115%20LPLH_KNRM_1_1.pdf?csf=1&web=1&e=4L3VPo";
            }
            else
            {
                _sharePointUrl = $"https://dorus1824.sharepoint.com?login_hint={loginHint}";
                _lplhUrl = $"https://dorus1824.sharepoint.com/:b:/r/sites/KNRM/Documenten/EHBO/LPLH/20181115%20LPLH_KNRM_1_1.pdf?csf=1&web=1&e=4L3VPo&login_hint={loginHint}";
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _userMenuSettings = (await LocalStorage.GetItemAsync<UserMenuSettings>("userMenuSettings")) ?? new UserMenuSettings();
            // Can not use the object directly because it freezes the page.
            _useFullLinkExpanded = _userMenuSettings.UseFullLinkExpanded;
            _configurationExpanded = _userMenuSettings.ConfigurationExpanded;
            StateHasChanged();
        }
    }

    private void RefreshForSubMenu(object? sender, LocationChangedEventArgs e)
    {
        StateHasChanged();
    }

    private async Task AddTraining()
    {
        var trainingTypes = await TrainingTypesRepository.GetTrainingTypes(false, false, _cls.Token);
        var vehicles = await VehicleRepository.GetAllVehiclesAsync(false, _cls.Token);
        var parameters = new DialogParameters<EditTrainingDialog>
        {
            { x => x.Planner, null },
            { x => x.Refresh, null },
            { x => x.Vehicles, vehicles },
            { x => x.Global, Global },
            { x => x.TrainingTypes, trainingTypes }
        };
        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true,
            DisableBackdropClick = true
        };
        DialogProvider.Show<EditTrainingDialog>(L["Add training"], parameters, options);
    }

    private async Task UseFullLinksExpandedChanged(bool newValue)
    {
        if (_userMenuSettings is null || _userMenuSettings.UseFullLinkExpanded == newValue) return;
        _userMenuSettings.UseFullLinkExpanded = newValue;
        _useFullLinkExpanded = newValue;
        await LocalStorage.SetItemAsync("userMenuSettings", _userMenuSettings);
    }

    private async Task ConfigurationExpandedChanged(bool newValue)
    {
        if (_userMenuSettings is null || _userMenuSettings.ConfigurationExpanded == newValue) return;
        _userMenuSettings.ConfigurationExpanded = newValue;
        _configurationExpanded = newValue;
        await LocalStorage.SetItemAsync("userMenuSettings", _userMenuSettings);
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}