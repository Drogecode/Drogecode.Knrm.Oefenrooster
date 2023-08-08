using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using ZXing.Aztec.Internal;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using System.Security;
using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;
using static MudBlazor.CategoryTypes;
using System.Text;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IUserRoleService _userRoleService;

    public AuthenticationController(
        ILogger<AuthenticationController> logger,
        IMemoryCache memoryCache,
        IUserRoleService userRoleService)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _userRoleService = userRoleService;
    }

    [HttpGet]
    [Route("login-secrets")]
    public async Task<ActionResult<GetLoginSecretsResponse>> GetLoginSecrets()
    {
        try
        {
            var response = new GetLoginSecretsResponse
            {
                LoginSecret = CreateSecret(64),
                LoginNonce = CreateSecret(64),
                Success = true
            };

            var cacheOptions = new MemoryCacheEntryOptions();
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            _memoryCache.Set(response.LoginSecret, response, cacheOptions);
            return response;

        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetLoginSecrets");
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("login-callback")]
    public async Task Logincallback([FromForm] string id_token, [FromForm] string state, [FromForm] string session_state, CancellationToken clt = default)
    {
        var found = _memoryCache.Get<GetLoginSecretsResponse>(state);
        if (found?.Success is not true) return;
        _memoryCache.Remove(state);

        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(id_token);
        if (string.Compare(found.LoginNonce, jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "nonce")?.Value, false) != 0) return;

        await SetUser(jwtSecurityToken, false, clt);
        Response.Redirect("/authentication/login-callback");
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
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }

    private async Task SetUser(JwtSecurityToken jwtSecurityToken, bool rememberMe, CancellationToken clt)
    {
        var claims = await GetClaimsList(jwtSecurityToken);

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

    public async Task<IEnumerable<Claim>> GetClaimsList(JwtSecurityToken jwtSecurityToken)
    {
        var email = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
        var fullName = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
        var userId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "oid")?.Value;
        var customerId = new Guid(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "tid")?.Value ?? throw new Exception("customerId not found"));
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, email),
            new("FullName", fullName),
            new("http://schemas.microsoft.com/identity/claims/objectidentifier", userId),
            new("http://schemas.microsoft.com/identity/claims/tenantid", customerId.ToString())
        };
        if (string.Compare(userId, DefaultSettingsHelper.IdTaco.ToString(), true) == 0)
            claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_Taco));
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
