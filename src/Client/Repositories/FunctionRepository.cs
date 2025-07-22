using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class FunctionRepository
{
    private readonly IFunctionClient _functionClient;
    private readonly IOfflineService _offlineService;

    public FunctionRepository(IFunctionClient functionClient, IOfflineService offlineService)
    {
        _functionClient = functionClient;
        _offlineService = offlineService;
    }

    public async Task<List<DrogeFunction>?> GetAllFunctionsAsync(bool cachedAndReplace, CancellationToken clt)
    {
        var response = await _offlineService.CachedRequestAsync(string.Format("all_func"),
            async () => await _functionClient.GetAllAsync(cachedAndReplace, clt),
            new ApiCachedRequest
                { OneCallPerSession = true, CachedAndReplace = cachedAndReplace, ExpireSession = DateTime.UtcNow.AddMinutes(30) },
            clt: clt);
        return response?.Functions?.ToList();
    }
}