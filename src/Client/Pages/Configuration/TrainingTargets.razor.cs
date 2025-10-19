using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components.Dialogs;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class TrainingTargets : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<TrainingTargets>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private IDialogService? DialogProvider { get; set; }
    
    private readonly RefreshModel _refreshModel = new();
    private readonly CancellationTokenSource _cls = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _refreshModel.RefreshRequested += RefreshMe;
        }
    }
    
    private Task OpenTrainingTargetDialog(TrainingTarget? trainingTarget, Guid? subjectId, bool isNew)
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

    private Task OpenTrainingSubjectDialog(TrainingSubject? trainingSubject, Guid? parentId, bool isNew)
    {
        var header = isNew ? L["Add new training subject"] : L["Edit training subject"];
        var parameters = new DialogParameters<AddTrainingSubjectDialog>
        {
            { x => x.TrainingSubject, trainingSubject },
            { x => x.Refresh, _refreshModel },
            { x => x.IsNew, isNew },
            { x => x.ParentId, parentId }
        };
        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        return DialogProvider.ShowAsync<AddTrainingSubjectDialog>(header, parameters, options);
    }

    private void RefreshMe()
    {
        // Refresh done in TrainingSubjectList
    }

    public void Dispose()
    {
        _cls.Cancel();
        _refreshModel.RefreshRequested -= RefreshMe;
    }
}