using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IFunctionService
{
    Task<DrogeFunction?> GetById(Guid customerId, Guid functionId, CancellationToken clt);
    Task<AddFunctionResponse> AddFunction(DrogeFunction function, Guid customerId, CancellationToken clt);
    Task<PatchResponse> PatchFunction(Guid customerId, DrogeFunction function, CancellationToken clt);
    Task<MultipleFunctionsResponse> GetAllFunctions(Guid customerId, CancellationToken clt);
}
