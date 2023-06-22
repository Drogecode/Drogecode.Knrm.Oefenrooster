using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;

public sealed partial class NavMenu : IDisposable
{
    [Inject] private IStringLocalizer<NavMenu> L { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    [Inject] private VehicleRepository _vehicleRepository { get; set; } = default!;
    [Inject] private ConfigurationRepository _configurationRepository { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;

    private CancellationTokenSource _cls = new();
    private string _uriCalendar = "/planner/calendar";
    private string _uriSchedule = "/planner/schedule";
    private string _uriPlannerUser = "/planner/user";
    private bool _showSettings;

    protected override async Task OnInitializedAsync()
    {
        Navigation.LocationChanged += RefreshForSubMenu;
        var installing = await _configurationRepository.InstallingActive();
        var dbUser = await _userRepository.GetCurrentUserAsync();//Force creation of user.
        if (installing == true || (dbUser != null && dbUser.Id.Equals(DefaultSettingsHelper.IdTaco)))
            _showSettings = true;
    }

    private void RefreshForSubMenu(object sender, LocationChangedEventArgs e)
    {
        StateHasChanged();
    }
    private async Task AddTraining()
    {
        var trainingTypes = await _scheduleRepository.GetTrainingTypes(_cls.Token);
        var vehicles = await _vehicleRepository.GetAllVehiclesAsync();
        var parameters = new DialogParameters {
            { "Planner", null },
            { "Refresh", null },
            { "Vehicles", vehicles },
            { "Global", Global },
            { "TrainingTypes", trainingTypes }
        };
        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            DisableBackdropClick = true
        };
        _dialogProvider.Show<EditTrainingDialog>(L["Add training"], parameters, options);
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
