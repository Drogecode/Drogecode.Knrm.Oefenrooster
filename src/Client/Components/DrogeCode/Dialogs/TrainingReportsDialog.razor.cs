using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode.Dialogs;

public partial class TrainingReportsDialog : ComponentBase
{
    [Inject, NotNull] private IStringLocalizer<TrainingReportsDialog>? L { get; set; } 
    [Inject, NotNull] private IScheduleClient? ScheduleClient { get; set; }
    [CascadingParameter, NotNull] IMudDialogInstance? MudDialog { get; set; }
    
    [Parameter, EditorRequired] public TrainingAdvance? Training { get; set; }
    [Parameter] public bool Visible { get; set; }
}