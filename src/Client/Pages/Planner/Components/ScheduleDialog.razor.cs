using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class ScheduleDialog : IDisposable
{
    [Inject] private IStringLocalizer<ScheduleDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public PlannedTraining Planner { get; set; } = default!;
    [Parameter] public List<DrogeUser>? Users { get; set; }
    [Parameter] public List<DrogeFunction>? Functions { get; set; }
    [Parameter] public RefreshModel Refresh { get; set; } = default!;
    private CancellationTokenSource _cls = new();

    void Submit() => MudDialog.Close(DialogResult.Ok(true));
    void Cancel() => MudDialog.Cancel();

    private async Task CheckChangeddd(bool toggled, PlanUser user, Guid functionId)
    {
        user.Assigned = toggled;
        if (toggled)
            user.PlannedFunctionId = functionId;
        else
            user.PlannedFunctionId = user.UserFunctionId;
        await _scheduleRepository.PatchScheduleUserScheduled(Planner.TrainingId, user, functionId, _cls.Token);
        await Refresh.CallRequestRefreshAsync();
    }
    private async Task CheckChangeddd(bool toggled, DrogeUser user, Guid functionId)
    {
        //Add to schedule with a new status to indicate it was not set by the user.
        await _scheduleRepository.OtherScheduleUser(toggled, Planner.TrainingId, functionId, user, _cls.Token);
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

    public void Dispose()
    {
        _cls.Cancel();
    }
}
