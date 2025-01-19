using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class TrainingMessageDialog
{
    [Inject] private IStringLocalizer<TrainingMessageDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IScheduleClient ScheduleClient { get; set; } = default!;
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    
    [Parameter, EditorRequired] public PlannedTraining Planner { get; set; } = default!;
    
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