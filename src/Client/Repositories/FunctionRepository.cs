using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class FunctionRepository
{
    private readonly IFunctionClient _functionClient;

    public FunctionRepository(IFunctionClient functionClient)
    {
        _functionClient = functionClient;
    }

    public async Task<List<DrogeFunction>?> GetAllFunctionsAsync()
    {
        var dbUser = await _functionClient.GetAllAsync();
        return dbUser.Functions?.ToList();
    }
}
