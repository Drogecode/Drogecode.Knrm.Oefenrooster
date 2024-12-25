using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
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
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Services;
using IAuthenticationService = Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces.IAuthenticationService;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IUserRoleService _userRoleService;
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    private const string REFRESHTOKEN = "RefreshToken";

    public AuthenticationController(
        ILogger<AuthenticationController> logger,
        IMemoryCache memoryCache,
        IUserRoleService userRoleService,
        IUserService userService,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _userRoleService = userRoleService;
        _userService = userService;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    [HttpGet]
    [Route("login-secrets")]
    public async Task<ActionResult<GetLoginSecretsResponse>> GetLoginSecrets()
    {
        try
        {
            var authService = GetAuthenticationService(); 
            return await authService.GetLoginSecrets();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetLoginSecrets");
            return BadRequest();
        }
    }

    private IAuthenticationService GetAuthenticationService()
    {
        var identityProvider =_configuration.GetValue<IdentityProvider>("IdentityProvider");
        IAuthenticationService authService;  
        switch (identityProvider)
        {
            case IdentityProvider.Azure:
                authService = new AuthenticationAzureService(_logger, _memoryCache, _configuration, _httpClient);
                break;
            case  IdentityProvider.KeyCloak:
                authService = new AuthenticationKeyCloakService(_logger, _memoryCache, _configuration, _httpClient);
                break;
            case IdentityProvider.None:
            default:
                throw new ArgumentOutOfRangeException($"identityProvider: `{identityProvider}` is not supported");
        }
        return authService;
    }

    [HttpGet]
    [Route("authenticate-user")]
    public async Task<ActionResult<bool>> AuthenticateUser(string code, string state, string sessionState, string redirectUrl, CancellationToken clt = default)
    {
        try
        {
            var found = _memoryCache.Get<CacheLoginSecrets>(state);
            if (found?.Success is not true || found.CodeVerifier is null)
            {
                _logger.LogWarning("found?.success = `{false}` || found?.CodeVerifier = `{null}`", found?.Success is not true, found?.CodeVerifier is null);
                return false;
            }

            _memoryCache.Remove(state);
            
            var authService = GetAuthenticationService(); 
            var supResult = await authService.AuthenticateUser(found, code, state, sessionState, redirectUrl, clt);
            if (supResult.Success is not true || supResult.JwtSecurityToken is null)
                return false;

            await SetUser(supResult.JwtSecurityToken, supResult.RefreshToken, false, clt);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "AuthenticatUser");
            return false;
        }
    }

    [HttpGet]
    [Route("current-user-info")]
    public async Task<ActionResult<CurrentUser>> CurrentUserInfo(CancellationToken clt = default)
    {
        try
        {
            if (User.Identity?.IsAuthenticated != true)
                return new CurrentUser
                {
                    IsAuthenticated = false
                };
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No objectidentifier found"));
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
        try
        {
            if (User?.Identity?.IsAuthenticated != true)
                return false;

            var oldRefreshToken = User?.FindFirstValue(REFRESHTOKEN);
            if (oldRefreshToken == null)
                return false;

            var authService = GetAuthenticationService(); 
            var supResult = await authService.Refresh(oldRefreshToken, clt);
            if (supResult.Success is not true || supResult.JwtSecurityToken is null)
                return false;
            
            await SetUser(supResult.JwtSecurityToken, supResult.RefreshToken, false, clt);
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
        var claims = await GetClaimsList(jwtSecurityToken, refresh_token, clt);

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

    private async Task<IEnumerable<Claim>> GetClaimsList(JwtSecurityToken jwtSecurityToken, string refresh_token, CancellationToken clt)
    {
        var authService = GetAuthenticationService();
        var drogeClaims =  authService.GetClaims(jwtSecurityToken);
        var userId = await GetUserIdByExternalId(drogeClaims.ExternalUserId, drogeClaims.FullName, drogeClaims.Email, drogeClaims.CustomerId, clt);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, drogeClaims.Email),
            new("FullName", drogeClaims.FullName),
            new("ExternalUserId", drogeClaims.ExternalUserId),
            new("login_hint", drogeClaims.LoginHint),
            new(REFRESHTOKEN, refresh_token),
            new("ValidFrom", jwtSecurityToken.ValidFrom.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
            new("ValidTo", jwtSecurityToken.ValidTo.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
            new("IdentityProvider", authService.IdentityProvider.ToString() ),
            new("http://schemas.microsoft.com/identity/claims/objectidentifier", userId.ToString()),
            new("http://schemas.microsoft.com/identity/claims/tenantid", drogeClaims.CustomerId.ToString())
        };
        var superUsers = _configuration.GetSection("DrogeCode:SuperAdmin").Get<List<Guid>>();
        if (superUsers is not null && superUsers.Contains(userId))
        {
            claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_super_user));
            claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_configure_user_roles));
        }

        var accesses = await _userRoleService.GetAccessForUser(userId, drogeClaims.CustomerId, jwtSecurityToken.Claims, clt);
        foreach (var access in accesses)
        {
            claims.Add(new Claim(ClaimTypes.Role, access));
        }

        return claims;
    }

    internal async Task<Guid> GetUserIdByExternalId(string externalUserId, string userName, string userEmail, Guid customerId, CancellationToken clt)
    {
        var user = await _userService.GetOrSetUserById(null, externalUserId, userName, userEmail, customerId, true, clt);
        if (user is null)
        {
            _logger.LogWarning("Failed to get or set user by external id");
            throw new UnauthorizedAccessException();
        }

        return user.Id;
    }
}