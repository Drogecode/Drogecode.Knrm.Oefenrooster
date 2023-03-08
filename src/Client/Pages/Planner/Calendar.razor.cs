using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Heron.MudCalendar;
using Microsoft.Extensions.Localization;
using Microsoft.Graph.Models;
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
    private List<CustomItem> _events = new();
    private CancellationTokenSource _cls = new();
    private DateOnly? _dateOnly;
    private int? _month;
    private bool _updating;

    protected override async Task OnInitializedAsync()
    {
        _dateOnly = DateOnly.FromDateTime(DateTime.Today);
        Global.NewTrainingAddedAsync += HandleNewTraining;
    }

    private async Task SetCalenderForMonth(DateRange dateRange)
    {
        if (_updating) return;
        if (dateRange.Start == null) return;
        _updating = true;
        _events = new();
        _month = null;
        TrainingWeek scheduleForUser = new();
        var trainingsInWeek = (await _scheduleRepository.CalendarForUser(dateRange, _cls.Token))?.Trainings;
        if (trainingsInWeek != null && trainingsInWeek.Count > 0)
        {

            scheduleForUser.From = DateOnly.FromDateTime(trainingsInWeek[0].DateStart);
            foreach (var training in trainingsInWeek)
            {
                _events.Add(new CustomItem
                {
                    Start = training.DateStart,
                    End = training.DateEnd,
                    training = training,
                    Text = training.Availabilty.ToString() ?? "",
                    Color = HeaderClass(training.TrainingType)
                });
            }
        }
        _calendarForUser.AddLast(scheduleForUser);
        _updating = false;
        StateHasChanged();
    }
    private string HeaderClass(TrainingType trainingType)
    {
        switch (trainingType)
        {
            case TrainingType.EHBO:
                return "var(--mud-palette-warning-darken)";
            case TrainingType.OneOnOne:
                return "var(--mud-palette-tertiary-darken)";
            case TrainingType.FireBrigade:
                return "var(--mud-palette-error-darken)";
            case TrainingType.HRB:
                return "var(--mud-palette-success-lighten)";
            case TrainingType.Default:
            default:
                return "var(--mud-palette-lines-inputs)";
        }
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

    private async Task OnChange( CustomItem customItem)
    {
        if (_updating) return;
        _updating = true;
        var updatedTraining = await _scheduleRepository.PatchScheduleForUser(customItem.training, _cls.Token);
        customItem.training = updatedTraining;
        _updating = false;
    }

    public void Dispose()
    {
        Global.NewTrainingAddedAsync -= HandleNewTraining;
        _cls.Cancel();
    }
    private class CustomItem : CalendarItem
    {
        public Training training { get; set; }
        public string Color { get; set; } = "var(--mud-palette-grey-default)";
    }
}
