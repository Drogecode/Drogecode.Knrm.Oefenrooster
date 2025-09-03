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

    public async Task<List<TrainingSubject>?> AllTrainingTargets(int count, int skip, CancellationToken clt)
    {
        var targets = await _offlineService.CachedRequestAsync($"traTarg_{count}_{skip}",
            async () => await _trainingTargetClient.AllTrainingTargetsAsync(count, skip, clt),
            clt: clt);
        return targets?.TrainingSubjects;
    }
    
    public async Task<List<TrainingSubject>?> AllTrainingTargets(int count, int skip, Guid subjectId, CancellationToken clt)
    {
        var targets = await _offlineService.CachedRequestAsync($"traTarg_{count}_{skip}_{subjectId}",
            async () => await _trainingTargetClient.AllTrainingTargets2Async(count, skip, subjectId, clt),
            clt: clt);
        return targets?.TrainingSubjects;
    }

    public async Task<TrainingTargetSet?> GetSetLinkedToTraining(Guid trainingId, CancellationToken clt)
    {
        var response = await _trainingTargetClient.GetSetLinkedToTrainingAsync(trainingId, clt);
        return response?.TrainingTargetSet;
    }

    public async Task<List<TrainingTarget>?> GetTargetsLinkedToTraining(Guid trainingId, CancellationToken clt)
    {
        var response = await _trainingTargetClient.GetTargetsLinkedToTrainingAsync(trainingId, clt);
        return response?.TrainingTargets;
    }
    
    public async Task<PutResponse?> PutNewTemplateSet(TrainingTargetSet body, CancellationToken clt)
    {
        var response = await _trainingTargetClient.PutNewTemplateSetAsync(body, clt);
        return response;
    }
    
    public async Task<PatchResponse?> PatchTemplateSet(TrainingTargetSet body, CancellationToken clt)
    {
        var response = await _trainingTargetClient.PatchTemplateSetAsync(body, clt);
        return response;
    }
    
    public async Task<PutResponse?> PutUserResponse(TrainingTargetResult body, CancellationToken clt)
    {
        var response = await _trainingTargetClient.PutUserResponseAsync(body, clt);
        return response;
    }
    
    public async Task<PatchResponse?> PatchUserResponse(TrainingTargetResult body, CancellationToken clt)
    {
        var response = await _trainingTargetClient.PatchUserResponseAsync(body, clt);
        return response;
    }
}