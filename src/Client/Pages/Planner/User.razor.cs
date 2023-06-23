using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;

public sealed partial class User : IDisposable
{
    [Inject] private IStringLocalizer<User> L { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private TrainingTypesRepository _trainingTypesRepository { get; set; } = default!;
    [Inject] private FunctionRepository _functionRepository { get; set; } = default!;
    [Inject] private VehicleRepository _vehicleRepository { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Parameter] public Guid? Id { get; set; }
    private CancellationTokenSource _cls = new();
    private DrogeUser? _user;
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private List<PlannedTraining>? _trainings;
    private List<DrogeVehicle>? _vehicles;
    private List<PlannerTrainingType>? _trainingTypes;
    protected override async void OnInitialized()
    {
    }
    protected override async Task OnParametersSetAsync()
    {
        if (Id is not null)
        {
            _user = (await _userRepository.GetById(Id.Value));
            _trainings = (await _scheduleRepository.AllTrainingsForUser(Id.Value, _cls.Token))?.Trainings;
        }
        _users = await _userRepository.GetAllUsersAsync(true);
        _functions = await _functionRepository.GetAllFunctionsAsync();
        _vehicles = await _vehicleRepository.GetAllVehiclesAsync();
        _trainingTypes = await _trainingTypesRepository.GetTrainingTypes(_cls.Token);
    }
    private void ClickUser(DrogeUser user)
    {
        Navigation.NavigateTo($"/planner/user/{user.Id}");
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
