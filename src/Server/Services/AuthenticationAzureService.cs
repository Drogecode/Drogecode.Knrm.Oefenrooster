using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class AuthenticationAzureService : AuthenticationService, IAuthenticationService
{
    public IdentityProvider IdentityProvider => IdentityProvider.Azure;
    
    public AuthenticationAzureService(
        ILogger logger,
        IMemoryCache memoryCache,
        IConfiguration configuration,
        HttpClient httpClient, 
        DataContext database) : base(logger, memoryCache, configuration, httpClient, database)
    {
    }

    public async Task<GetLoginSecretsResponse> GetLoginSecrets()
    {
        var tenantId = _configuration.GetValue<string>("AzureAd:TenantId") ?? throw new DrogeCodeNullException("no tenant id found for azure login");
        var clientId = _configuration.GetValue<string>("AzureAd:LoginClientId") ?? throw new DrogeCodeNullException("no client id found for azure login");
        return await GetLoginSecretShared(IdentityProvider.Azure, tenantId, clientId, null);
    }

    public async Task<AuthenticateUserResult> AuthenticateUser(CacheLoginSecrets found, string code, string state, string sessionState, string redirectUrl, CancellationToken clt)
    {
        var tenantId = _configuration.GetValue<string>("AzureAd:TenantId") ?? throw new DrogeCodeConfigurationException("no tenant id found for azure login");
        var secret = InternalGetLoginClientSecret();
        var clientId = _configuration.GetValue<string>("AzureAd:LoginClientId") ?? throw new DrogeCodeConfigurationException("no client id found for azure login");
        var scope = _configuration.GetValue<string>("AzureAd:LoginScope") ?? throw new DrogeCodeConfigurationException("no scope found for azure login");
        return await AuthenticateUserShared($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token", clientId, scope, code, secret, redirectUrl, found, clt);
    }

    public async Task<AuthenticateUserResult> Refresh(string oldRefreshToken, CancellationToken clt)
    {
        // https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-auth-code-flow#refresh-the-access-token
        var tenantId = _configuration.GetValue<string>("AzureAd:TenantId") ?? throw new DrogeCodeConfigurationException("no tenant id found for azure login");
        var secret = InternalGetLoginClientSecret();
        var clientId = _configuration.GetValue<string>("AzureAd:LoginClientId") ?? throw new DrogeCodeConfigurationException("no client id found for azure login");
        var scope = _configuration.GetValue<string>("AzureAd:LoginScope") ?? throw new DrogeCodeConfigurationException("no scope found for azure login");
        return await RefreshShared($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token", clientId, scope, oldRefreshToken, secret, clt);
    }

    public DrogeClaims GetClaims(AuthenticateUserResult subResult)
    {
        var jwtSecurityToken = subResult.JwtSecurityToken!;
        return new DrogeClaims()
        {
            Email = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value ?? "",
            FullName = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "name")?.Value ?? "",
            ExternalUserId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "oid")?.Value ?? "",
            LoginHint = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "login_hint")?.Value ?? "",
            IdToken = "",
            TenantId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "tid")?.Value ?? ""
        };
    }

    public Task<bool> AuditLogin(Guid? userId, Guid? sharedActionId, string ipAddress, string clientVersion, bool directLogin, CancellationToken clt)
    {
        return AuditLoginShared(userId, sharedActionId, ipAddress, clientVersion, directLogin , clt);
    }

    private string InternalGetLoginClientSecret()
    {
        var secret = _configuration.GetValue<string>("AzureAd:LoginClientSecret");
        if (!string.IsNullOrWhiteSpace(secret)) return secret;
        var fromKeyVault = KeyVaultHelper.GetSecret("LoginClientSecret", _logger);
        if (fromKeyVault is not null) return fromKeyVault.Value;
        throw new DrogeCodeConfigurationException("no secret found for azure login");
    }
}