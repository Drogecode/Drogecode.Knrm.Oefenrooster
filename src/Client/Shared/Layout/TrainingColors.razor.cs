using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;

public sealed partial class TrainingColors
{
    [Inject, NotNull] private TrainingTypesRepository? TrainingTypesRepository { get; set; }
    [Parameter] public bool IsDarkMode { get; set; }
    private List<PlannerTrainingType>? _trainingTypes;

    protected override async Task OnInitializedAsync()
    {
        _trainingTypes ??= await TrainingTypesRepository.GetTrainingTypes(false, false);
    }
}