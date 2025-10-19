using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Exceptions;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components.Dialogs;

public sealed partial class ScheduleDialog : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<ScheduleDialog>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private ScheduleRepository? ScheduleRepository { get; set; }
    [Inject, NotNull] private VehicleRepository? VehicleRepository { get; set; }
    [Inject, NotNull] private UserRepository? UserRepository { get; set; }
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter] public PlannedTraining Planner { get; set; } = default!;
    [Parameter] public List<DrogeUser>? Users { get; set; }
    [Parameter] public List<DrogeFunction>? Functions { get; set; }
    [Parameter] public List<DrogeVehicle>? Vehicles { get; set; }
    [Parameter] public RefreshModel Refresh { get; set; } = default!;
    [Parameter] public MainLayout MainLayout { get; set; } = default!;
    private readonly CancellationTokenSource _cls = new();
    private List<DrogeLinkVehicleTraining>? _linkVehicleTraining;
    private List<DrogeVehicle> _vehicleInfoForThisTraining = [];
    private readonly List<Guid> _isUpdating = [];
    private Guid? _currentUserId;
    private Guid? _specialFunctionId;
    private bool _plannerIsUpdated;
    private bool _showWoeps;
    private bool _canEdit;
    private bool _authEditOtherUser;
    private bool _authEditSelf;
    private bool _showPadlock;
    private bool _isLoading = true;
    private int _vehicleCount;
    private int _column1 = 2;
    private int _column2 = 3;
    private int _column3 = 3;
    private int _column4 = 3;
    private int _column5 = 1;

    void Submit() => MudDialog.Close(DialogResult.Ok(true));

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Task<GetPlannedTrainingResponse?>? latestVersionTask = null;
            _specialFunctionId = Functions?.FirstOrDefault(x => x.Special)?.Id;
            if (Planner.TrainingId is not null && !Planner.TrainingId.Equals(Guid.Empty))
                latestVersionTask = ScheduleRepository.GetPlannedTrainingById(Planner.TrainingId, _cls.Token);
            else if (Planner.DefaultId is not null && !Planner.DefaultId.Equals(Guid.Empty))
                latestVersionTask = ScheduleRepository.GetPlannedTrainingForDefaultDate(Planner.DateStart, Planner.DefaultId, _cls.Token);
            if (Planner.TrainingId != null)
                _linkVehicleTraining = await VehicleRepository.GetForTrainingAsync(Planner.TrainingId ?? throw new DrogeCodeNullException("Planner.TrainingId"));
            else if (Planner.DefaultId is not null)
                _linkVehicleTraining = await VehicleRepository.GetForDefaultAsync(Planner.DefaultId ?? throw new DrogeCodeNullException("Planner.DefaultId"));
            if (Vehicles != null && _linkVehicleTraining != null)
            {
                _vehicleInfoForThisTraining = [];
                var count = 0;
                foreach (var vehicle in Vehicles)
                {
                    bool? isSelected = _linkVehicleTraining?.FirstOrDefault(x => x.VehicleId == vehicle.Id)?.IsSelected;
                    if (isSelected == true || (isSelected == null && vehicle.IsDefault))
                    {
                        _vehicleInfoForThisTraining.Add(vehicle);
                        count++;
                    }
                }

                _vehicleCount = count;
            }

            PlannedTraining? latestVersion = null;
            ClaimsPrincipal? user = null;
            if (AuthenticationState is not null)
            {
                var authState = await AuthenticationState;
                user = authState.User;
                _authEditOtherUser = user.IsInRole(AccessesNames.AUTH_scheduler_other);
                _authEditSelf = user.IsInRole(AccessesNames.AUTH_scheduler_self);
                if (!_authEditOtherUser)
                {
                    var dbUser = await UserRepository.GetCurrentUserAsync();
                    _currentUserId = dbUser?.Id;
                }
            }

            if (latestVersionTask is not null)
                latestVersion = (await latestVersionTask)?.Training;
            if (latestVersion is not null)
            {
                Planner.PlanUsers = latestVersion.PlanUsers;
                _plannerIsUpdated = true;
            }
            else
                _showWoeps = true;
            
            var canEditModel = PlannerHelper.CanEdit(Planner.DateEnd, user);
            _canEdit = canEditModel.CanEdit;
            _showPadlock = canEditModel.ShowPadlock;

            if (!_canEdit)
            {
                _showPadlock = true;
            }

            _isLoading = false;
            MudDialog.StateHasChanged();
            StateHasChanged();
        }
    }

    private async Task ClickSpecialFunction(PlanUser user)
    {
        if (!_canEdit) return;
        if (user.PlannedFunctionId.Equals(_specialFunctionId))
            await FunctionSelectionChanged(user, user.UserFunctionId);
        else
            await FunctionSelectionChanged(user, _specialFunctionId);
    }

    private async Task CheckChanged(bool toggled, PlanUser user, Guid functionId)
    {
        if (!_canEdit || _isUpdating.Contains(user.UserId)) return;
        _isUpdating.Add(user.UserId);
        StateHasChanged();
        try
        {
            user.Assigned = toggled;
            user.VehicleId = _vehicleInfoForThisTraining.FirstOrDefault(x => x.IsDefault)?.Id ?? _vehicleInfoForThisTraining.FirstOrDefault()?.Id;
            if (toggled)
                user.PlannedFunctionId = functionId;
            else
                user.PlannedFunctionId = user.UserFunctionId;
            var result = await ScheduleRepository.PatchAssignedUser(Planner.TrainingId, Planner, user, AuditReason.Assigned);
            if (Planner.TrainingId is null || Planner.TrainingId.Equals(Guid.Empty))
                Planner.TrainingId = result.IdPatched;
            MainLayout.ShowSnackbarAssignmentChanged(user, Planner);
            await Refresh.CallRequestRefreshAsync();
        }
        finally
        {
            _isUpdating.Remove(user.UserId);
            StateHasChanged();
        }
    }

    private async Task CheckChanged(bool toggled, DrogeUser user, Guid functionId)
    {
        if (!_canEdit || _isUpdating.Contains(user.Id)) return;
        _isUpdating.Add(user.Id);
        StateHasChanged();
        try
        {
            // Add to the schedule with a new status to indicate it was not set by the user.
            var result = await ScheduleRepository.PutAssignedUser(toggled, Planner.TrainingId, functionId, user, Planner);
            if (Planner.TrainingId is null || Planner.TrainingId.Equals(Guid.Empty))
                Planner.TrainingId = result.IdPut;
            var planUser = Planner.PlanUsers.FirstOrDefault(x => x.UserId == user.Id);
            if (planUser is null)
            {
                planUser = new PlanUser
                {
                    UserId = user.Id,
                    TrainingId = Planner.TrainingId,
                    UserFunctionId = user.UserFunctionId,
                    PlannedFunctionId = functionId,
                    Availability = Availability.None,
                    AvailableId = result.AvailableId,
                    Assigned = toggled,
                    Name = user.Name,
                    VehicleId = _vehicleInfoForThisTraining.FirstOrDefault(x => x.IsDefault)?.Id ?? _vehicleInfoForThisTraining.FirstOrDefault()?.Id,
                };
                Planner.PlanUsers.Add(planUser);
            }
            else
            {
                planUser.Assigned = toggled;
                if (toggled)
                    planUser.PlannedFunctionId = functionId;
                else
                    planUser.PlannedFunctionId = planUser.UserFunctionId;
            }

            MainLayout.ShowSnackbarAssignmentChanged(planUser, Planner);
            await Refresh.CallRequestRefreshAsync();
        }
        finally
        {
            _isUpdating.Remove(user.Id);
            StateHasChanged();
        }
    }

    private Color GetColor(Availability? availabilty)
    {
        switch (availabilty)
        {
            case Availability.Available:
                return Color.Success;
            case Availability.NotAvailable:
                return Color.Error;
            case Availability.Maybe:
                return Color.Warning;
            default: return Color.Inherit;
        }
    }

    private async Task VehicleSelectionChanged(PlanUser user, Guid? id)
    {
        if (!_canEdit) return;
        user.VehicleId = id;
        await ScheduleRepository.PatchAssignedUser(Planner.TrainingId, null, user, AuditReason.ChangeVehicle);
        await Refresh.CallRequestRefreshAsync();
    }

    private async Task FunctionSelectionChanged(PlanUser user, Guid? id)
    {
        if (!_canEdit) return;
        user.ClickedFunction = false;
        user.PlannedFunctionId = id;
        await ScheduleRepository.PatchAssignedUser(Planner.TrainingId, null, user, AuditReason.ChangedFunction);
        await Refresh.CallRequestRefreshAsync();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}