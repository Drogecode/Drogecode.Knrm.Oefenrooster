using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components.Dialogs;

public sealed partial class AddTrainingSubjectDialog : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private ITrainingTargetClient? TrainingTargetClient { get; set; }
    [CascadingParameter, NotNull] IMudDialogInstance? MudDialog { get; set; }
    [Parameter] public TrainingSubject? TrainingSubject { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public bool? IsNew { get; set; }
    [Parameter] public Guid? ParentId { get; set; }

    [AllowNull] private MudForm _form;
    private bool _success;
    private string[] _errors = [];
    private readonly CancellationTokenSource _cls = new();
    void Cancel() => MudDialog.Cancel();

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && IsNew == true && TrainingSubject is null)
        {
            TrainingSubject = new TrainingSubject
            {
                ParentId = ParentId,
            };
            StateHasChanged();
        }
    }

    private async Task Submit()
    {
        if (TrainingSubject is null) return;
        bool result;
        if (IsNew == true)
        {
            result = (await TrainingTargetClient.PutNewSubjectAsync(TrainingSubject, _cls.Token)).Success;
        }
        else
        {
            result = (await TrainingTargetClient.PatchSubjectAsync(TrainingSubject, _cls.Token)).Success;
        }

        if (result)
        {
            if (Refresh is not null)
            {
                await Refresh.CallRequestRefreshAsync();
            }
            MudDialog.Close();
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}