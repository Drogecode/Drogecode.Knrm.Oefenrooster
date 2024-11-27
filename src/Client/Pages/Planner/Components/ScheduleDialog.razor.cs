using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class ScheduleDialog : IDisposable
{
    [Inject] private IStringLocalizer<ScheduleDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private VehicleRepository _vehicleRepository { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter] public PlannedTraining Planner { get; set; } = default!;
    [Parameter] public List<DrogeUser>? Users { get; set; }
    [Parameter] public List<DrogeFunction>? Functions { get; set; }
    [Parameter] public List<DrogeVehicle>? Vehicles { get; set; }
    [Parameter] public RefreshModel Refresh { get; set; } = default!;
    [Parameter] public MainLayout MainLayout { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<DrogeLinkVehicleTraining>? _linkVehicleTraining;
    private List<DrogeVehicle> _vehicleInfoForThisTraining = [];
    private Guid? _currentUserId = null;
    private bool _plannerIsUpdated;
    private bool _showWoeps;
    private bool _canEdit;
    private bool _authEditOtherUser;
    private bool _showPadlock;
    private bool _isLoading = true;
    private int _vehicleCount;
    private int _colmn1 = 2;
    private int _colmn2 = 3;
    private int _colmn3 = 3;
    private int _colmn4 = 3;
    private int _colmn5 = 1;

    void Submit() => MudDialog.Close(DialogResult.Ok(true));
    void Cancel() => MudDialog.Cancel();
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Task<GetPlannedTrainingResponse?>? latestVersionTask = null;
            if (Planner.TrainingId is not null && !Planner.TrainingId.Equals(Guid.Empty))
                latestVersionTask = _scheduleRepository.GetPlannedTrainingById(Planner.TrainingId, _cls.Token);
            else if (Planner.DefaultId is not null && !Planner.DefaultId.Equals(Guid.Empty))
                latestVersionTask = _scheduleRepository.GetPlannedTrainingForDefaultDate(Planner.DateStart, Planner.DefaultId, _cls.Token);
            if (Planner.TrainingId != null)
                _linkVehicleTraining = await _vehicleRepository.GetForTrainingAsync(Planner.TrainingId ?? throw new ArgumentNullException("Planner.TrainingId"));
            else if (Planner.DefaultId is not null)
                _linkVehicleTraining = await _vehicleRepository.GetForDefaultAsync(Planner.DefaultId ?? throw new ArgumentNullException("Planner.DefaultId"));
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
                user = authState?.User;
                _authEditOtherUser = user?.IsInRole(AccessesNames.AUTH_scheduler_other_user) ?? false;
                if (!_authEditOtherUser)
                {
                    var dbUser = await _userRepository.GetCurrentUserAsync();
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

            if (Planner.DateEnd >= DateTime.UtcNow.AddDays(AccessesSettings.AUTH_scheduler_edit_past_days))
            {
                _canEdit = true;
            }
            else if (user is not null)
            {
                _showPadlock = true;
                _canEdit = user.IsInRole(AccessesNames.AUTH_scheduler_edit_past);
            }

            if (!_canEdit)
            {
                _showPadlock = true;
            }

            _isLoading = false;
            MudDialog.StateHasChanged();
            StateHasChanged();
        }
    }

    private async Task ClickLeader(PlanUser user)
    {
        if (!_canEdit) return;
        if (user.PlannedFunctionId.Equals(DefaultSettingsHelper.KompasLeiderId))
            await FunctionSelectionChanged(user, user.UserFunctionId);
        else
            await FunctionSelectionChanged(user, DefaultSettingsHelper.KompasLeiderId);
    }

    private async Task CheckChanged(bool toggled, PlanUser user, Guid functionId)
    {
        if (!_canEdit) return;
        user.Assigned = toggled;
        user.VehicleId = _vehicleInfoForThisTraining?.FirstOrDefault(x => x.IsDefault)?.Id ?? _vehicleInfoForThisTraining?.FirstOrDefault()?.Id;
        if (toggled)
            user.PlannedFunctionId = functionId;
        else
            user.PlannedFunctionId = user.UserFunctionId;
        var result = await _scheduleRepository.PatchAssignedUser(Planner.TrainingId, Planner, user, AuditReason.Assigned);
        if (Planner.TrainingId is null || Planner.TrainingId.Equals(Guid.Empty))
            Planner.TrainingId = result.IdPatched;
        MainLayout.ShowSnackbarAssignmentChanged(user, Planner);
        await Refresh.CallRequestRefreshAsync();
    }

    private async Task CheckChanged(bool toggled, DrogeUser user, Guid functionId)
    {
        if (!_canEdit) return;
        //Add to schedule with a new status to indicate it was not set by the user.
        var result = await _scheduleRepository.PutAssignedUser(toggled, Planner.TrainingId, functionId, user, Planner);
        if (Planner.TrainingId is null || Planner.TrainingId.Equals(Guid.Empty))
            Planner.TrainingId = result.IdPut;
        var planuser = Planner.PlanUsers.FirstOrDefault(x => x.UserId == user.Id);
        if (planuser is null)
        {
            planuser = new PlanUser
            {
                UserId = user.Id,
                TrainingId = Planner.TrainingId,
                UserFunctionId = user.UserFunctionId,
                PlannedFunctionId = functionId,
                Availability = Availability.None,
                Assigned = toggled,
                Name = user.Name,
                VehicleId = _vehicleInfoForThisTraining?.FirstOrDefault(x => x.IsDefault)?.Id ?? _vehicleInfoForThisTraining?.FirstOrDefault()?.Id,
            };
            Planner.PlanUsers.Add(planuser);
        }
        else
        {
            planuser.Assigned = toggled;
            if (toggled)
                planuser.PlannedFunctionId = functionId;
            else
                planuser.PlannedFunctionId = planuser.UserFunctionId;

        }
        MainLayout.ShowSnackbarAssignmentChanged(planuser!, Planner);
        await Refresh.CallRequestRefreshAsync();
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
        await _scheduleRepository.PatchAssignedUser(Planner.TrainingId, null, user, AuditReason.ChangeVehicle);
        await Refresh.CallRequestRefreshAsync();
    }

    private async Task FunctionSelectionChanged(PlanUser user, Guid? id)
    {
        if (!_canEdit) return;
        user.ClickedFunction = false;
        user.PlannedFunctionId = id;
        await _scheduleRepository.PatchAssignedUser(Planner.TrainingId, null, user, AuditReason.ChangedFunction);
        await Refresh.CallRequestRefreshAsync();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
