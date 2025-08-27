using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;
using Microsoft.AspNetCore.Mvc;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ITrainingTargetService : IDrogeService
{
    Task<AllTrainingTargetsResponse> AllTrainingTargets(int count, int skip, Guid? subjectId, Guid customerId, CancellationToken clt);
    Task<GetSingleTargetSetResponse> GetSetLinkedToTraining(Guid trainingId, Guid customerId, CancellationToken clt);
    Task<ActionResult<GetAllTargetSetResponse>> GetAllReusableSets(Guid customerId, int count, int skip, CancellationToken clt);
}