﻿using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

//https://keycloaktest.droogers.cloud/realms/master/.well-known/openid-configuration
public class AuthenticationKeyCloakService : AuthenticationService, IAuthenticationService
{
    public IdentityProvider IdentityProvider => IdentityProvider.KeyCloak;

    public AuthenticationKeyCloakService(
        ILogger logger,
        IMemoryCache memoryCache,
        IConfiguration configuration,
        HttpClient httpClient,
        DataContext database) : base(logger, memoryCache, configuration, httpClient, database)
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

    public DrogeClaims GetClaims(AuthenticateUserResult subResult)
    {
        var jwtSecurityToken = subResult.JwtSecurityToken!;
        return new DrogeClaims()
        {
            Email = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value ?? "",
            FullName = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "name")?.Value ?? jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value ?? "",
            ExternalUserId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? "",
            LoginHint = "",
            IdToken = subResult.IdToken,
            TenantId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "iss")?.Value ?? ""
        };
    }

    public Task<bool> AuditLogin(Guid? userId, Guid? sharedActionId, string ipAddress, string clientVersion, bool directLogin, CancellationToken clt)
    {
        return AuditLoginShared(userId, sharedActionId, ipAddress, clientVersion, directLogin, clt);
    }
    
    public string GetTenantId()
    {
        var tenantId = _configuration.GetValue<string>("KeyCloak:TenantId") ?? throw new DrogeCodeConfigurationException("no tenant id found for KeyCloak refresh");
        return tenantId;
    }

    private string InternalGetLoginClientSecret()
    {
        var secret = _configuration.GetValue<string>("KeyCloak:ClientSecret");
        if (!string.IsNullOrWhiteSpace(secret)) return secret;
        var fromKeyVault = KeyVaultHelper.GetSecret("LoginClientSecret", _logger);
        if (fromKeyVault is not null) return fromKeyVault.Value;
        throw new DrogeCodeConfigurationException("no secret found for keycloak login");
    }
}