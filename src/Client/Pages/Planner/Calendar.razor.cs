using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;
public sealed partial class Calendar : IDisposable
{
    [Inject] private IStringLocalizer<Calendar> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter] public Guid CustomerId { get; set; } = Guid.Empty;
    private LinkedList<TrainingWeek> _calendarForUser = new();
    private CancellationTokenSource _cls = new();
    private int? _month;
    private int _high = -1;
    private int _low = -2;
    private bool _updating;

    protected override async Task OnInitializedAsync()
    {
        Global.NewTrainingAddedAsync += HandleNewTraining;
        for (int i = -1; i < 6; i++)
        {
            await AddWeekToCalendar(true);
        }
    }

    private async Task AddMultipeWeekToCalendar(bool high, int count)
    {
        for (int i = -1; i < count; i++)
        {
            await AddWeekToCalendar(high);
        }
    }

    private async Task AddWeekToCalendar(bool high)
    {
        if (_updating) return;
        _updating = true;
        TrainingWeek scheduleForUser = new();
        var trainingsInWeek = (await _scheduleRepository.CalendarForUser(high ? _high : _low, _cls.Token))?.Trainings;
        if (trainingsInWeek != null && trainingsInWeek.Count > 0)
        {
            scheduleForUser.From = DateOnly.FromDateTime(trainingsInWeek[0].DateStart);
            foreach (var training in trainingsInWeek)
            {
                scheduleForUser.Trainings.AddLast(training);
                scheduleForUser.Till = DateOnly.FromDateTime(training.DateStart);
            }
        }
        if (high)
        {
            _calendarForUser.AddLast(scheduleForUser);
            _high++;
        }
        else
        {
            _calendarForUser.AddFirst(scheduleForUser);
            _low--;
        }
        _updating= false;
        StateHasChanged();
    }
    private async Task HandleNewTraining(NewTraining newTraining, Guid newId)
    {
        if (newTraining.Date == null) return;
        var asTraining = new Training
        {
            TrainingId = newId,
            Name = newTraining.Name,
            DateStart = DateTime.SpecifyKind((newTraining.Date ?? throw new ArgumentNullException("Date is null")) + (newTraining.TimeStart ?? throw new ArgumentNullException("StartTime is null")), DateTimeKind.Utc),
            DateEnd = DateTime.SpecifyKind((newTraining.Date ?? throw new ArgumentNullException("Date is null")) + (newTraining.TimeEnd ?? throw new ArgumentNullException("StartTime is null")), DateTimeKind.Utc),
            TrainingType = newTraining.TrainingType
        };
        var date = DateOnly.FromDateTime(newTraining.Date ?? throw new UnreachableException("newTraining.Date is null after null check"));
        foreach (var week in _calendarForUser)
        {
            if (week.From <= date && week.Till >= date)
            {
                var node = week.Trainings.First;
                LinkedListNode<Training>? last = null;
                while (node != null)
                {
                    if (node.Value.DateStart.CompareTo(newTraining.Date) >= 0)
                    {
                        week.Trainings.AddAfter(node, asTraining);
                        StateHasChanged();
                        return;
                    }
                    last = node;
                    node = node.Next;
                }
                if (last != null)
                {
                    week.Trainings.AddAfter(last, asTraining);
                    StateHasChanged();
                }
                return;
            }
        }
    }

    public void Dispose()
    {
        Global.NewTrainingAddedAsync -= HandleNewTraining;
        _cls.Cancel();
    }
}
