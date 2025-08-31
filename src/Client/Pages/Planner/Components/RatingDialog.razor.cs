using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public partial class RatingDialog : ComponentBase
{
    [Inject, NotNull] private IStringLocalizer<RatingDialog>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private TrainingTargetRepository? TrainingTargetRepository { get; set; }
    [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter] public PlannedTraining? Planner { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public DrogeCodeGlobal? Global { get; set; }
    [Parameter] public List<DrogeUser>? Users { get; set; }
    private CancellationTokenSource _cls = new();
    private List<TrainingTarget>? _trainingTargets;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_target_user_rate))
            {
                if (Planner?.TrainingId is not null)
                {
                    _trainingTargets = await TrainingTargetRepository.GetTargetsLinkedToTraining(Planner.TrainingId.Value, _cls.Token);
                }
            }
            StateHasChanged();
            MudDialog?.StateHasChanged();
        }
    }

    private async Task UpdateResult(int i, TrainingTargetResult resultObject)
    {
        if (resultObject.RoosterAvailableId == Guid.Empty)
        {
            Console.WriteLine("RoosterAvailableId is empty");
        }
        
        if (resultObject.Id is null)
        {
            //ToDo: Add new result
        }
        else
        {
            //ToDo: Update result
        }
    }
}