using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode.Dialogs;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public sealed partial class ReadReportChip
{
    [Inject, NotNull] private IStringLocalizer<ReadMoreChip>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private IDialogService? DialogProvider { get; set; }
    [Parameter, EditorRequired, NotNull] public PlannedTraining? Training { get; set; }
    

    private Task OpenReportsDialog()
    {
        var parameters = new DialogParameters<TrainingReportsDialog>
        {
            { x => x.Training, Training },
        };
        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Large,
            CloseButton = true,
            FullWidth = true
        };
        return DialogProvider.ShowAsync<TrainingReportsDialog>(L["Training message"], parameters, options);
    }
}