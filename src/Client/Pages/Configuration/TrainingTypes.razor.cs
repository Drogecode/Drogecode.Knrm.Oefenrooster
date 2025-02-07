using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class TrainingTypes : IDisposable
{
    [Inject] private IStringLocalizer<TrainingTypes> L { get; set; } = default!;
    [Inject] private IDialogService DialogProvider { get; set; } = default!;
    [Inject] private TrainingTypesRepository TrainingTypesRepository { get; set; } = default!;
    private List<PlannerTrainingType>? _trainingTypes;
    private readonly CancellationTokenSource _cls = new();
    private readonly RefreshModel _refreshModel = new();
    protected override async Task OnParametersSetAsync()
    {
        _trainingTypes = await TrainingTypesRepository.GetTrainingTypes(true, false, _cls.Token);
        _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
    }

    private Task OpenTrainingTypeDialog(PlannerTrainingType? trainingType, bool isNew)
    {
        var header = isNew ? L["Add new training type"] : L["Edit training type"];
        var parameters = new DialogParameters<TrainingTypeDialog> {
            { x=> x.TrainingType, trainingType },
            { x=> x.Refresh, _refreshModel },
            { x=> x.IsNew, isNew},
        };
        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        return DialogProvider.ShowAsync<TrainingTypeDialog>(header, parameters, options);
    }

    private async Task RefreshMeAsync()
    {
        _trainingTypes = await TrainingTypesRepository.GetTrainingTypes(true, false, _cls.Token);
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
        _refreshModel.RefreshRequestedAsync -= RefreshMeAsync;
    }
}