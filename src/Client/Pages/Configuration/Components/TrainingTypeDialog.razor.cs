using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.User.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Microsoft.Extensions.Localization;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;

public sealed partial class TrainingTypeDialog : IDisposable
{
    [Inject] private IStringLocalizer<TrainingTypeDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private TrainingTypesRepository _trainingTypesRepository { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public PlannerTrainingType? TrainingType { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public bool IsNew { get; set; }

    private CancellationTokenSource _cls = new();
    private PlannerTrainingType? _originalTrainingType { get; set; }
    private bool _success;
    private string[] _errors = Array.Empty<string>();
    [AllowNull] private MudForm _form;
    void Cancel() => MudDialog.Cancel();
    protected override void OnParametersSet()
    {
        if (IsNew == true)
        {
            TrainingType = new PlannerTrainingType();
        }
        _originalTrainingType = (PlannerTrainingType?)TrainingType?.Clone();
    }

    private async Task Submit()
    {
        await _form.Validate();
        if (!_form.IsValid) return;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
