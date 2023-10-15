using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout.Components;

public sealed partial class AddDayItemDialog
{
    [Inject] private IStringLocalizer<AddDayItemDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!; 
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;

    void Cancel() => MudDialog.Cancel();
    void OnSubmit() => MudDialog.Close(DialogResult.Ok(true));
}
