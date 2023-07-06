using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class TrainingTypesRepository
{
    private readonly ITrainingTypesClient _trainingTypesClient;
    private readonly IOfflineService _offlineService;

    public TrainingTypesRepository(ITrainingTypesClient trainingTypesClient, IOfflineService offlineService)
    {
        _trainingTypesClient = trainingTypesClient;
        _offlineService = offlineService;
    }

    internal async Task<List<PlannerTrainingType>?> GetTrainingTypes(CancellationToken clt = default)
    {
        var schedule = await _trainingTypesClient.GetTrainingTypesAsync(clt);
        return schedule.PlannerTrainingTypes?.ToList();
    }
    internal async Task<PutTrainingTypeResponse> Post(PlannerTrainingType trainingType, CancellationToken clt = default)
    {
        var result = await _trainingTypesClient.PostNewTrainingTypeAsync(trainingType, clt);
        return result;
    }

    internal async Task<PatchTrainingTypeResponse> Patch(PlannerTrainingType trainingType, CancellationToken clt = default)
    {
        var result = await _trainingTypesClient.PatchTrainingTypeAsync(trainingType, clt);
        return result;
    }
}
