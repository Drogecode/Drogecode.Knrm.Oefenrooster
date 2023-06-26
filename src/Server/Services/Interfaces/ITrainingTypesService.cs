using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Microsoft.AspNetCore.Mvc;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ITrainingTypesService
{
    Task<PutTrainingTypeResponse> PostTrainingType(Guid userId, Guid customerId, PlannerTrainingType plannerTrainingType, CancellationToken clt);
    Task<MultiplePlannerTrainingTypesResponse> GetTrainingTypes(Guid customerId, CancellationToken token);
    Task<GetTraininTypeByIdResponse> GetById(Guid id, Guid customerId, CancellationToken clt);
    Task<PatchTrainingTypeResponse> PatchTrainingType(Guid userId, Guid customerId, PlannerTrainingType plannerTrainingType, CancellationToken clt);
}
