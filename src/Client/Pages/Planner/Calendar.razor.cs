using Drogecode.Knrm.Oefenrooster.Client.Helpers;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Heron.MudCalendar;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;
public sealed partial class Calendar : IDisposable
{
    [Inject] private IStringLocalizer<Calendar> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter] public Guid CustomerId { get; set; } = Guid.Empty;
    private List<CustomItem> _events = new();
    private CancellationTokenSource _cls = new();
    private bool _updating;

    protected override async Task OnInitializedAsync()
    {
        Global.NewTrainingAddedAsync += HandleNewTraining;
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
                _events.Add(new CustomItem
                {
                    Start = training.DateStart,
                    End = training.DateEnd,
                    Training = training,
                    Text = training.Availabilty.ToString() ?? "",
                    Color = PlannerHelper.HeaderClass(training.TrainingType)
                });
            }
        }
        _updating = false;
        StateHasChanged();
    }

    private async Task OnChange(CustomItem customItem)
    {
        if (_updating) return;
        _updating = true;
        var updatedTraining = await _scheduleRepository.PatchScheduleForUser(customItem.Training, _cls.Token);
        customItem.Training = updatedTraining;
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
        _events.Add(new CustomItem
        {
            Start = asTraining.DateStart,
            End = asTraining.DateEnd,
            Training = asTraining,
            Text = asTraining.Availabilty.ToString() ?? "",
            Color = PlannerHelper.HeaderClass(asTraining.TrainingType)
        });
        StateHasChanged();
    }

    public void Dispose()
    {
        Global.NewTrainingAddedAsync -= HandleNewTraining;
        _cls.Cancel();
    }
    private class CustomItem : CalendarItem
    {
        public Training? Training { get; set; }
        public string Color { get; set; } = "var(--mud-palette-grey-default)";
    }
}
