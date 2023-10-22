using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class ScheduleDialog : IDisposable
{
    [Inject] private IStringLocalizer<ScheduleDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private VehicleRepository _vehicleRepository { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public PlannedTraining Planner { get; set; } = default!;
    [Parameter] public List<DrogeUser>? Users { get; set; }
    [Parameter] public List<DrogeFunction>? Functions { get; set; }
    [Parameter] public List<DrogeVehicle>? Vehicles { get; set; }
    [Parameter] public RefreshModel Refresh { get; set; } = default!;
    [Parameter] public MainLayout MainLayout { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<DrogeLinkVehicleTraining>? _linkVehicleTraining;
    private List<DrogeVehicle>? _vehicleInfoForThisTraining;
    private bool _plannerIsUpdated;
    private bool _showWoeps;
    private int _vehicleCount;
    private int _colmn1 = 2;
    private int _colmn2 = 3;
    private int _colmn3 = 3;
    private int _colmn4 = 3;
    private int _colmn5 = 1;

    void Submit() => MudDialog.Close(DialogResult.Ok(true));
    void Cancel() => MudDialog.Cancel();

    protected override async Task OnParametersSetAsync()
    {
        Task<GetPlannedTrainingResponse?>? latestVersionTask = null;
        if (Planner.TrainingId is not null && !Planner.TrainingId.Equals(Guid.Empty))
            latestVersionTask = _scheduleRepository.GetPlannedTrainingById(Planner.TrainingId, _cls.Token);
        else if (Planner.DefaultId is not null && !Planner.DefaultId.Equals(Guid.Empty))
            latestVersionTask = _scheduleRepository.GetPlannedTrainingForDefaultDate(Planner.DateStart, Planner.DefaultId, _cls.Token);
        if (Planner.TrainingId != null)
            _linkVehicleTraining = await _vehicleRepository.GetForTrainingAsync(Planner.TrainingId ?? throw new ArgumentNullException("Planner.TrainingId"));
        if (Vehicles != null && _linkVehicleTraining != null)
        {
            _vehicleInfoForThisTraining = new();
            var count = 0;
            foreach (var vehicle in Vehicles.Where(x => x.IsActive))
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
        if (latestVersionTask is not null)
            latestVersion = (await latestVersionTask)?.Training;
        if (latestVersion is not null)
        {
            Planner.PlanUsers = latestVersion.PlanUsers;
            _plannerIsUpdated = true;
        }
        else
            _showWoeps = true;
    }

    private async Task ClickLeader(PlanUser user)
    {
        if (user.PlannedFunctionId.Equals(DefaultSettingsHelper.KompasLeiderId))
            await FunctionSelectionChanged(user, user.UserFunctionId);
        else
            await FunctionSelectionChanged(user, DefaultSettingsHelper.KompasLeiderId);
    }

    private async Task CheckChanged(bool toggled, PlanUser user, Guid functionId)
    {
        user.Assigned = toggled;
        if (toggled)
            user.PlannedFunctionId = functionId;
        else
            user.PlannedFunctionId = user.UserFunctionId;
        var result = await _scheduleRepository.PatchAssignedUser(Planner.TrainingId, Planner, user);
        if (Planner.TrainingId is null || Planner.TrainingId.Equals(Guid.Empty))
            Planner.TrainingId = result.IdPatched;
        MainLayout.ShowSnackbarAssignmentChanged(user, Planner);
        await Refresh.CallRequestRefreshAsync();
    }

    private async Task CheckChanged(bool toggled, DrogeUser user, Guid functionId)
    {
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
                Availabilty = Availabilty.None,
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

    private Color GetColor(Availabilty? availabilty)
    {
        switch (availabilty)
        {
            case Availabilty.Available:
                return Color.Success;
            case Availabilty.NotAvailable:
                return Color.Error;
            case Availabilty.Maybe:
                return Color.Warning;
            default: return Color.Inherit;
        }
    }

    private async Task VehicleSelectionChanged(PlanUser user, Guid? id)
    {
        user.VehicleId = id;
        await _scheduleRepository.PatchAssignedUser(Planner.TrainingId, null, user);
        await Refresh.CallRequestRefreshAsync();
    }

    private async Task FunctionSelectionChanged(PlanUser user, Guid? id)
    {
        user.ClickedFunction = false;
        user.PlannedFunctionId = id;
        await _scheduleRepository.PatchAssignedUser(Planner.TrainingId, null, user);
        await Refresh.CallRequestRefreshAsync();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
