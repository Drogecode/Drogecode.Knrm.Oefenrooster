using Drogecode.Knrm.Oefenrooster.Client.Helpers;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Heron.MudCalendar;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Diagnostics;
using System.Drawing;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;
public sealed partial class Calendar : IDisposable
{
    [Inject] private IStringLocalizer<Calendar> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private CalendarItemRepository _calendarItemRepository { get; set; } = default!;
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter] public Guid CustomerId { get; set; } = Guid.Empty;
    private List<CalendarItem> _events = new();
    private List<RoosterItemMonth>? _monthItems;
    private CancellationTokenSource _cls = new();
    private bool _updating;
    private DateTime? _month;

    protected override async Task OnInitializedAsync()
    {
        Global.NewTrainingAddedAsync += HandleNewTraining;
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
        var trainingsInWeek = (await _scheduleRepository.CalendarForUser(dateRange, _cls.Token))?.Trainings;
        if (trainingsInWeek != null && trainingsInWeek.Count > 0)
        {
            scheduleForUser.From = DateOnly.FromDateTime(trainingsInWeek[0].DateStart);
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

    private async Task HandleNewTraining(EditTraining newTraining)
    {
        if (newTraining.Date == null) return;
        var asTraining = new Training
        {
            TrainingId = newTraining.Id,
            Name = newTraining.Name,
            DateStart = DateTime.SpecifyKind((newTraining.Date ?? throw new ArgumentNullException("Date is null")) + (newTraining.TimeStart ?? throw new ArgumentNullException("StartTime is null")), DateTimeKind.Utc),
            DateEnd = DateTime.SpecifyKind((newTraining.Date ?? throw new ArgumentNullException("Date is null")) + (newTraining.TimeEnd ?? throw new ArgumentNullException("StartTime is null")), DateTimeKind.Utc),
            RoosterTrainingTypeId = newTraining.RoosterTrainingTypeId
        };
        var date = DateOnly.FromDateTime(newTraining.Date ?? throw new UnreachableException("newTraining.Date is null after null check"));
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
    private class DrogeCodeCalendarItem : CalendarItem
    {
        public Training? Training { get; set; }
    }
    private class RoosterItemDayCalendarItem : CalendarItem
    {
        public RoosterItemDay? ItemDay { get; set; }
    }
}
