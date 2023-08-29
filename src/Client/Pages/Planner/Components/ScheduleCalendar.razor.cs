﻿using Drogecode.Knrm.Oefenrooster.Client.Enums;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Models.CalendarItems;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Heron.MudCalendar;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class ScheduleCalendar : IDisposable
{
    [Inject] private IStringLocalizer<ScheduleCalendar> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private CalendarItemRepository _calendarItemRepository { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeUser>? Users { get; set; }
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; }
    [Parameter, EditorRequired] public List<DrogeVehicle>? Vehicles { get; set; }
    [Parameter, EditorRequired] public List<PlannerTrainingType>? TrainingTypes { get; set; }
    private CancellationTokenSource _cls = new();
    private List<CalendarItem> _events = new();
    private List<UserTrainingCounter>? _userTrainingCounter;
    private List<RoosterItemMonth>? _monthItems;
    private bool _updating;
    private ScheduleView _view = ScheduleView.Calendar;
    private DateTime? _month;

    private async Task SetMonth(DateTime? dateTime)
    {
        if (dateTime == null) return;
        _month = dateTime;
        DateRange dateRange = new DateRange
        {
            Start = new DateTime(dateTime.Value.Year, dateTime.Value.Month, 1),
            End = new DateTime(dateTime.Value.Year, dateTime.Value.Month, DateTime.DaysInMonth(dateTime.Value.Year, dateTime.Value.Month))
        };
        await SetCalenderForMonth(dateRange);
    }

    private async Task SetCalenderForMonth(DateRange dateRange)
    {
        if (_updating || dateRange.Start == null) return;
        _updating = true;
        _events = new();
        _userTrainingCounter = null;
        TrainingWeek scheduleForUser = new();
        var scheduleForAll = await _scheduleRepository.ScheduleForAll(dateRange, false, _cls.Token);
        if (scheduleForAll == null) return;
        _userTrainingCounter = scheduleForAll.UserTrainingCounters;
        var trainingsInRange = scheduleForAll.Planners;
        if (trainingsInRange != null && trainingsInRange.Count > 0)
        {
            scheduleForUser.From = DateOnly.FromDateTime(trainingsInRange[0].DateStart);
            foreach (var training in trainingsInRange)
            {
                _events.Add(new ScheduleCalendarItem
                {
                    Start = training.DateStart,
                    End = training.DateEnd,
                    Training = training,
                });
            }
        }
        _month = PlannerHelper.ForMonth(dateRange);
        if (_month != null)
        {
            var monthItems = await _calendarItemRepository.GetMonthItemAsync(_month.Value.Year, _month.Value.Month, _cls.Token);
            _monthItems = monthItems?.MonthItems;
            var dayItems = await _calendarItemRepository.GetDayItemsAsync(dateRange, _cls.Token);
            if (dayItems?.DayItems != null)
            {
                foreach (var dayItem in dayItems.DayItems)
                {
                    _events.Add(new RoosterItemDayCalendarItem
                    {
                        Start = dayItem.DateStart,
                        End = dayItem.DateEnd,
                        AllDay = dayItem.IsFullDay,
                        ItemDay = dayItem,
                    });
                }
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
