using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout.Components;

public sealed partial class AddTrainingDialog : IDisposable
{
    [Inject] private IStringLocalizer<AddTrainingDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public DrogeCodeGlobal Global { get; set; } = default!;

    private CancellationTokenSource _cls = new();
    private DrogeUser _user = new();
    private NewTraining _training = new();
    private bool _success;
    private string[] _errors = { };
    private MudForm _form;

    private async Task OnSubmit()
    {
        if (!_form.IsValid) return;
       var newId = await _scheduleRepository.AddTraining(_training, _cls.Token);
        await Global.CallNewTrainingAddedAsync(_training, newId);
        MudDialog.Close(DialogResult.Ok(true));
    }

    void Cancel() => MudDialog.Cancel();

    public void Dispose()
    {
        _cls.Cancel();
    }
}
