namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IFunctionService
{
    Task<List<DrogeFunction>> GetAllFunctions(Guid customerId);
}
