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
}