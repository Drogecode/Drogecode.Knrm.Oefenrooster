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

public class AuthenticationKeyCloakService : AuthenticationService, IAuthenticationService
{
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
        var result = new AuthenticateUserResult
        {
            Success = false,
        };
        var tenantId = _configuration.GetValue<string>("KeyCloak:TenantId") ?? throw new DrogeCodeConfigurationException("no tenant id found for KeyCloak login");
        var secret = InternalGetLoginClientSecret();
        var clientId = _configuration.GetValue<string>("KeyCloak:ClientId") ?? throw new DrogeCodeConfigurationException("no client id found for KeyCloak login");
        var scope = _configuration.GetValue<string>("KeyCloak:Scopes") ?? throw new DrogeCodeConfigurationException("no scope found for KeyCloak login");
        var instance = _configuration.GetValue<string>("KeyCloak:Instance") ?? throw new DrogeCodeNullException("no instance found for KeyCloak login");
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
        using (var response = await _httpClient.PostAsync($"{instance}/realms/{tenantId}/protocol/openid-connect/token", formContent))
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

    public Task<AuthenticateUserResult> Refresh(string oldRefreshToken, CancellationToken clt)
    {
        throw new NotImplementedException();
    }

    public DrogeClaims GetClaims(JwtSecurityToken jwtSecurityToken)
    {
        return new DrogeClaims()
        {
            Email = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value ?? "",
            FullName = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "name")?.Value ?? jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value ?? "",
            ExternalUserId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? "",
            LoginHint = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "login_hint")?.Value ?? "",
            CustomerId = new Guid("118a24f6-8114-4b86-850a-971e75f7178e") // ToDo: hardcoded for now, Keycloak does not have a guid as id
            //CustomerId = new Guid(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "iss")?.Value ?? throw new Exception("customerId not found"))
        };
    }

    private string InternalGetLoginClientSecret()
    {
        var fromKeyVault = KeyVaultHelper.GetSecret("LoginClientSecret");
        if (fromKeyVault is not null) return fromKeyVault.Value;
        return _configuration.GetValue<string>("KeyCloak:ClientSecret") ?? throw new DrogeCodeConfigurationException("no secret found for keycloak login");
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