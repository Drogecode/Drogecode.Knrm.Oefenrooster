using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.Extensions.Localization;
using System.Numerics;
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
    private DrogeFunction? _userFunction;
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private GetScheduledTrainingsForUserResponse? _trainings;
    private List<DrogeVehicle>? _vehicles;
    private List<PlannerTrainingType>? _trainingTypes;
    private IEnumerable<DrogeUser> _selectedUsersAction = new List<DrogeUser>();
    private bool _updatingSelection = false;
    private bool _loadingUserInfo = true;
    protected override void OnInitialized()
    {
        _trainings = null;
        StateHasChanged();
    }
    protected override async Task OnParametersSetAsync()
    {
        _selectedUsersAction = new List<DrogeUser>();
        _users = await _userRepository.GetAllUsersAsync(false, false, _cls.Token);
        _functions = await _functionRepository.GetAllFunctionsAsync();
        _vehicles = await _vehicleRepository.GetAllVehiclesAsync();
        _trainingTypes = await _trainingTypesRepository.GetTrainingTypes(false, _cls.Token);
        if (Id is not null)
        {
            _loadingUserInfo = true;
            StateHasChanged();
            if (_user?.LinkedAsA is not null)
            {
                var newList = new List<DrogeUser>();
                foreach (var linkedUser in _user.LinkedAsA.Where(x => x.LinkType == UserUserLinkType.Buddy))
                {
                    var linked = await _userRepository.GetById(linkedUser.LinkedUserId);
                    if (linked is not null)
                        newList.Add(linked);
                }
                _selectedUsersAction = _selectedUsersAction.Concat(newList);
                StateHasChanged();
            }
            _user = await _userRepository.GetById(Id.Value);
            _trainings = (await _scheduleRepository.AllTrainingsForUser(Id.Value, _cls.Token));
            _userFunction = _functions?.FirstOrDefault(x => x.Id == _user?.UserFunctionId);
            _loadingUserInfo = false;
        }
        StateHasChanged();
    }

    private void ClickUser(DrogeUser user)
    {
        Navigation.NavigateTo($"/planner/user/{user.Id}");
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
        if (_updatingSelection)
            return;
        _updatingSelection = true;
        if (Id is null)
            return;
        var body = new UpdateLinkUserUserForUserRequest
        {
            UserAId = Id.Value,
            LinkType = UserUserLinkType.Buddy,
        };
        if (_selectedUsersAction.Any())
        {
            body.UserBId = _selectedUsersAction.FirstOrDefault()!.Id;
            body.Add = false;
            await _userRepository.UpdateLinkUserUserForUserAsync(body, _cls.Token);
        }
        if (selection.Any())
        {
            body.UserBId = selection.FirstOrDefault()!.Id;
            body.Add = true;
            await _userRepository.UpdateLinkUserUserForUserAsync(body, _cls.Token);
        }
        _selectedUsersAction = selection;
        _updatingSelection = false;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
