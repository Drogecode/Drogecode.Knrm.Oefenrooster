using Drogecode.Knrm.Oefenrooster.Client.Enums;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
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
    [Parameter, EditorRequired] public List<DrogeUser>? Users { get; set; }
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; }
    [Parameter, EditorRequired] public List<DrogeVehicle>? Vehicles { get; set; }
    [Parameter, EditorRequired] public List<PlannerTrainingType>? TrainingTypes { get; set; }
    private CancellationTokenSource _cls = new();
    private List<CustomItem> _events = new();
    private List<UserTrainingCounter>? _userTrainingCounter;
    private bool _updating;
    private ScheduleView _view = ScheduleView.Calendar;

    private async Task SetCalenderForMonth(DateRange dateRange)
    {
        if (_updating || dateRange.Start == null) return;
        _updating = true;
        _events = new();
        _userTrainingCounter = null;
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
                _events.Add(new CustomItem
                {
                    Start = training.DateStart,
                    End = training.DateEnd,
                    Training = training,
                });
            }
        }
        _updating = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
    private class CustomItem : CalendarItem
    {
        public PlannedTraining? Training { get; set; }
    }
}
