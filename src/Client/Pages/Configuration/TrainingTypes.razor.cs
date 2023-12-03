using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.Client.Pages.User;
using Drogecode.Knrm.Oefenrooster.Client.Pages.User.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class TrainingTypes : IDisposable
{
    [Inject] private IStringLocalizer<TrainingTypes> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    [Inject] private TrainingTypesRepository _trainingTypesRepository { get; set; } = default!;
    private List<PlannerTrainingType>? _trainingTypes;
    private CancellationTokenSource _cls = new();
    private RefreshModel _refreshModel = new();
    protected override async Task OnParametersSetAsync()
    {
        _trainingTypes = await _trainingTypesRepository.GetTrainingTypes(true, _cls.Token);
        _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
    }

    private void OpenTrainingTypeDialog(PlannerTrainingType? trainingType, bool isNew)
    {
        var header = isNew ? L["Add new training type"] : L["Edit training type"];
        var parameters = new DialogParameters<TrainingTypeDialog> {
            { x=> x.TrainingType, trainingType },
            { x=> x.Refresh, _refreshModel },
            { x=> x.IsNew, isNew},
        };
        DialogOptions options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        _dialogProvider.Show<TrainingTypeDialog>(header, parameters, options);
    }

    private async Task Delete(PlannerTrainingType? trainingType)
    {
    }

    private async Task RefreshMeAsync()
    {
        _trainingTypes = await _trainingTypesRepository.GetTrainingTypes(true, _cls.Token);
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
        _refreshModel.RefreshRequestedAsync -= RefreshMeAsync;
    }
}