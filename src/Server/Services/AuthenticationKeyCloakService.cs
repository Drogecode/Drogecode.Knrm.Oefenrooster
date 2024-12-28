using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

//https://keycloaktest.droogers.cloud/realms/master/.well-known/openid-configuration
public class AuthenticationKeyCloakService : AuthenticationService, IAuthenticationService
{
    public IdentityProvider IdentityProvider => IdentityProvider.KeyCloak;
    
    public AuthenticationKeyCloakService(
        ILogger logger,
        IMemoryCache memoryCache,
        IConfiguration configuration,
        HttpClient httpClient) : base(logger,memoryCache,configuration,httpClient)
    {
    }

    public async Task<GetLoginSecretsResponse> GetLoginSecrets()
    {
        var tenantId = _configuration.GetValue<string>("KeyCloak:TenantId") ?? throw new DrogeCodeNullException("no tenant id found for KeyCloak login");
        var clientId = _configuration.GetValue<string>("KeyCloak:ClientId") ?? throw new DrogeCodeNullException("no client id found for KeyCloak login");
        var instance = _configuration.GetValue<string>("KeyCloak:Instance") ?? throw new DrogeCodeNullException("no instance found for KeyCloak login");
        return await GetLoginSecretShared(IdentityProvider.KeyCloak, tenantId, clientId, instance);
    }

    public async Task<AuthenticateUserResult> AuthenticateUser(CacheLoginSecrets found, string code, string state, string sessionState, string redirectUrl, CancellationToken clt)
    {
        var tenantId = _configuration.GetValue<string>("KeyCloak:TenantId") ?? throw new DrogeCodeConfigurationException("no tenant id found for KeyCloak login");
        var secret = InternalGetLoginClientSecret();
        var clientId = _configuration.GetValue<string>("KeyCloak:ClientId") ?? throw new DrogeCodeConfigurationException("no client id found for KeyCloak login");
        var scope = _configuration.GetValue<string>("KeyCloak:Scopes") ?? throw new DrogeCodeConfigurationException("no scope found for KeyCloak login");
        var instance = _configuration.GetValue<string>("KeyCloak:Instance") ?? throw new DrogeCodeNullException("no instance found for KeyCloak login");
        return await AuthenticateUserShared($"{instance}/realms/{tenantId}/protocol/openid-connect/token", clientId, scope, code, secret, redirectUrl, found, clt);
    }

    public async Task<AuthenticateUserResult> Refresh(string oldRefreshToken, CancellationToken clt)
    {
        var result = new AuthenticateUserResult
        {
            Success = false,
        };
        var tenantId = _configuration.GetValue<string>("KeyCloak:TenantId") ?? throw new DrogeCodeConfigurationException("no tenant id found for KeyCloak refresh");
        var secret = InternalGetLoginClientSecret();
        var clientId = _configuration.GetValue<string>("KeyCloak:ClientId") ?? throw new DrogeCodeConfigurationException("no client id found for KeyCloak refresh");
        var scope = _configuration.GetValue<string>("KeyCloak:Scopes") ?? throw new DrogeCodeConfigurationException("no scope found for KeyCloak refresh");
        var instance = _configuration.GetValue<string>("KeyCloak:Instance") ?? throw new DrogeCodeNullException("no instance found for KeyCloak refresh");
        return await RefreshShared($"{instance}/realms/{tenantId}/protocol/openid-connect/token", clientId, scope, oldRefreshToken, secret, clt);
    }

    public DrogeClaims GetClaims(JwtSecurityToken jwtSecurityToken)
    {
        return new DrogeClaims()
        {
            Email = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value ?? "",
            FullName = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "name")?.Value ?? jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value ?? "",
            ExternalUserId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? "",
            LoginHint = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "login_hint")?.Value ?? "",
            TenantId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "iss")?.Value ?? ""
        };
    }

    private string InternalGetLoginClientSecret()
    {
        var fromKeyVault = KeyVaultHelper.GetSecret("LoginClientSecret");
        if (fromKeyVault is not null) return fromKeyVault.Value;
        return _configuration.GetValue<string>("KeyCloak:ClientSecret") ?? throw new DrogeCodeConfigurationException("no secret found for keycloak login");
    }
}