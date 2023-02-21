using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class EditTrainingDialog : IDisposable
{
    [Inject] private IStringLocalizer<EditTrainingDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public PlannedTraining Planner { get; set; } = default!;
    [Parameter] public RefreshModel Refresh { get; set; } = default!;
    [Parameter] public List<DrogeVehicle>? Vehicles { get; set; }
    private CancellationTokenSource _cls = new();

    void Cancel() => MudDialog.Cancel();
    private async Task Submit() {

        MudDialog.Close(DialogResult.Ok(true)); 
    }

    private async Task CheckChanged(bool toggled, DrogeVehicle vehicle)
    {

    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
