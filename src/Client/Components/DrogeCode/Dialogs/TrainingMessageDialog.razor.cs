using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode.Dialogs;

public sealed partial class TrainingMessageDialog
{
    [Inject, NotNull] private IStringLocalizer<TrainingMessageDialog>? L { get; set; }
    [Inject, NotNull] private IScheduleClient? ScheduleClient { get; set; }
    [Inject, NotNull] private TrainingTargetRepository? TrainingTargetRepository { get; set; }
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter, EditorRequired] public TrainingAdvance Training { get; set; } = default!;
    [Parameter] public bool Visible { get; set; }

    private GetDescriptionByTrainingIdResponse? _description;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Training.TrainingId is not null)
        {
            _description = await ScheduleClient.GetDescriptionByTrainingIdAsync(Training.TrainingId.Value);
            StateHasChanged();
        }
    }
}