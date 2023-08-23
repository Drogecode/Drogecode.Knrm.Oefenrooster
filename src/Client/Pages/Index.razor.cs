using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.Extensions.Localization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages;
public sealed partial class Index : IDisposable
{
    [Inject] private IStringLocalizer<Index> L { get; set; } = default!;
    [Inject] private FunctionRepository _functionRepository { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private TrainingTypesRepository _trainingTypesRepository{ get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private SharePointRepository _sharePointRepository{ get; set; } = default!;
    [Inject] private VehicleRepository _vehicleRepository { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private DrogeUser? _user;
    private List<DrogeFunction>? _functions;
    private List<PlannedTraining>? _futureTrainings;
    private List<Training>? _pinnedTrainings;
    private List<SharePointTraining>? _sharePointTrainings;
    private List<SharePointAction>? _sharePointActions;
    private List<DrogeUser>? _users;
    private IEnumerable<DrogeUser> _selectedUsersAction = new List<DrogeUser>();
    private List<DrogeVehicle>? _vehicles;
    private List<PlannerTrainingType>? _trainingTypes; 
    protected override async Task OnParametersSetAsync()
    {
        _users = await _userRepository.GetAllUsersAsync(false);
        _vehicles = await _vehicleRepository.GetAllVehiclesAsync();
        _trainingTypes = await _trainingTypesRepository.GetTrainingTypes(_cls.Token);


        _user = await _userRepository.GetCurrentUserAsync();//Force creation of user.
        if (_user is not null)
        {
            var thisUser = _users!.FirstOrDefault(x => x.Id == _user.Id);
            if (thisUser is not null)
            {
                ((List<DrogeUser>)_selectedUsersAction).Add(thisUser);
            }
        }
        _functions = await _functionRepository.GetAllFunctionsAsync();
        _futureTrainings = (await _scheduleRepository.GetScheduledTrainingsForUser(_user?.Id, _cls.Token))?.Trainings;
        StateHasChanged();
        _pinnedTrainings = (await _scheduleRepository.GetPinnedTrainingsForUser(_cls.Token))?.Trainings;
        StateHasChanged();
        _sharePointActions = await _sharePointRepository.GetLastActionsForCurrentUser(10, 0, _cls.Token);
        StateHasChanged();
        _sharePointTrainings = await _sharePointRepository.GetLastTrainingsForCurrentUser(10, 0, _cls.Token);
    }
    private string GetMultiSelectionText(List<DrogeUser> selectedValues)
    {
        var result = string.Join(", ", selectedValues.Select(x => x.Name));
        return result;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
