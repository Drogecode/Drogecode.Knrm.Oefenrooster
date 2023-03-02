using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using MudBlazor;
using MudBlazor.Extensions;
using System;
using System.Diagnostics;
using System.Xml.Serialization;

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
    private DateOnly? _dateOnly;
    private int? _month;
    private bool _updating;

    protected override async Task OnInitializedAsync()
    {
        _dateOnly = DateOnly.FromDateTime(DateTime.Today);
        Global.NewTrainingAddedAsync += HandleNewTraining;
        await SetCalenderForMonth(_dateOnly ?? throw new ArgumentNullException());
    }

    private async Task SelectionChanged(DateOnly dateOnly)
    {

        if (_updating) return;
        _dateOnly = dateOnly;
        await SetCalenderForMonth(dateOnly);
        StateHasChanged();
    }
    private async Task SetCalenderForMonth(DateOnly dateOnly)
    {
        if (_updating) return;
        _updating = true;
        _calendarForUser = new();
        _month = null;
        var lastStart = DateTime.MinValue;
        TrainingWeek scheduleForUser = new();
        var trainingsInWeek = (await _scheduleRepository.CalendarForUser(dateOnly, _cls.Token))?.Trainings;
        if (trainingsInWeek != null && trainingsInWeek.Count > 0)
        {

            scheduleForUser.From = DateOnly.FromDateTime(trainingsInWeek[0].DateStart);
            foreach (var training in trainingsInWeek)
            {
                var d = training.DateStart.StartOfWeek(DayOfWeek.Monday);
                if (lastStart.CompareTo(d) < 0 )
                {
                    _calendarForUser.AddLast(scheduleForUser);
                    scheduleForUser = new();
                    lastStart = d;
                }
                scheduleForUser.Trainings.AddLast(training);
                scheduleForUser.Till = DateOnly.FromDateTime(training.DateStart);
            }
        }
        _calendarForUser.AddLast(scheduleForUser);
        _updating = false;
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
    private async Task ControlButtonClicked(Page page)
    {
        switch (page)
        {
            case Page.Next:
                _dateOnly = _dateOnly.Value.AddMonths(1);
                await SetCalenderForMonth(_dateOnly ?? throw new ArgumentNullException());
                break;
            case Page.Previous:
                _dateOnly = _dateOnly.Value.AddMonths(-1);
                await SetCalenderForMonth(_dateOnly ?? throw new ArgumentNullException());
                break;
        }
        StateHasChanged();
    }

    public void Dispose()
    {
        Global.NewTrainingAddedAsync -= HandleNewTraining;
        _cls.Cancel();
    }
}
