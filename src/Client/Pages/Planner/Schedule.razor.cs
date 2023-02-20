﻿using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;

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
    private int? _month;
    private int _high = -1;
    private int _low = -2;
    private bool _updating;

    protected override async Task OnInitializedAsync()
    {
        for (int i = -1; i < 6; i++)
        {
            await AddWeekToSchadules(true);
        }
        _users = await _userRepository.GetAllUsersAsync();
        _functions = await _functionRepository.GetAllFunctionsAsync();
        _vehicles = await _vehicleRepository.GetAllVehiclesAsync();
    }

    private async Task AddMultipeWeekToSchadules(bool high, int count)
    {
        for (int i = -1; i < count; i++)
        {
            await AddWeekToSchadules(high);
        }
    }

    private async Task AddWeekToSchadules(bool high)
    {
        if (_updating) return;
        _updating = true;
        List<PlannedTraining>? scheduleForUser = null;
        var PlannersInWeek = (await _scheduleRepository.ScheduleForAll(high ? _high : _low, _cls.Token))?.Planners;
        if (PlannersInWeek != null)
        {
            scheduleForUser = new List<PlannedTraining>();
            foreach (var Plan in PlannersInWeek)
                scheduleForUser.Add(Plan);
        }
        if (high)
        {
            if (scheduleForUser != null)
                _scheduleForUser.AddLast(scheduleForUser);
            _high++;
        }
        else
        {
            if (scheduleForUser != null)
                _scheduleForUser.AddFirst(scheduleForUser);
            _low--;
        }
        _updating = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
