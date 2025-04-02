using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserGlobal;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;

public partial class AddGlobalUserDialog : ComponentBase
{
    [Inject, NotNull] private IStringLocalizer<AddGlobalUserDialog>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public DrogeUserGlobal? GlobalUser { get; set; }
    [Parameter] public bool? IsNew { get; set; }
    
    void Cancel() => MudDialog.Cancel();


    private async Task Submit()
    {
        MudDialog.Close();
    }
}