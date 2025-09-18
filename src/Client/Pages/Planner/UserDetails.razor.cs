using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;

public sealed partial class UserDetails : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<UserDetails>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private UserRepository? UserRepository { get; set; }
    [Inject, NotNull] private IUserClient? UserClient { get; set; }
    [Inject, NotNull] private IUserRoleClient? UserRoleClient { get; set; }
    [Inject, NotNull] private ScheduleRepository? ScheduleRepository { get; set; }
    [Inject, NotNull] private TrainingTypesRepository? TrainingTypesRepository { get; set; }
    [Inject, NotNull] private FunctionRepository? FunctionRepository { get; set; }
    [Inject, NotNull] private VehicleRepository? VehicleRepository { get; set; }
    [Inject, NotNull] private TrainingTargetRepository? TrainingTargetRepository { get; set; }
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter] public Guid? Id { get; set; }
    private readonly CancellationTokenSource _cls = new();
    private MultipleDrogeUserRolesBasicResponse? _userRoles;
    private MultipleLinkedUserRolesResponse? _userLinkRoles;
    private GetScheduledTrainingsForUserResponse? _trainings;
    private GetUserMonthInfoResponse? _userMonthInfo;
    private DrogeUser? _user;
    private DrogeFunction? _userFunction;
    private List<Guid?>? _userRolesForUser;
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private List<DrogeVehicle>? _vehicles;
    private List<PlannerTrainingType>? _trainingTypes;
    private List<UserResultForTarget>? _userResultForTargets;
    private IEnumerable<DrogeUser>? _selectedUsersAction;
    private RatingPeriod _ratingPeriod = RatingPeriod.SelectValue;
    private bool _updatingSelection;
    private bool _updatingRoles;
    private bool _loadingTrainings;
    private bool _loadingRating;
    private const int TAKE = 15;
    private int _total = TAKE;
    private int _skip;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _users = await UserRepository.GetAllUsersAsync(false, false, false, _cls.Token);
            _functions = await FunctionRepository.GetAllFunctionsAsync(false, _cls.Token);
            _vehicles = await VehicleRepository.GetAllVehiclesAsync(false, _cls.Token);
            _trainingTypes = await TrainingTypesRepository.GetTrainingTypes(false, false, _cls.Token);
            if (Id is not null)
            {
                _user = await UserRepository.GetById(Id.Value, false);
                StateHasChanged();
                if (_user?.LinkedAsA is not null)
                {
                    var newList = new List<DrogeUser>();
                    foreach (var linkedUser in _user.LinkedAsA.Where(x => x.LinkType == UserUserLinkType.Buddy))
                    {
                        var linked = await UserRepository.GetById(linkedUser.LinkedUserId, false);
                        if (linked is not null)
                            newList.Add(linked);
                    }

                    _selectedUsersAction = newList;
                }
                else
                    _selectedUsersAction = [];

                _trainings = await ScheduleRepository.AllTrainingsForUser(Id.Value, TAKE, _skip * TAKE, _cls.Token);
                StateHasChanged();
                _userFunction = _functions?.FirstOrDefault(x => x.Id == _user?.UserFunctionId);
                _userMonthInfo = await ScheduleRepository.GetUserMonthInfo(Id.Value, _cls.Token);

                if (AuthenticationState is not null)
                {
                    var authState = await AuthenticationState;
                    var user = authState.User;
                    if (user.IsInRole(AccessesNames.AUTH_users_add_role))
                    {
                        _userRoles = await UserRoleClient.GetAllBasicAsync(_cls.Token);
                        _userLinkRoles = await UserClient.GetRolesForUserByIdAsync(Id.Value, _cls.Token);
                        _userRolesForUser = [];
                        if (_userLinkRoles?.Roles is not null)
                        {
                            foreach (var role in _userLinkRoles.Roles.Where(x => x.IsSet))
                            {
                                _userRolesForUser.Add(role.Id);
                            }
                        }
                        else if (_userLinkRoles is not null)
                        {
                            _userLinkRoles.Roles = [];
                        }
                    }
                }
            }

            StateHasChanged();
        }
    }

    private async Task UserFunctionChanged(Guid? newFunction)
    {
        if (_user is null || newFunction is null)
            return;
        _user.UserFunctionId = newFunction;
        await UserRepository.UpdateUserAsync(_user);
    }

    private string GetSelectedRolesText(List<Guid?> roleIds)
    {
        var result = string.Empty;
        foreach (var roleId in roleIds)
        {
            result += $"{_userRoles?.Roles?.FirstOrDefault(x => x.Id == roleId)?.Name}, ";
        }

        return result.TrimEnd(',', ' ');
    }

    private async Task UserRolesChanged(IEnumerable<Guid?>? userRoles)
    {
        userRoles ??= [];
        var userRolesList = userRoles.ToList();
        DebugHelper.WriteLine($"UserRolesChanged - {userRolesList.Count} roles");
        if (_updatingRoles || Id is null)
            return;
        _updatingRoles = true;
        StateHasChanged();
        try
        {
            _userRolesForUser ??= [];
            var newRolesAsList = userRolesList.ToList();
            var removedRoles = new List<Guid?>();
            foreach (var oldRole in _userRolesForUser)
            {
                if (newRolesAsList.Contains(oldRole))
                {
                    DebugHelper.WriteLine($"UserRolesChanged - ignore role {oldRole}");
                    newRolesAsList.Remove(oldRole);
                }
                else
                {
                    DebugHelper.WriteLine($"UserRolesChanged - Unlinking role {oldRole}");
                    var drogeUserRoleLinked = _userLinkRoles?.Roles?.FirstOrDefault(x => x.Id == oldRole);
                    if (drogeUserRoleLinked is null)
                    {
                        DebugHelper.WriteLine($"UserRolesChanged - Role {oldRole} not found");
                        continue;
                    }

                    drogeUserRoleLinked.IsSet = false;
                    await UserRoleClient.LinkUserToRoleAsync(Id.Value, drogeUserRoleLinked, _cls.Token);
                    removedRoles.Add(oldRole);
                    _userLinkRoles!.Roles!.Remove(drogeUserRoleLinked);
                }
            }

            foreach (var removedRole in removedRoles)
            {
                _userRolesForUser.Remove(removedRole);
            }

            foreach (var newRole in newRolesAsList)
            {
                DebugHelper.WriteLine($"UserRolesChanged - Linking role {newRole}");
                var drogeUserRoleLinked = _userLinkRoles?.Roles?.FirstOrDefault(x => x.Id == newRole);
                if (drogeUserRoleLinked is null)
                {
                    var drogeUserRoleBasic = _userRoles?.Roles?.FirstOrDefault(x => x.Id == newRole);
                    if (drogeUserRoleBasic is null)
                    {
                        DebugHelper.WriteLine($"UserRolesChanged - Role {newRole} not found");
                        continue;
                    }

                    drogeUserRoleLinked = drogeUserRoleBasic.ToDrogeUserRoleLinked();
                }

                drogeUserRoleLinked.IsSet = true;
                drogeUserRoleLinked.SetExternal = false;
                await UserRoleClient.LinkUserToRoleAsync(Id.Value, drogeUserRoleLinked, _cls.Token);
                _userRolesForUser.Add(newRole);
                _userLinkRoles?.Roles?.Add(drogeUserRoleLinked);
            }
        }
        finally
        {
            _updatingRoles = false;
            StateHasChanged();
        }
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        try
        {
            var selectionList = selection.ToList();
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
                await UserRepository.UpdateLinkUserUserForUserAsync(body, _cls.Token);
            }

            foreach (var newSelected in selectionList)
            {
                body.UserBId = newSelected.Id;
                body.Add = true;
                await UserRepository.UpdateLinkUserUserForUserAsync(body, _cls.Token);
            }

            _selectedUsersAction = selectionList;
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
        await UserRepository.UpdateUserAsync(_user);
    }

    private async Task LoadMore()
    {
        if (_loadingTrainings || Id is null)
            return;
        _loadingTrainings = true;
        StateHasChanged();
        _skip++;
        var newTrainings = (await ScheduleRepository.AllTrainingsForUser(Id.Value, TAKE, _skip * TAKE, _cls.Token));
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

        _loadingTrainings = false;
        StateHasChanged();
    }

    private async Task RatingPeriodChanged(RatingPeriod value)
    {
        if (_loadingRating)
            return;
        _loadingRating = true;
        StateHasChanged();
        _ratingPeriod = value;

        if (Id is not null)
        {
            _userResultForTargets = (await TrainingTargetRepository.GetAllResultForUser(Id.Value, _ratingPeriod, _cls.Token))?.UserResultForTargets;
        }

        _loadingRating = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}