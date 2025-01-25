using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class AuthenticationRepository
{
    private readonly IAuthenticationClient _authenticationClient;
    private readonly IOfflineService _offlineService;

    public AuthenticationRepository(IAuthenticationClient authenticationClient, IOfflineService offlineService)
    {
        _authenticationClient = authenticationClient;
        _offlineService = offlineService;
    }

    public async Task<bool> GetAuthenticateDirectEnabled(CancellationToken clt = default)
    {
        var result = await _offlineService.CachedRequestAsync("auth_dir",
            async () => await _authenticationClient.GetAuthenticateDirectEnabledAsync(clt),
            new ApiCachedRequest{OneCallPerSession = true},
            clt);
        return result;
    }
}