﻿using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Models.CalendarItems;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Heron.MudCalendar;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;
public sealed partial class Calendar : IDisposable
{
    [Inject] private IStringLocalizer<Calendar> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository scheduleRepository { get; set; } = default!;
    [Inject] private DayItemRepository DayItemRepository { get; set; } = default!;
    [Inject] private MonthItemRepository MonthItemRepository { get; set; } = default!;
    [Inject] private UserRepository UserRepository { get; set; } = default!;
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter] public Guid CustomerId { get; set; } = Guid.Empty;
    private List<CalendarItem> _events = new();
    private List<RoosterItemMonth>? _monthItems;
    private CancellationTokenSource _cls = new();
    private DrogeUser? _user;
    private bool _updating;
    private DateTime? _month;

    protected override async Task OnInitializedAsync()
    {
        Global.NewTrainingAddedAsync += HandleNewTraining;
        _user = await UserRepository.GetCurrentUserAsync();
    }
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
        TrainingWeek scheduleForUser = new();
        var trainingsInWeek = (await scheduleRepository.CalendarForUser(dateRange, _cls.Token))?.Trainings;
        if (trainingsInWeek != null && trainingsInWeek.Count > 0)
        {
            foreach (var training in trainingsInWeek)
            {
                _events.Add(new DrogeCodeCalendarItem
                {
                    Start = training.DateStart,
                    End = training.DateEnd,
                    Training = training,
                    Text = training.Availabilty.ToString() ?? ""
                });
            }
        }
        _month = PlannerHelper.ForMonth(dateRange);
        if (_month != null)
        {
            var monthItems = await MonthItemRepository.GetMonthItemAsync(_month.Value.Year, _month.Value.Month, _cls.Token);
            _monthItems = monthItems?.MonthItems;
            var dayItems = await DayItemRepository.GetDayItemsAsync(dateRange, _user.Id, _cls.Token);
            if (dayItems?.DayItems != null)
            {
                foreach (var dayItem in dayItems.DayItems.Where(x => x.DateStart is not null))
                {
                    _events.Add(new RoosterItemDayCalendarItem
                    {
                        Start = dayItem.DateStart!.Value,
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

    private async Task HandleNewTraining(EditTraining newTraining)
    {
        if (newTraining.Date == null) return;
        var asTraining = new Training
        {
            TrainingId = newTraining.Id,
            Name = newTraining.Name,
            DateStart = DateTime.SpecifyKind((newTraining.Date ?? throw new ArgumentNullException("Date is null")) + (newTraining.TimeStart ?? throw new ArgumentNullException("StartTime is null")), DateTimeKind.Local),
            DateEnd = DateTime.SpecifyKind((newTraining.Date ?? throw new ArgumentNullException("Date is null")) + (newTraining.TimeEnd ?? throw new ArgumentNullException("StartTime is null")), DateTimeKind.Local),
            RoosterTrainingTypeId = newTraining.RoosterTrainingTypeId,
            ShowTime = newTraining.ShowTime,
            IsPinned = newTraining.IsPinned,
        };
        _events.Add(new DrogeCodeCalendarItem
        {
            Start = asTraining.DateStart,
            End = asTraining.DateEnd,
            Training = asTraining,
            Text = asTraining.Availabilty.ToString() ?? "",
        });
        StateHasChanged();
    }

    public void Dispose()
    {
        Global.NewTrainingAddedAsync -= HandleNewTraining;
        _cls.Cancel();
    }
}
