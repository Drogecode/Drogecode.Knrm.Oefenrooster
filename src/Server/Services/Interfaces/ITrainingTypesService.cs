using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ITrainingTypesService
{
    Task<MultiplePlannerTrainingTypesResponse> GetTrainingTypes(Guid customerId, CancellationToken token);
}
