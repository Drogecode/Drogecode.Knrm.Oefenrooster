using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class TrainingTargetRepository
{
    private readonly ITrainingTargetClient _trainingTargetClient;
    private readonly IOfflineService _offlineService;

    public TrainingTargetRepository(ITrainingTargetClient trainingTargetClient, IOfflineService offlineService)
    {
        _trainingTargetClient = trainingTargetClient;
        _offlineService = offlineService;
    }

    public async Task<List<TrainingTarget>?> AllTrainingTargetsAsync(int count, int skip, CancellationToken clt)
    {
        var targets = await _offlineService.CachedRequestAsync($"traTarg_{count}_{skip}",
            async () => await _trainingTargetClient.AllTrainingTargetsAsync(count, skip, clt),
            clt: clt);
        return targets?.TrainingTargets;
    }
    
    public async Task<List<TrainingTarget>?> AllTrainingTargetsAsync(int count, int skip, Guid subjectId, CancellationToken clt)
    {
        var targets = await _offlineService.CachedRequestAsync($"traTarg_{count}_{skip}_{subjectId}",
            async () => await _trainingTargetClient.AllTrainingTargets2Async(count, skip, subjectId, clt),
            clt: clt);
        return targets?.TrainingTargets;
    }
}