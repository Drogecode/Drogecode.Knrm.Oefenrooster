using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class ScheduleTable
{
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeUser>? Users { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeVehicle>? Vehicles { get; set; } = default!;
    [Parameter, EditorRequired] public List<PlannerTrainingType>? TrainingTypes { get; set; } = default!;

    private CancellationTokenSource _cls = new();
    private bool _updating;
    private List<PlannedTraining> _events = new();
    private List<UserTrainingCounter>? _userTrainingCounter;

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
}
