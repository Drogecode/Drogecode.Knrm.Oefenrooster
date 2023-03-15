﻿using Drogecode.Knrm.Oefenrooster.Client.Helpers;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Heron.MudCalendar;
using Microsoft.Extensions.Localization;
using Microsoft.Graph.Models;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;

public sealed partial class Schedule : IDisposable
{
    [Inject] private IStringLocalizer<Schedule> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private FunctionRepository _functionRepository { get; set; } = default!;
    [Inject] private VehicleRepository _vehicleRepository { get; set; } = default!;
    [Parameter] public Guid CustomerId { get; set; } = Guid.Empty;
    private LinkedList<List<PlannedTraining>> _scheduleForUser = new();
    private CancellationTokenSource _cls = new();
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private List<DrogeVehicle>? _vehicles;
    private List<CustomItem> _events = new();
    private bool _updating;

    protected override async Task OnInitializedAsync()
    {
        _users = await _userRepository.GetAllUsersAsync();
        _functions = await _functionRepository.GetAllFunctionsAsync();
        _vehicles = await _vehicleRepository.GetAllVehiclesAsync();
    }

    private async Task SetCalenderForMonth(DateRange dateRange)
    {
        if (_updating || dateRange.Start == null) return;
        _updating = true;
        _events = new();
        TrainingWeek scheduleForUser = new();
        var trainingsInWeek = (await _scheduleRepository.ScheduleForAll(dateRange, _cls.Token))?.Planners;
        if (trainingsInWeek != null && trainingsInWeek.Count > 0)
        {
            scheduleForUser.From = DateOnly.FromDateTime(trainingsInWeek[0].DateStart);
            foreach (var training in trainingsInWeek)
            {
                _events.Add(new CustomItem
                {
                    Start = training.DateStart,
                    End = training.DateEnd,
                    Training = training,
                    Color = PlannerHelper.HeaderClass(training.TrainingType)
                });
            }
        }
        _updating = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
    private class CustomItem : CalendarItem
    {
        public PlannedTraining? Training { get; set; }
        public string Color { get; set; } = "var(--mud-palette-grey-default)";
    }
}
