using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class AuthenticationAzureService : IAuthenticationService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;

    public AuthenticationAzureService(
        IMemoryCache memoryCache,
        IConfiguration configuration)
    {
        _memoryCache = memoryCache;
        _configuration = configuration;
    }
    
    public async Task<GetLoginSecretsResponse> GetLoginSecrets()
    {
        var codeVerifier = SecretHelper.CreateSecret(120);
        var tenantId = _configuration.GetValue<string>("AzureAd:TenantId") ?? throw new DrogeCodeNullException("no tenant id found for azure login");
        var clientId = _configuration.GetValue<string>("AzureAd:LoginClientId") ?? throw new DrogeCodeNullException("no client id found for azure login");
        var response = new CacheLoginSecrets
        {
            IdentityProvider = IdentityProvider.Azure,
            LoginSecret = SecretHelper.CreateSecret(64),
            LoginNonce = SecretHelper.CreateSecret(64),
            CodeChallenge = SecretHelper.GenerateCodeChallenge(codeVerifier),
            CodeVerifier = codeVerifier,
            TenantId = tenantId,
            ClientId = clientId,
            Success = true
        };

        var cacheOptions = new MemoryCacheEntryOptions();
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
        _memoryCache.Set(response.LoginSecret, response, cacheOptions);
        return response;
    }
}