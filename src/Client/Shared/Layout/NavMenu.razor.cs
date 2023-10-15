using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Client.Shared.Layout.Components;
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
    [Inject] private TrainingTypesRepository _trainingTypesRepository { get; set; } = default!;
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;

    private CancellationTokenSource _cls = new();
    private string _uriCalendar = "/planner/calendar";
    private string _uriSchedule = "/planner/schedule";
    private string _uriPlannerUser = "/planner/user";

    protected override async Task OnInitializedAsync()
    {
        Navigation.LocationChanged += RefreshForSubMenu;
        var installing = await _configurationRepository.InstallingActive();
        var dbUser = await _userRepository.GetCurrentUserAsync();//Force creation of user.
    }

    private void RefreshForSubMenu(object sender, LocationChangedEventArgs e)
    {
        StateHasChanged();
    }
    private async Task AddTraining()
    {
        var trainingTypes = await _trainingTypesRepository.GetTrainingTypes(_cls.Token);
        var vehicles = await _vehicleRepository.GetAllVehiclesAsync();
        var parameters = new DialogParameters<EditTrainingDialog>
        {
            { x=>x.Planner, null },
            { x=>x.Refresh, null },
            { x=>x.Vehicles, vehicles },
            { x=>x.Global, Global },
            { x=>x.TrainingTypes, trainingTypes }
        };
        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            DisableBackdropClick = true
        };
        _dialogProvider.Show<EditTrainingDialog>(L["Add training"], parameters, options);
    }

    public async Task AddDayItem()
    {
        var parameters = new DialogParameters<AddDayItemDialog>
        {
        };
        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            DisableBackdropClick = true
        };
        _dialogProvider.Show<AddDayItemDialog>(L["Add other item"], parameters, options);
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
