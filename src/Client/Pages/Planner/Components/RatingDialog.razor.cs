using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public partial class RatingDialog : ComponentBase, IDisposable
{
    [Inject, NotNull] private IStringLocalizer<RatingDialog>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private TrainingTargetRepository? TrainingTargetRepository { get; set; }
    [Inject, NotNull] private IRatingService? RatingService { get; set; }
    [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter] public PlannedTraining? Planner { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public DrogeCodeGlobal? Global { get; set; }
    [Parameter] public List<DrogeUser>? Users { get; set; }
    private CancellationTokenSource _cls = new();
    private List<TrainingTarget>? _trainingTargets;
    private bool _updatingAll;
    private int _allHovered;

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

    private async Task UpdateResult(bool value, TrainingTargetResult resultObject)
    {
        await UpdateResult(value ? 5 : 0, resultObject);
    }

    private async Task UpdateResult(int i, TrainingTargetResult resultObject)
    {
        if (_updatingAll || resultObject.IsUpdating || _trainingTargets is null) return;
        resultObject.IsUpdating = true;
        StateHasChanged();
        resultObject.Result = i;

        await RatingService.UpdateResult(i, false, resultObject, _trainingTargets);

        resultObject.IsUpdating = false;
        StateHasChanged();
    }

    private async Task AllSameClicked(int i)
    {
        if (_updatingAll) return;
        _updatingAll = true;
        StateHasChanged();

        await UpdateTrainingTargetsAsync(i, TrainingTargetType.Exercise);

        _updatingAll = false;
        StateHasChanged();
    }

    private async Task AllSameClicked(bool e)
    {
        if (_updatingAll) return;
        _updatingAll = true;
        StateHasChanged();

        await UpdateTrainingTargetsAsync(e ? 5 : 0, TrainingTargetType.Knowledge);

        _updatingAll = false;
        StateHasChanged();
    }

    private async Task UpdateTrainingTargetsAsync(int i, TrainingTargetType forType)
    {
        var patchResult = await TrainingTargetRepository.PatchUserResponseForTrainingAsync(Planner!.TrainingId!.Value, forType, i, _cls.Token);
        if (patchResult?.Success == true)
        {
            _trainingTargets = null;
            _trainingTargets = await TrainingTargetRepository.GetTargetsLinkedToTraining(Planner.TrainingId.Value, _cls.Token);
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}