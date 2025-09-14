using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
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

    public async Task<AllTrainingTargetSubjectsResponse?> AllTrainingTargets(CancellationToken clt)
    {
        var targets = await _offlineService.CachedRequestAsync($"traTarg_all",
            async () =>
            {
                var response = new AllTrainingTargetSubjectsResponse()
                {
                    TrainingSubjects = [],
                    Success = true
                };
                var count = 15;
                var skip = 0;
                while (true)
                {
                    var d = await _trainingTargetClient.AllTrainingTargetsAsync(count, skip * count, clt);
                    if (d?.TrainingSubjects is null)
                    {
                        DebugHelper.WriteLine("TrainingSubjects is null");
                        break;
                    }

                    response.TrainingSubjects.AddRange(d.TrainingSubjects);
                    response.TotalCount = d.TotalCount;
                    if (d.ElapsedMilliseconds > 0)
                    {
                        response.ElapsedMilliseconds += d.ElapsedMilliseconds;
                    }

                    if (!d.Success)
                    {
                        response.Success = false;
                    }

                    if (d.TotalCount <= response.TrainingSubjects.Count)
                    {
                        DebugHelper.WriteLine($"TotalCount <= _trainingSubjects.Count == {d.TotalCount} <= {response.TrainingSubjects.Count}");
                        break;
                    }

                    skip++;
                }

                return response;
            },
            clt: clt);
        return targets;
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

    public async Task<GetTargetSetWithTargetsResult?> GetSetWithTargetsLinkedToTraining(Guid trainingId, CancellationToken clt)
    {
        var response = await _trainingTargetClient.GetSetWithTargetsLinkedToTrainingAsync(trainingId, clt);
        return response;
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

    public async Task<PutResponse?> PutUserResponse(TrainingTargetResult body, bool fromCurrentUser, CancellationToken clt)
    {
        var response = await _trainingTargetClient.PutUserResponseAsync(fromCurrentUser, body, clt);
        return response;
    }

    public async Task<PatchResponse?> PatchUserResponse(TrainingTargetResult body, CancellationToken clt)
    {
        var response = await _trainingTargetClient.PatchUserResponseAsync(body, clt);
        return response;
    }

    public async Task<PatchResponse?> PatchUserResponseForTrainingAsync(Guid trainingId, TrainingTargetType forType, int result, CancellationToken clt)
    {
        var response = await _trainingTargetClient.PatchUserResponseForTrainingAsync(trainingId, forType, result, clt);
        return response;
    }

    public async Task<AllResultForUserResponse?> GetAllResultForUser(Guid userIdResult, RatingPeriod period, CancellationToken clt)
    {
        var response = await _trainingTargetClient.GetAllResultForUserAsync(userIdResult, period, clt);
        return response;
    }
}