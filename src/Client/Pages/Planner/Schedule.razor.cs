using Drogecode.Knrm.Oefenrooster.Client.Helpers;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Heron.MudCalendar;
using Microsoft.Extensions.Localization;
using Microsoft.Graph.Models;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;

public sealed partial class Schedule : IDisposable
{
    [Inject] private IStringLocalizer<Schedule> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private FunctionRepository _functionRepository { get; set; } = default!;
    [Inject] private VehicleRepository _vehicleRepository { get; set; } = default!;
    [Parameter] public Guid CustomerId { get; set; } = Guid.Empty;
    private LinkedList<List<PlannedTraining>> _scheduleForUser = new();
    private CancellationTokenSource _cls = new();
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private List<DrogeVehicle>? _vehicles;
    private List<CustomItem> _events = new();
    private List<UserTrainingCounter>? _userTrainingCounter;
    private List<PlannerTrainingType>? _trainingTypes;
    private bool _updating;

    protected override async Task OnInitializedAsync()
    {
        _users = await _userRepository.GetAllUsersAsync();
        _functions = await _functionRepository.GetAllFunctionsAsync();
        _vehicles = await _vehicleRepository.GetAllVehiclesAsync();
        _trainingTypes = await _scheduleRepository.GetTrainingTypes(_cls.Token);
    }

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
                var trainingType = (_trainingTypes?.FirstOrDefault(x => x.Id == training.RoosterTrainingTypeId)) ?? (_trainingTypes?.FirstOrDefault(x => x.IsDefault));
                _events.Add(new CustomItem
                {
                    Start = training.DateStart,
                    End = training.DateEnd,
                    Training = training,
                    ColorStyle = PlannerHelper.HeaderStyle(trainingType?.ColorLight)
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
        public string ColorStyle { get; set; } = $"background-color: {MudBlazor.Color.Default}";
    }
}
