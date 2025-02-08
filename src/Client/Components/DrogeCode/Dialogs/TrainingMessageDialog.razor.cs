using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode.Dialogs;

public sealed partial class TrainingMessageDialog
{
    [Inject] private IStringLocalizer<TrainingMessageDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IScheduleClient ScheduleClient { get; set; } = default!;
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    
    [Parameter, EditorRequired] public TrainingAdvance Planner { get; set; } = default!;
    
    private GetDescriptionByTrainingIdResponse? _description;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Planner.TrainingId is not null)
        {
            _description = await ScheduleClient.GetDescriptionByTrainingIdAsync(Planner.TrainingId.Value);
            StateHasChanged();
        }
    }
}