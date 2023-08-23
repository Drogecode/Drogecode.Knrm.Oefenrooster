using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.Extensions.Localization;
using MudBlazor;
using static MudBlazor.Colors;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    private CancellationTokenSource _cls = new();
    private List<DrogeLinkVehicleTraining>? _linkVehicleTraining;
    private List<DrogeVehicle>? _vehicleInfoForThisTraining;
    private int _vehicleCount;
    private int _colmn1 = 1;
    private int _colmn2 = 3;
    private int _colmn3 = 3;
    private int _colmn4 = 3;
    private int _colmn5 = 2;

    void Submit() => MudDialog.Close(DialogResult.Ok(true));
    void Cancel() => MudDialog.Cancel();

    protected override async Task OnParametersSetAsync()
    {
        if (Planner.TrainingId != null)
            _linkVehicleTraining = await _vehicleRepository.GetForTrainingAsync(Planner.TrainingId ?? throw new ArgumentNullException("Planner.TrainingId"));
        if (Vehicles != null && _linkVehicleTraining != null)
        {
            _vehicleInfoForThisTraining = new();
            var count = 0;
            foreach (var vehicle in Vehicles.Where(x => x.Active))
            {
                bool? isSelected = _linkVehicleTraining?.FirstOrDefault(x => x.VehicleId == vehicle.Id)?.IsSelected;
                if (isSelected == true || (isSelected == null && vehicle.Default))
                {
                    _vehicleInfoForThisTraining.Add(vehicle);
                    count++;
                }
            }
            _vehicleCount = count;
        }
    }

    private async Task CheckChanged(bool toggled, PlanUser user, Guid functionId)
    {
        user.Assigned = toggled;
        if (toggled)
            user.PlannedFunctionId = functionId;
        else
            user.PlannedFunctionId = user.UserFunctionId;
        await _scheduleRepository.PatchAssignedUser(Planner.TrainingId, Planner, user, _cls.Token);
        await Refresh.CallRequestRefreshAsync();
    }

    private async Task CheckChanged(bool toggled, DrogeUser user, Guid functionId)
    {
        //Add to schedule with a new status to indicate it was not set by the user.
        await _scheduleRepository.PutAssignedUser(toggled, Planner.TrainingId, functionId, user, Planner, _cls.Token);
        var planuser = Planner.PlanUsers.FirstOrDefault(x => x.UserId == user.Id);
        if (planuser == null)
        {
            Planner.PlanUsers.Add(new PlanUser
            {
                UserId = user.Id,
                UserFunctionId = user.UserFunctionId,
                PlannedFunctionId = functionId,
                Availabilty = Availabilty.None,
                Assigned = toggled,
                Name = user.Name,
            });
        }
        else
        {
            planuser.Assigned = toggled;
            if (toggled)
                planuser.PlannedFunctionId = functionId;
            else
                planuser.PlannedFunctionId = planuser.UserFunctionId;

        }
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
        await _scheduleRepository.PatchAssignedUser(Planner.TrainingId, null, user, _cls.Token);
        await Refresh.CallRequestRefreshAsync();
    }

    private async Task FunctionSelectionChanged(PlanUser user, Guid? id)
    {
        user.ClickedFunction = false;
        user.PlannedFunctionId = id;
        await _scheduleRepository.PatchAssignedUser(Planner.TrainingId, null, user, _cls.Token);
        await Refresh.CallRequestRefreshAsync();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
