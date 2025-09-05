using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;

public abstract class AuthenticationService
{
    protected readonly ILogger _logger;
    private readonly IMemoryCache _memoryCache;
    protected readonly IConfiguration _configuration;
    protected readonly HttpClient _httpClient;
    private readonly DataContext _database;

    public AuthenticationService(
        ILogger logger,
        IMemoryCache memoryCache,
        IConfiguration configuration,
        HttpClient httpClient, 
        DataContext database)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _configuration = configuration;
        _httpClient = httpClient;
        _database = database;
    }

    public async Task<bool> ValidatePassword(string password, string hashedPassword, CancellationToken clt)
    {
        if (!_configuration.GetValue<bool>("Drogecode:DirectLogin"))
        {
            _logger.LogWarning("Drogecode:DirectLogin is not enabled");
            return false;
        }

        if (clt.IsCancellationRequested)
            return false;
        return PasswordHasher.ComparePassword(password, hashedPassword);
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
        cacheOptions.Priority = CacheItemPriority.High;
        _memoryCache.Set(response.LoginSecret, response, cacheOptions);
        return response;
    }

    protected async Task<AuthenticateUserResult> AuthenticateUserShared(string requestUrl, string clientId, string scope, string code, string secret, string redirectUrl, CacheLoginSecrets found, CancellationToken clt)
    {
        var result = new AuthenticateUserResult
        {
            Success = false,
        };
        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("scope", scope),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirectUrl),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code_verifier", found.CodeVerifier),
            new KeyValuePair<string, string>("client_secret", secret),
        });
        using (var response = await _httpClient.PostAsync(requestUrl, formContent))
        {
            var responseString = await response.Content.ReadAsStringAsync(clt);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resObj = JsonConvert.DeserializeObject<LoginResponse>(responseString);
                result.IdToken = resObj?.id_token ?? "";
                result.RefreshToken = resObj?.refresh_token ?? "";
            }
            else
            {
                _logger.LogWarning("Failed login: {jsonstring}", responseString);
                return result;
            }
        }

        var handler = new JwtSecurityTokenHandler();
        result.JwtSecurityToken = handler.ReadJwtToken(result.IdToken);
        if (string.Compare(found.LoginNonce, result.JwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "nonce")?.Value, false, CultureInfo.InvariantCulture) != 0)
        {
            _logger.LogWarning("Nonce is wrong `{cache}` != `{jwt}`", found.LoginNonce, result.JwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "nonce")?.Value ?? "null");
            return result;
        }

        result.Success = true;
        return result;
    }

    protected async Task<AuthenticateUserResult> RefreshShared(string requestUrl, string clientId, string scope, string oldRefreshToken, string secret, CancellationToken clt)
    {
        var result = new AuthenticateUserResult
        {
            Success = false,
        };
        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("scope", scope),
            new KeyValuePair<string, string>("refresh_token", oldRefreshToken),
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("client_secret", secret),
        });
        using (var response = await _httpClient.PostAsync(requestUrl, formContent))
        {
            string responseString = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resObj = JsonConvert.DeserializeObject<LoginResponse>(responseString);
                result.IdToken = resObj?.id_token ?? "";
                result.RefreshToken = resObj?.refresh_token ?? "";
            }
            else
            {
                _logger.LogWarning("Failed refresh: {jsonstring}", responseString);
                return result;
            }
        }

        var handler = new JwtSecurityTokenHandler();
        result.JwtSecurityToken = handler.ReadJwtToken(result.IdToken);
        result.Success = true;
        return result;
    }

    protected async Task<bool> AuditLoginShared(Guid? userId, Guid? sharedActionId, string ipAddress, string clientVersion, bool directLogin, CancellationToken clt)
    {
        _database.UserLogins.Add(
            new DbUserLogins
            {
                Id = Guid.NewGuid(),
                UserId = userId, 
                SharedActionId = sharedActionId, 
                Ip = ipAddress,
                LoginDate = DateTime.UtcNow,
                DirectLogin = directLogin,
                Version = clientVersion
            });
        return await _database.SaveChangesAsync(clt) > 0;
    }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private class LoginResponse
    {
        public string? access_token { get; set; }
        public string? token_type { get; set; }
        public int expires_in { get; set; }
        public string? scope { get; set; }
        public string? refresh_token { get; set; }
        public string? id_token { get; set; }
    }
}