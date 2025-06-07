using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Models.CalendarItems;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Heron.MudCalendar;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;
public sealed partial class Calendar : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<Calendar>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private ISessionExpireService? SessionExpireService { get; set; }
    [Inject, NotNull] private ScheduleRepository? ScheduleRepository { get; set; }
    [Inject, NotNull] private DayItemRepository? DayItemRepository { get; set; }
    [Inject, NotNull] private MonthItemRepository? MonthItemRepository { get; set; }
    [Inject, NotNull] private UserRepository? UserRepository { get; set; }
    [CascadingParameter, NotNull] private DrogeCodeGlobal? Global { get; set; }
    [Parameter] public Guid CustomerId { get; set; } = Guid.Empty;
    private List<CalendarItem> _events = [];
    private List<RoosterItemMonth>? _monthItems;
    private readonly CancellationTokenSource _cls = new();
    private DrogeUser? _user;
    private bool _updating;
    private bool _initialized;
    private bool _currentMonth;
    private DateTime? _month;
    private DateTime _firstMonth = DateTime.Today;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _firstMonth = await SessionExpireService.GetSelectedMonth(_cls.Token);
            Global.NewTrainingAddedAsync += HandleNewTraining;
            _user = await UserRepository.GetCurrentUserAsync();
            await SetMonth(_firstMonth);
            _initialized = true;
            StateHasChanged();
        }
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
        _events = [];
        TrainingWeek scheduleForUser = new();
        var trainingsInWeek = (await ScheduleRepository.CalendarForUser(dateRange, _cls.Token))?.Trainings;
        if (trainingsInWeek != null && trainingsInWeek.Count > 0)
        {
            foreach (var training in trainingsInWeek)
            {
                _events.Add(new DrogeCodeCalendarItem
                {
                    Start = training.DateStart,
                    End = training.DateEnd,
                    Training = training,
                    Text = training.Availability.ToString() ?? ""
                });
            }
        }
        _month = PlannerHelper.ForMonth(dateRange);
        if (_month is not null)
        {
            _firstMonth = _month.Value;
            _currentMonth = DateTime.Today.Month == _month.Value.Month;
            var monthItems = await MonthItemRepository.GetMonthItemAsync(_month.Value.Year, _month.Value.Month, _cls.Token);
            _monthItems = monthItems?.MonthItems;
            _user ??= await UserRepository.GetCurrentUserAsync();
            var dayItems = await DayItemRepository.GetDayItemsAsync(dateRange, _user?.Id ?? Guid.Empty, _cls.Token);
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
        await SessionExpireService.SetSelectedMonth(_month, _cls.Token);
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
            DateStart = DateTime.SpecifyKind((newTraining.Date ?? throw new ArgumentNullException("Date is null")) + (newTraining.TimeStart ?? throw new ArgumentNullException("StartTime is null")), DateTimeKind.Local).ToUniversalTime(),
            DateEnd = DateTime.SpecifyKind((newTraining.Date ?? throw new ArgumentNullException("Date is null")) + (newTraining.TimeEnd ?? throw new ArgumentNullException("StartTime is null")), DateTimeKind.Local).ToUniversalTime(),
            RoosterTrainingTypeId = newTraining.RoosterTrainingTypeId,
            ShowTime = newTraining.ShowTime,
            IsPinned = newTraining.IsPinned,
        };
        _events.Add(new DrogeCodeCalendarItem
        {
            Start = asTraining.DateStart,
            End = asTraining.DateEnd,
            Training = asTraining,
            Text = asTraining.Availability.ToString() ?? "",
        });
        StateHasChanged();
    }

    private async Task RemainingDaysUnavailable()
    {
        foreach (var item in _events.Where(item => item is DrogeCodeCalendarItem))
        {
            var calendarItem = item as DrogeCodeCalendarItem;
            if (calendarItem?.Training is null || calendarItem.Training.SetBy != AvailabilitySetBy.None)
                continue;
            calendarItem.Training.Availability = Availability.NotAvailable;
            calendarItem.Training.SetBy = AvailabilitySetBy.AllUnavailableButton;
        }
    }

    public void Dispose()
    {
        Global.NewTrainingAddedAsync -= HandleNewTraining;
        _cls.Cancel();
    }
}
