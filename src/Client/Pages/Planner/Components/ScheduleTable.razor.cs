﻿using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Microsoft.Extensions.Localization;
using static MudBlazor.CategoryTypes;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class ScheduleTable : IDisposable
{
    [Inject] private IStringLocalizer<ScheduleTable> L { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeUser>? Users { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeVehicle>? Vehicles { get; set; } = default!;
    [Parameter, EditorRequired] public List<PlannerTrainingType>? TrainingTypes { get; set; } = default!;

    private CancellationTokenSource _cls = new();
    private bool _updating;
    private List<PlannedTraining> _events = new();
    private List<UserTrainingCounter>? _userTrainingCounter;
    private DateTime _month;

    private async Task<DateTime> SetMonth(DateTime dateTime)
    {
        _month = dateTime;
        DateRange dateRange = new DateRange
        {
            Start = new DateTime(dateTime.Year, dateTime.Month, 1),
            End = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month))
        };
        await SetCalenderForMonth(dateRange);
        return _month;
    }

    protected override async Task OnInitializedAsync()
    {
        await SetMonth(DateTime.Today);
    }

    private async Task SetCalenderForMonth(DateRange dateRange)
    {
        if (_updating || dateRange.Start == null) return;
        _updating = true;
        _events = new();
        TrainingWeek scheduleForUser = new();
        var scheduleForAll = await _scheduleRepository.ScheduleForAll(dateRange, _cls.Token);
        if (scheduleForAll == null) return;
        _userTrainingCounter = scheduleForAll.UserTrainingCounters;
        var trainingsInRange = scheduleForAll.Planners;
        if (trainingsInRange != null && trainingsInRange.Count > 0)
        {
            scheduleForUser.From = DateOnly.FromDateTime(trainingsInRange[0].DateStart);
            foreach (var training in trainingsInRange)
            {
                _events.Add(training);
            }
        }
        _updating = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}