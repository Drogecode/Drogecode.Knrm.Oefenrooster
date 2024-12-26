using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;

public abstract class AuthenticationService
{
    protected readonly ILogger _logger;
    private readonly IMemoryCache _memoryCache;
    protected readonly IConfiguration _configuration;
    protected readonly HttpClient _httpClient;

    public AuthenticationService(
        ILogger logger,
        IMemoryCache memoryCache,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _configuration = configuration;
        _httpClient = httpClient;
    }
    protected async Task<GetLoginSecretsResponse> GetLoginSecretShared(IdentityProvider provider, string tenantId, string clientId, string? instance)
    {
        var codeVerifier = SecretHelper.CreateSecret(120);
        var response = new CacheLoginSecrets
        {
            IdentityProvider = provider,
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