using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class TrainingHistoryDialog
{
    [Inject] private IStringLocalizer<TrainingHistoryDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IAuditClient AuditClient { get; set; } = default!;
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public Guid? TrainingId { get; set; }
    [Parameter] public List<DrogeUser>? Users { get; set; }

    private List<TrainingAudit>? _trainingAudits = null;
    void Submit() => MudDialog.Close(DialogResult.Ok(true));
    void Cancel() => MudDialog.Cancel();
    protected override async Task OnParametersSetAsync()
    {
        if (TrainingId is not null)
            _trainingAudits = (await AuditClient.GetTrainingAuditAsync(TrainingId.Value, 50, 0)).TrainingAudits;
    }
}
