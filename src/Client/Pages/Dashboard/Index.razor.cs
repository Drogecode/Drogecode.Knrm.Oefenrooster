using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard;
public sealed partial class Index : IDisposable
{
    [Inject] private IStringLocalizer<Index> L { get; set; } = default!;
    [Inject] private FunctionRepository _functionRepository { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private TrainingTypesRepository _trainingTypesRepository { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private VehicleRepository _vehicleRepository { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private DrogeUser? _user;
    private List<DrogeFunction>? _functions;
    private List<PlannedTraining>? _futureTrainings;
    private List<Training>? _pinnedTrainings;
    private List<DrogeUser>? _users;
    private List<DrogeVehicle>? _vehicles;
    private List<PlannerTrainingType>? _trainingTypes;
    protected override async Task OnParametersSetAsync()
    {
        _users = await _userRepository.GetAllUsersAsync(false, false, _cls.Token);
        _vehicles = await _vehicleRepository.GetAllVehiclesAsync();
        _trainingTypes = await _trainingTypesRepository.GetTrainingTypes(false, _cls.Token);

        _user = await _userRepository.GetCurrentUserAsync();
        _functions = await _functionRepository.GetAllFunctionsAsync();
        _futureTrainings = (await _scheduleRepository.GetScheduledTrainingsForUser(_user?.Id, _cls.Token))?.Trainings;
        StateHasChanged();
        _pinnedTrainings = (await _scheduleRepository.GetPinnedTrainingsForUser(_cls.Token))?.Trainings;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
