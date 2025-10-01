using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components.Dialogs;

public partial class AddTrainingTargetDialog : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<AddGlobalUserDialog>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [CascadingParameter, NotNull] IMudDialogInstance? MudDialog { get; set; }
    [Parameter] public TrainingTarget? TrainingTarget { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public bool? IsNew { get; set; }
    [Parameter] public Guid? SubjectId { get; set; }
    
    [AllowNull] private MudForm _form;
    private bool _success;
    private string[] _errors = [];
    private readonly CancellationTokenSource _cls = new();
    void Cancel() => MudDialog.Cancel();
    
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && IsNew == true && TrainingTarget is null && SubjectId is not null)
        {
            TrainingTarget = new TrainingTarget
            {
                SubjectId = SubjectId.Value,
            };
            StateHasChanged();
        }
    }
    
    private async Task Submit()
    {

        MudDialog.Close();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}