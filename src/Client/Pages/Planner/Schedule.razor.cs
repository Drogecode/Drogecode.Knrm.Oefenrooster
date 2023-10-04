﻿using Drogecode.Knrm.Oefenrooster.Client.Enums;
using Drogecode.Knrm.Oefenrooster.Client.Helpers;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Heron.MudCalendar;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;

public sealed partial class Schedule : IDisposable
{
    [Inject] private IStringLocalizer<Schedule> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private TrainingTypesRepository _trainingTypesRepository { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private FunctionRepository _functionRepository { get; set; } = default!;
    [Inject] private VehicleRepository _vehicleRepository { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Parameter] public Guid CustomerId { get; set; } = Guid.Empty;
    [Parameter] public string? View { get; set; }
    private CancellationTokenSource _cls = new();
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private List<DrogeVehicle>? _vehicles;
    private List<PlannerTrainingType>? _trainingTypes;
    private bool _updating;
    private ScheduleView _view = ScheduleView.Calendar;

    private Action<SnackbarOptions> _snackbarConfig = (SnackbarOptions options) =>
    {
        options.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
        options.RequireInteraction = false;
        options.ShowCloseIcon = true;
        options.VisibleStateDuration = 20000;
    };

    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrEmpty(View) && View.Equals("Table"))
            _view = ScheduleView.Table;
        _users = await _userRepository.GetAllUsersAsync(false);
        _functions = await _functionRepository.GetAllFunctionsAsync();
        _vehicles = await _vehicleRepository.GetAllVehiclesAsync();
        _trainingTypes = await _trainingTypesRepository.GetTrainingTypes(_cls.Token);
    }

    public void ChangeView(ScheduleView newView)
    {
        if (_view == newView) return;
        _view = newView;
        Navigation.NavigateTo($"/planner/schedule/{newView}");
    }

    public void ShowSnackbarAssignmentChanged(PlanUser user, PlannedTraining training)
    {
        var key = $"table_{user.UserId}_{training.TrainingId}";
        Snackbar.RemoveByKey(key);
        Snackbar.Add(L["{0} {1} {2} {3} {4}", user.Assigned ? L["Assigned"] : L["Removed"], user.Name, user.Assigned ? L["to"] : L["from"], training.DateStart.ToShortDateString(), training.Name ?? ""], (user.Availabilty == Availabilty.NotAvailable || user.Availabilty == Availabilty.Maybe) && user.Assigned ? Severity.Warning : user.Assigned ? Severity.Normal : Severity.Info, configure: _snackbarConfig, key: key);
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
