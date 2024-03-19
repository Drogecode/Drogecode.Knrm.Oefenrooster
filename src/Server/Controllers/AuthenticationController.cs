﻿using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ZXing.QrCode.Internal;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IUserRoleService _userRoleService;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    private const string REFRESHTOKEN = "RefreshToken";

    public AuthenticationController(
        ILogger<AuthenticationController> logger,
        IMemoryCache memoryCache,
        IUserRoleService userRoleService,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _userRoleService = userRoleService;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    [HttpGet]
    [Route("login-secrets")]
    public async Task<ActionResult<GetLoginSecretsResponse>> GetLoginSecrets()
    {
        try
        {
            var codeVerifier = CreateSecret(120);
            var response = new CacheLoginSecrets
            {
                LoginSecret = CreateSecret(64),
                LoginNonce = CreateSecret(64),
                CodeChallenge = GenerateCodeChallenge(codeVerifier), //BASE64URL-ENCODE(SHA256(ASCII(code_verifier)))
                CodeVerifier = codeVerifier,
                Success = true
            };

            var cacheOptions = new MemoryCacheEntryOptions();
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            _memoryCache.Set(response.LoginSecret, response, cacheOptions);
            return response as GetLoginSecretsResponse;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetLoginSecrets");
            return BadRequest();
        }
    }
    private static string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        var b64Hash = Convert.ToBase64String(hash);
        var code = Regex.Replace(b64Hash, "\\+", "-");
        code = Regex.Replace(code, "\\/", "_");
        code = Regex.Replace(code, "=+$", "");
        return code;
    }

    [HttpGet]
    [Route("authenticat-user")]
    public async Task<ActionResult<bool>> AuthenticatUser(string code, string state, string sessionState, string redirectUrl, CancellationToken clt = default)
    {
        try
        {
            string id_token = "";
            string refresh_token = "";
            var found = _memoryCache.Get<CacheLoginSecrets>(state);
            if (found?.Success is not true || found?.CodeVerifier is null)
            {
                _logger.LogWarning("fund?.success = `{false}` || found?.CodeVerifier = `{null}`", found?.Success is not true, found?.CodeVerifier is null);
                return false;
            }
            _memoryCache.Remove(state);
            var tenantId = _configuration.GetValue<string>("AzureAd:TenantId") ?? throw new Exception("no tenant id found for azure login");
            var secret = _configuration.GetValue<string>("AzureAd:LoginClientSecret") ?? throw new Exception("no secret found for azure login");
            var clientId = _configuration.GetValue<string>("AzureAd:LoginClientId") ?? throw new Exception("no client id found for azure login");
            var scope = _configuration.GetValue<string>("AzureAd:LoginScope") ?? throw new Exception("no scope found for azure login");
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
            using (var response = await _httpClient.PostAsync($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token", formContent))
            {
                string responseString = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var resObj = JsonConvert.DeserializeObject<AzureLoginResponse>(responseString);
                    id_token = resObj?.id_token ?? "";
                    refresh_token = resObj?.refresh_token ?? "";
                }
                else
                {
                    _logger.LogWarning("Failed login: {jsonstring}", responseString);
                    return false;
                }
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(id_token);
            if (string.Compare(found.LoginNonce, jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "nonce")?.Value, false) != 0)
            {
                _logger.LogWarning("Nonce is wrong `{cache}` != `{jwt}`", found.LoginNonce, jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "nonce")?.Value ?? "null");
                return false;
            }

            await SetUser(jwtSecurityToken, refresh_token, false, clt);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "AuthenticatUser");
            return false;
        }
    }

    private class AzureLoginResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }
    }

    [HttpGet]
    [Route("current-user-info")]
    public async Task<ActionResult<CurrentUser>> CurrentUserInfo(CancellationToken clt = default)
    {
        try
        {
            if (User?.Identity?.IsAuthenticated != true)
                return new CurrentUser
                {
                    IsAuthenticated = false
                };
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var res = new CurrentUser
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                UserName = User.Identity.Name,
                Claims = User.Claims.Select(claim => new KeyValuePair<string, string>(claim.Type, claim.Value)).ToList(),
                Id = userId,
            };
            return res;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CurrentUserInfo");
            return BadRequest();
        }
    }

    [Authorize]
    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }

    [Authorize]
    [HttpPost]
    [Route("refresh")]
    public async Task<ActionResult<bool>> Refresh(CancellationToken clt = default)
    {
        // https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-auth-code-flow#refresh-the-access-token
        try
        {
            if (User?.Identity?.IsAuthenticated != true)
                return false;

            var oldRefreshToken = User?.FindFirstValue(REFRESHTOKEN);
            if (oldRefreshToken == null)
                return false;

            string id_token = "";
            string refresh_token = "";
            var tenantId = _configuration.GetValue<string>("AzureAd:TenantId") ?? throw new Exception("no tenant id found for azure login");
            var secret = _configuration.GetValue<string>("AzureAd:LoginClientSecret") ?? throw new Exception("no secret found for azure login");
            var clientId = _configuration.GetValue<string>("AzureAd:LoginClientId") ?? throw new Exception("no client id found for azure login");
            var scope = _configuration.GetValue<string>("AzureAd:LoginScope") ?? throw new Exception("no scope found for azure login");
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("scope", scope),
                new KeyValuePair<string, string>("refresh_token", oldRefreshToken),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_secret", secret),
            });
            using (var response = await _httpClient.PostAsync($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token", formContent))
            {
                string responseString = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var resObj = JsonConvert.DeserializeObject<AzureLoginResponse>(responseString);
                    id_token = resObj?.id_token ?? "";
                    refresh_token = resObj?.refresh_token ?? "";
                }
                else
                {
                    _logger.LogWarning("Failed refresh: {jsonstring}", responseString);
                    return false;
                }
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(id_token);
            await SetUser(jwtSecurityToken, refresh_token, false, clt);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Refresh");
            return false;
        }
    }

    private async Task SetUser(JwtSecurityToken jwtSecurityToken, string refresh_token, bool rememberMe, CancellationToken clt)
    {
        var claims = await GetClaimsList(jwtSecurityToken, refresh_token);

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            //AllowRefresh = <bool>,
            // Refreshing the authentication session should be allowed.

            //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
            // The time at which the authentication ticket expires. A 
            // value set here overrides the ExpireTimeSpan option of 
            // CookieAuthenticationOptions set with AddCookie.

            IsPersistent = rememberMe
            // Whether the authentication session is persisted across 
            // multiple requests. When used with cookies, controls
            // whether the cookie's lifetime is absolute (matching the
            // lifetime of the authentication ticket) or session-based.

            //IssuedUtc = <DateTimeOffset>,
            // The time at which the authentication ticket was issued.

            //RedirectUri = <string>
            // The full path or absolute URI to be used as an http 
            // redirect response value.
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }

    private async Task<IEnumerable<Claim>> GetClaimsList(JwtSecurityToken jwtSecurityToken, string refresh_token)
    {
        var email = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value ?? "";
        var fullName = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "name")?.Value ?? "";
        var userId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "oid")?.Value ?? "";
        var loginHint = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "login_hint")?.Value ?? "";
        var customerId = new Guid(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "tid")?.Value ?? throw new Exception("customerId not found"));
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, email),
            new("FullName", fullName),
            new("login_hint", loginHint),
            new(REFRESHTOKEN, refresh_token),
            new("ValidFrom", jwtSecurityToken.ValidFrom.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
            new("ValidTo", jwtSecurityToken.ValidTo.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
            new("http://schemas.microsoft.com/identity/claims/objectidentifier", userId),
            new("http://schemas.microsoft.com/identity/claims/tenantid", customerId.ToString())
        };
        if (string.Compare(userId, DefaultSettingsHelper.IdTaco.ToString(), true) == 0)
        {
            claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_Taco));
            claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_scheduler_history));
        }
        var accesses = await _userRoleService.GetAccessForUser(customerId, jwtSecurityToken.Claims);
        if (accesses == null) return claims;
        foreach (var access in accesses)
        {
            claims.Add(new Claim(ClaimTypes.Role, access));
        }

        return claims;
    }

    private string CreateSecret(int length)
    {
        StringBuilder res = new StringBuilder();
        byte[] random = new byte[1];
        using (var cryptoProvider = new RNGCryptoServiceProvider())
        {
            while (0 < length--)
            {
                char rndChar = '\0';
                do
                {
                    cryptoProvider.GetBytes(random);
                    rndChar = (char)((random[0] % 92) + 33);
                } while (!char.IsLetterOrDigit(rndChar));
                res.Append(rndChar);
            }
        }
        return res.ToString();
    }
}
