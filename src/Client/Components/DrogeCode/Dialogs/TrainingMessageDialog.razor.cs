using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode.Dialogs;

public sealed partial class TrainingMessageDialog : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<TrainingMessageDialog>? L { get; set; }
    [Inject, NotNull] private IScheduleClient? ScheduleClient { get; set; }
    [Inject, NotNull] private TrainingTargetRepository? TrainingTargetRepository { get; set; }
    [Inject, NotNull] private IRatingService? RatingService { get; set; }
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter, EditorRequired] public TrainingAdvance Training { get; set; } = default!;
    [Parameter] public bool Visible { get; set; }
    private CancellationTokenSource _cls = new();
    private GetTargetSetWithTargetsResult? _targetSetWithTargetsResult;
    private GetDescriptionByTrainingIdResponse? _description;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Training.TrainingId is not null)
        {
            _description = await ScheduleClient.GetDescriptionByTrainingIdAsync(Training.TrainingId.Value, _cls.Token);
            _targetSetWithTargetsResult = await TrainingTargetRepository.GetSetWithTargetsLinkedToTraining(Training.TrainingId.Value, _cls.Token);
            StateHasChanged();
        }
    }

    private async Task UpdateResult(bool b, TrainingTargetResult resultObject)
    {
        if (resultObject.IsUpdating || _targetSetWithTargetsResult?.TrainingTargets is null) return;
        resultObject.IsUpdating = true;
        StateHasChanged();

        resultObject.Result = b ? 5 : 0;
        var isNew = resultObject.Id is null;
        await RatingService.UpdateResult(b ? 5 : 0, true, resultObject, _targetSetWithTargetsResult.TrainingTargets);
        if (isNew)
        {
            _targetSetWithTargetsResult.TrainingTargetResults ??= [];
            _targetSetWithTargetsResult.TrainingTargetResults.Add(resultObject);
        }

        resultObject.IsUpdating = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}