using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode.Dialogs;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public sealed partial class ReadMoreChip
{
    [Inject] private IStringLocalizer<ReadMoreChip> L { get; set; } = default!;
    [Inject] private IDialogService DialogProvider { get; set; } = default!;
    [Parameter, EditorRequired, NotNull] public TrainingAdvance? Planner { get; set; }
    

    private Task OpenMessageDialog()
    {
        var parameters = new DialogParameters<TrainingMessageDialog>
        {
            { x => x.Planner, Planner },
        };
        var options = new DialogOptions()
        {
            MaxWidth = MudBlazor.MaxWidth.Large,
            CloseButton = true,
            FullWidth = true
        };
        return DialogProvider.ShowAsync<TrainingMessageDialog>(L["Training message"], parameters, options);
    }
}