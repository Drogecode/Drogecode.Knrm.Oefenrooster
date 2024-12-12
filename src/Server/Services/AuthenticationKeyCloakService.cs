using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class AuthenticationKeyCloakService : IAuthenticationService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;

    public AuthenticationKeyCloakService(
        IMemoryCache memoryCache,
        IConfiguration configuration)
    {
        _memoryCache = memoryCache;
        _configuration = configuration;
    }
    
    public async Task<GetLoginSecretsResponse> GetLoginSecrets()
    {
        var codeVerifier = SecretHelper.CreateSecret(120);
        var tenantId = _configuration.GetValue<string>("KeyCloak:TenantId") ?? throw new DrogeCodeNullException("no tenant id found for KeyCloak login");
        var clientId = _configuration.GetValue<string>("KeyCloak:ClientId") ?? throw new DrogeCodeNullException("no client id found for KeyCloak login");
        var instance = _configuration.GetValue<string>("KeyCloak:Instance") ?? throw new DrogeCodeNullException("no instance found for KeyCloak login");
        var response = new CacheLoginSecrets
        {
            IdentityProvider = IdentityProvider.KeyCloak,
            LoginSecret = SecretHelper.CreateSecret(64),
            LoginNonce = SecretHelper.CreateSecret(64),
            CodeChallenge = SecretHelper.GenerateCodeChallenge(codeVerifier),
            CodeVerifier = codeVerifier,
            TenantId = tenantId,
            ClientId = clientId,
            Instance = instance,
            Success = true
        };

        var cacheOptions = new MemoryCacheEntryOptions();
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
        _memoryCache.Set(response.LoginSecret, response, cacheOptions);
        return response;
    }
}