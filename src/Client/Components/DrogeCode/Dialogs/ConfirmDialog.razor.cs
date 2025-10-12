using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode.Dialogs;

public partial class ConfirmDialog : ComponentBase
{
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public bool Value { get; set; }
    [Parameter] public string? Body { get; set; }
    
    void Cancel() => MudDialog.Cancel();

    private async Task Submit()
    {
        MudDialog.Close(DialogResult.Ok(Value));
    }
}