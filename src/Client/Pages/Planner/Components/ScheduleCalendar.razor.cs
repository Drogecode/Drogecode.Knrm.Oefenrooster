using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Enums;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Models.CalendarItems;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;
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
    [Inject] private ISessionExpireService SessionExpireService { get; set; } = default!;
    [Inject] private ScheduleRepository ScheduleRepository { get; set; } = default!;
    [Inject] private DayItemRepository DayItemRepository { get; set; } = default!;
    [Inject] private MonthItemRepository MonthItemRepository { get; set; } = default!;
    [Inject] private UserRepository UserRepository { get; set; } = default!;
    [Inject, NotNull] private CustomerSettingRepository? CustomerSettingRepository { get; set; }
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeUser>? Users { get; set; }
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; }
    [Parameter, EditorRequired] public List<DrogeVehicle>? Vehicles { get; set; }
    [Parameter, EditorRequired] public List<PlannerTrainingType>? TrainingTypes { get; set; }
    private static string _timeZone = "Europe/Amsterdam";
    private CancellationTokenSource _cls = new();
    private List<CalendarItem> _events = new();
    private List<UserTrainingCounter>? _userTrainingCounter;
    private List<RoosterItemMonth>? _monthItems;
    private DrogeUser? _user;
    private bool _updating;
    private bool _currentMonth;
    private bool _initialized;
    private ScheduleView _view = ScheduleView.Calendar;
    private DateTime? _month;
    private DateTime _firstMonth = DateTime.Today;

    protected override async Task OnInitializedAsync()
    {
        _firstMonth = await SessionExpireService.GetSelectedMonth(_cls.Token);
        Global.NewTrainingAddedAsync += HandleNewTraining;
        _user = await UserRepository.GetCurrentUserAsync();
        await SetMonth(_firstMonth);
        _initialized = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _timeZone = await CustomerSettingRepository.GetTimeZone(_cls.Token);
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
        _events = new();
        _userTrainingCounter = null;
        TrainingWeek scheduleForUser = new();
        var scheduleForAll = await ScheduleRepository.ScheduleForAll(dateRange, false, _cls.Token);
        if (scheduleForAll == null) return;
        _userTrainingCounter = scheduleForAll.UserTrainingCounters;
        var trainingsInRange = scheduleForAll.Planners;
        if (trainingsInRange != null && trainingsInRange.Count > 0)
        {
            scheduleForUser.From = DateOnly.FromDateTime(trainingsInRange[0].DateStart);
            var zone = TimeZoneInfo.FindSystemTimeZoneById(_timeZone);
            foreach (var training in trainingsInRange)
            {
                var dateStart = TimeZoneInfo.ConvertTimeFromUtc(training.DateStart, zone);
                var dateEnd = TimeZoneInfo.ConvertTimeFromUtc(training.DateEnd, zone);
                _events.Add(new ScheduleCalendarItem
                {
                    Start = dateStart,
                    End = dateEnd,
                    Training = training,
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
            var dayItems = await DayItemRepository.GetDayItemsAsync(dateRange, Guid.Empty, _cls.Token);
            if (dayItems?.DayItems != null)
            {
                foreach (var dayItem in dayItems.DayItems.Where(x => x.DateStart is not null))
                {

                    var userDeleted = false;
                    if (dayItem.LinkedUsers?.FirstOrDefault()?.UserId is not null)
                    {
                        var user = Users?.FirstOrDefault(x => x.Id == dayItem.LinkedUsers.FirstOrDefault()!.UserId);
                        user ??= await UserRepository.GetById(dayItem.LinkedUsers.FirstOrDefault()!.UserId, true);
                        if (user != null)
                        {
                            dayItem.Text += ": " + user.Name;
                        }
                        else
                        {
                            dayItem.Text += ": " + LApp["User not found or deleted"];
                            userDeleted = true;
                        }
                    }
                    _events.Add(new RoosterItemDayCalendarItem
                    {
                        Start = dayItem.DateStart!.Value,
                        End = dayItem.DateEnd,
                        AllDay = dayItem.IsFullDay,
                        ItemDay = dayItem,
                        UserDeleted = userDeleted,
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
        var asTraining = new PlannedTraining
        {
            TrainingId = newTraining.Id,
            Name = newTraining.Name,
            DateStart = DateTime.SpecifyKind((newTraining.Date ?? throw new ArgumentNullException("Date is null")) + (newTraining.TimeStart ?? throw new ArgumentNullException("StartTime is null")), DateTimeKind.Local).ToUniversalTime(),
            DateEnd = DateTime.SpecifyKind((newTraining.Date ?? throw new ArgumentNullException("Date is null")) + (newTraining.TimeEnd ?? throw new ArgumentNullException("StartTime is null")), DateTimeKind.Local).ToUniversalTime(),
            RoosterTrainingTypeId = newTraining.RoosterTrainingTypeId,
            ShowTime = newTraining.ShowTime,
            IsCreated = true,
            IsPinned = newTraining.IsPinned,
        };
        _events.Add(new ScheduleCalendarItem
        {
            Start = asTraining.DateStart,
            End = asTraining.DateEnd,
            Training = asTraining,
        });
        StateHasChanged();
    }

    public void Dispose()
    {
        Global.NewTrainingAddedAsync -= HandleNewTraining;
        _cls.Cancel();
    }
}
