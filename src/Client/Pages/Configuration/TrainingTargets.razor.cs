using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components.Dialogs;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public partial class TrainingTargets : ComponentBase
{
    [Inject, NotNull] private IStringLocalizer<TrainingTargets>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private IDialogService? DialogProvider { get; set; }
    private readonly RefreshModel _refreshModel = new();

    private Task OpenTrainingTargetDialog(TrainingTarget? trainingTarget, Guid subjectId, bool isNew)
    {
        var header = isNew ? L["Add new training target"] : L["Edit training target"];
        var parameters = new DialogParameters<AddTrainingTargetDialog>
        {
            { x => x.TrainingTarget, trainingTarget },
            { x => x.Refresh, _refreshModel },
            { x => x.IsNew, isNew },
            { x => x.SubjectId, subjectId }
        };
        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        return DialogProvider.ShowAsync<AddTrainingTargetDialog>(header, parameters, options);
    }

    private Task OpenTrainingSubjectDialog(TrainingSubject? trainingSubject, Guid? subjectId, bool isNew)
    {
        return Task.CompletedTask;
    }
}