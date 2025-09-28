using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ITrainingTargetService : IDrogeService
{
    Task<AllTrainingTargetSubjectsResponse> AllTrainingTargets(int count, int skip, Guid? subjectId, Guid customerId, CancellationToken clt);
    Task<AllTrainingTargetsResponse> GetTargetsLinkedToTraining(Guid trainingId, Guid customerId, CancellationToken clt);
    Task<GetSingleTargetSetResponse> GetSetLinkedToTraining(Guid trainingId, Guid customerId, CancellationToken clt);
    Task<GetTargetSetWithTargetsResult> GetSetWithTargetsLinkedToTraining(Guid trainingId, Guid customerId, Guid userId, CancellationToken clt);
    Task<GetAllTargetSetResponse> GetAllReusableSets(Guid customerId, int count, int skip, CancellationToken clt);
    Task<PutResponse> PutNewTemplateSet(TrainingTargetSet body, Guid userId, Guid customerId, CancellationToken clt);
    Task<PatchResponse> PatchTemplateSet(TrainingTargetSet body, Guid userId, Guid customerId, CancellationToken clt);
    Task<PutResponse> PutUserResponse(TrainingTargetResult body, Guid userId, Guid customerId, CancellationToken clt);
    Task<PatchResponse> PatchUserResponse(TrainingTargetResult body, Guid userId, Guid customerId, CancellationToken clt);
    Task<PatchResponse> PatchUserResponseForTraining(Guid trainingId, TrainingTargetType forType, int result, Guid userId, Guid customerId, CancellationToken clt);
    Task<AllResultForUserResponse> GetAllResultForUser(Guid userIdResult, Guid userId, RatingPeriod period, Guid customerId, CancellationToken clt);
}