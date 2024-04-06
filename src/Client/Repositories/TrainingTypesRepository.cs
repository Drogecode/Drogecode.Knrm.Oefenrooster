using Drogecode.Knrm.Oefenrooster.Client.Models;
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

    internal async Task<List<PlannerTrainingType>?> GetTrainingTypes(bool forceCache, bool cachedAndReplace, CancellationToken clt = default)
    {
        var response = await _offlineService.CachedRequestAsync(string.Format("tra_tp_all"),
            async () => await _trainingTypesClient.GetTrainingTypesAsync(cachedAndReplace, clt),
            new ApiCachedRequest
                { OneCallPerSession = true, ForceCache = forceCache, CachedAndReplace = cachedAndReplace },
            clt: clt);
        return response.PlannerTrainingTypes?.ToList();
    }

    internal async Task<PutTrainingTypeResponse> Post(PlannerTrainingType trainingType, CancellationToken clt = default)
    {
        var result = await _trainingTypesClient.PostNewTrainingTypeAsync(trainingType, clt);
        return result;
    }

    internal async Task<PatchTrainingTypeResponse> Patch(PlannerTrainingType trainingType,
        CancellationToken clt = default)
    {
        var result = await _trainingTypesClient.PatchTrainingTypeAsync(trainingType, clt);
        return result;
    }
}