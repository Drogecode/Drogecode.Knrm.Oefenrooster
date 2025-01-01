using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components.Dialogs;

public sealed partial class UserTableConfigureDialog
{
    [Inject] private IStringLocalizer<StatisticsTab> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter, EditorRequired] public List<DistinctType>? DistinctTypes {get; set;}
    [Parameter, EditorRequired] public List<string>? Boats { get; set; }
    [Parameter, EditorRequired] public List<int>? Years {get; set;}
    [Parameter, EditorRequired] public IEnumerable<DistinctType?> SelectedTypes { get; set; } = [];
    [Parameter, EditorRequired] public IEnumerable<string>? SelectedBoats { get; set; }
    [Parameter, EditorRequired] public IEnumerable<int> SelectedYear { get; set; } = [];
    [Parameter, EditorRequired] public decimal Compensation { get; set; } = 1.25M;
    
    private void Submit()
    {
        var result = new StatisticsUserTableConfigureSettings
        {
            SelectedTypes = SelectedTypes,
            SelectedYear = SelectedYear,
            SelectedBoats = SelectedBoats,
            Compensation = Compensation,
        };
        MudDialog.Close(DialogResult.Ok(result));
    }
}