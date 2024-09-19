using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.Extensions.Localization;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;

public sealed partial class UserDetails : IDisposable
{
    [Inject] private IStringLocalizer<UserDetails> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private TrainingTypesRepository _trainingTypesRepository { get; set; } = default!;
    [Inject] private FunctionRepository _functionRepository { get; set; } = default!;
    [Inject] private VehicleRepository _vehicleRepository { get; set; } = default!;
    [Parameter] public Guid? Id { get; set; }
    private CancellationTokenSource _cls = new();
    private DrogeUser? _user;
    private DrogeFunction? _userFunction;
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private GetScheduledTrainingsForUserResponse? _trainings;
    private List<DrogeVehicle>? _vehicles;
    private List<PlannerTrainingType>? _trainingTypes;
    private IEnumerable<DrogeUser> _selectedUsersAction;
    private bool _updatingSelection = false;
    private const int TAKE = 15;
    private int _total = TAKE;
    private int _skip = 0;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _users = await _userRepository.GetAllUsersAsync(false, false, false, _cls.Token);
            _functions = await _functionRepository.GetAllFunctionsAsync(false, _cls.Token);
            _vehicles = await _vehicleRepository.GetAllVehiclesAsync(false, _cls.Token);
            _trainingTypes = await _trainingTypesRepository.GetTrainingTypes(false, false, _cls.Token);
            if (Id is not null)
            {
                _user = await _userRepository.GetById(Id.Value, false);
                if (_user?.LinkedAsA is not null)
                {
                    var newList = new List<DrogeUser>();
                    foreach (var linkedUser in _user.LinkedAsA.Where(x => x.LinkType == UserUserLinkType.Buddy))
                    {
                        var linked = await _userRepository.GetById(linkedUser.LinkedUserId, false);
                        if (linked is not null)
                            newList.Add(linked);
                    }

                    _selectedUsersAction = newList;
                }
                else
                    _selectedUsersAction = [];

                _trainings = (await _scheduleRepository.AllTrainingsForUser(Id.Value, TAKE, _skip * TAKE, _cls.Token));
                _userFunction = _functions?.FirstOrDefault(x => x.Id == _user?.UserFunctionId);
            }

            StateHasChanged();
        }
    }

    private async Task UserFunctionChanged(Guid? newFunction)
    {
        if (_user is null || newFunction is null)
            return;
        _user.UserFunctionId = newFunction;
        await _userRepository.UpdateUserAsync(_user);
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        try
        {
            if (_updatingSelection || Id is null || _selectedUsersAction is null)
                return;
            _updatingSelection = true;
            var body = new UpdateLinkUserUserForUserRequest
            {
                UserAId = Id.Value,
                LinkType = UserUserLinkType.Buddy,
            };
            foreach (var oldSelected in _selectedUsersAction)
            {
                body.UserBId = oldSelected.Id;
                body.Add = false;
                await _userRepository.UpdateLinkUserUserForUserAsync(body, _cls.Token);
            }

            foreach (var newSelected in selection)
            {
                body.UserBId = newSelected.Id;
                body.Add = true;
                await _userRepository.UpdateLinkUserUserForUserAsync(body, _cls.Token);
            }

            _selectedUsersAction = selection;
            _updatingSelection = false;
        }
        catch (Exception ex)
        {
            DebugHelper.WriteLine(ex);
            Debugger.Break();
            _updatingSelection = false;
        }
    }

    private async Task UserNumberChanged(int? newNumber)
    {
        if (_user is null)
            return;
        _user.Nr = newNumber;
        await _userRepository.UpdateUserAsync(_user);
    }

    private async Task LoadMore()
    {
        _skip++;
        var newTrainings = (await _scheduleRepository.AllTrainingsForUser(Id.Value, TAKE, _skip * TAKE, _cls.Token));
        if (_trainings is null || newTrainings is null || newTrainings.TotalCount != _trainings.TotalCount)
        {
            _trainings = newTrainings;
            _total = TAKE;
            _skip = 0;
        }
        else
        {
            _trainings.Trainings.AddRange(newTrainings.Trainings);
            _total += TAKE;
        }
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}