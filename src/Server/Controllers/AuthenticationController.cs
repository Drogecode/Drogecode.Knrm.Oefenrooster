using Drogecode.Knrm.Oefenrooster.Server.Controllers.Abstract;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Server.Services;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using IAuthenticationService = Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces.IAuthenticationService;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Authentication")]
public class AuthenticationController : DrogeController
{
    private readonly IMemoryCache _memoryCache;
    private readonly IUserRoleService _userRoleService;
    private readonly IUserService _userService;
    private readonly ICustomerSettingService _customerSettingService;
    private readonly IReportActionSharedService _reportActionSharedService;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly DataContext _database;
    private IAuthenticationService? _authService;

    private const string REFRESHTOKEN = "RefreshToken";

    public AuthenticationController(
        ILogger<AuthenticationController> logger,
        IMemoryCache memoryCache,
        IUserRoleService userRoleService,
        IUserService userService,
        ICustomerSettingService customerSettingService,
        IReportActionSharedService reportActionSharedService,
        IConfiguration configuration,
        HttpClient httpClient,
        DataContext database) : base(logger)
    {
        _memoryCache = memoryCache;
        _userRoleService = userRoleService;
        _userService = userService;
        _customerSettingService = customerSettingService;
        _reportActionSharedService = reportActionSharedService;
        _configuration = configuration;
        _httpClient = httpClient;
        _database = database;
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
        if (_authService is not null)
            return _authService;
        var identityProvider = _configuration.GetValue<IdentityProvider>("IdentityProvider");
        switch (identityProvider)
        {
            case IdentityProvider.Azure:
                _authService = new AuthenticationAzureService(_logger, _memoryCache, _configuration, _httpClient, _database);
                break;
            case IdentityProvider.KeyCloak:
                _authService = new AuthenticationKeyCloakService(_logger, _memoryCache, _configuration, _httpClient, _database);
                break;
            case IdentityProvider.None:
            default:
                throw new ArgumentOutOfRangeException($"identityProvider: `{identityProvider}` is not supported");
        }

        return _authService;
    }

    [HttpPost]
    [Route("authenticate-user")]
    public async Task<ActionResult<bool>> AuthenticateUser([FromBody] AuthenticateRequest body, CancellationToken clt = default)
    {
        try
        {
            var found = _memoryCache.Get<CacheLoginSecrets>(body.State);
            if (found?.Success is not true || found.CodeVerifier is null)
            {
                _logger.LogWarning("found?.success = `{false}` || found?.CodeVerifier = `{null}`", found?.Success is not true, found?.CodeVerifier is null);
                return false;
            }

            _memoryCache.Remove(body.State);

            var authService = GetAuthenticationService();
            var supResult = await authService.AuthenticateUser(found, body.Code, body.State, body.SessionState, body.RedirectUrl, clt);
            if (supResult.Success is not true || supResult.JwtSecurityToken is null)
                return false;

            await SetUser(supResult, false, body.ClientVersion, clt);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "AuthenticateUser POST");
            return false;
        }
    }
    
    [HttpPost]
    [Route("authenticate-external")]
    public async Task<ActionResult<bool>> AuthenticateExternal([FromBody] AuthenticateExternalRequest body, CancellationToken clt = default)
    {
        try
        {
            if (User?.Identity?.IsAuthenticated == true && !User.IsInRole(AccessesNames.AUTH_External))
            {
                _logger.LogInformation("AuthenticateExternal request, but user is already authenticated");
                return false;
            }

            var passwordCorrect = await _reportActionSharedService.AuthenticateExternal(body, clt);
            if (!passwordCorrect.Success) return false;
            var authService = GetAuthenticationService();
            var ip = GetRequesterIp();
            var claims = new List<Claim>
            {
                new("http://schemas.microsoft.com/identity/claims/objectidentifier", body.ExternalId?.ToString() ?? ""),
                new("http://schemas.microsoft.com/identity/claims/tenantid", passwordCorrect.CustomerId.ToString()),
                new("ExternalId", body.ExternalId?.ToString() ?? throw new AggregateException("ExternalId is null")),
                new(ClaimTypes.Role, AccessesNames.AUTH_External),
                new("ValidFrom", DateTime.UtcNow.AddMinutes(-10).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                new("ValidTo", DateTime.UtcNow.AddHours(1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            await authService.AuditLogin(null, body.ExternalId, ip, body.ClientVersion, true, clt);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "AuthenticateExternal");
            return false;
        }
    }

    [HttpGet]
    [Route("authenticate-direct/enabled")]
    public async Task<ActionResult<bool>> GetAuthenticateDirectEnabled()
    {
        return _configuration.GetValue<bool>("Drogecode:DirectLogin");
    }

    [HttpPost]
    [Route("authenticate-direct")]
    public async Task<ActionResult<bool>> AuthenticateDirect([FromBody] AuthenticateDirectRequest body, CancellationToken clt = default)
    {
        try
        {
            if (!_configuration.GetValue<bool>("Drogecode:DirectLogin"))
            {
                _logger.LogWarning("Drogecode:DirectLogin is not enabled");
                return false;
            }

            if (User?.Identity?.IsAuthenticated == true && !User.IsInRole(AccessesNames.AUTH_External))
            {
                _logger.LogInformation("AuthenticateDirect request, but user is already authenticated");
                return false;
            }

            var user = await _userService.GetUserByNameForServer(body.Name, clt);
            if (user is null)
            {
                _logger.LogInformation("No user found with name {name}", body.Name?.CleanStringForLogging());
                return false;
            }

            if (user.HashedPassword is null || body.Passwoord is null)
            {
                _logger.LogInformation("No password `{hashed}` '{fromBody}' found ", user.HashedPassword is null, body.Passwoord is null);
                return false;
            }

            var authService = GetAuthenticationService();

            var passwordCorrect = await authService.ValidatePassword(body.Passwoord, user.HashedPassword, clt);
            if (!passwordCorrect)
            {
                _logger.LogInformation("Wrong password for user {name}", body.Name?.CleanStringForLogging());
                return false;
            }

            var ip = GetRequesterIp();
            var claims = new List<Claim>
            {
                new("http://schemas.microsoft.com/identity/claims/objectidentifier", user.Id.ToString() ?? ""),
                new("http://schemas.microsoft.com/identity/claims/tenantid", user.CustomerId.ToString()),
                new("ExternalId", user.ExternalId ?? string.Empty),
                new(ClaimTypes.Role, AccessesNames.AUTH_Direct),
                new("ValidFrom", DateTime.UtcNow.AddMinutes(-10).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                new("ValidTo", DateTime.UtcNow.AddHours(1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            await authService.AuditLogin(user.Id, null, ip, body.ClientVersion, true, clt);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "AuthenticateExternal");
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
            if (User.IsInRole(AccessesNames.AUTH_External))
            {
                return new CurrentUser
                {
                    IsAuthenticated = true,
                    Claims = User.Claims.Select(claim => new KeyValuePair<string, string>(claim.Type, claim.Value)).ToList(),
                };
            }

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
            {
                _logger.LogInformation("Refresh request, but user is not authenticated");
                return false;
            }

            if (User.IsInRole(AccessesNames.AUTH_External))
            {
                _logger.LogInformation("Refresh request for external user, but external will expire");
                await HttpContext.SignOutAsync();
                return false;
            }

            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));

            var oldRefreshToken = User?.FindFirstValue(REFRESHTOKEN);
            if (oldRefreshToken == null)
            {
                _logger.LogInformation("Refresh request, but old refresh token is null for user `{userId}` in customer `{customerId}`", userId, customerId);
                await HttpContext.SignOutAsync();
                return false;
            }

            var authService = GetAuthenticationService();
            var supResult = await authService.Refresh(oldRefreshToken, clt);
            if (supResult.Success is not true || supResult.JwtSecurityToken is null)
            {
                _logger.LogInformation("Refresh request, but refresh failed for user `{userId}` in customer `{customerId}`", userId, customerId);
                await HttpContext.SignOutAsync();
                return false;
            }

            _logger.LogInformation("Refresh for user `{userId}` in customer `{customerId}` successful", userId, customerId);
            await SetUser(supResult, false, "refresh", clt);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Refresh");
            await HttpContext.SignOutAsync();
            return false;
        }
    }

    private async Task SetUser(AuthenticateUserResult subResult, bool rememberMe, string clientVersion, CancellationToken clt)
    {
        var claims = await GetClaimsList(subResult, clientVersion, clt);

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

    private async Task<IEnumerable<Claim>> GetClaimsList(AuthenticateUserResult subResult, string clientVersion, CancellationToken clt)
    {
        var authService = GetAuthenticationService();
        var drogeClaims = authService.GetClaims(subResult);
        var customerId = await GetCustomerIdByExternalId(drogeClaims.TenantId, clt);
        var userId = await GetUserIdByExternalId(drogeClaims.ExternalUserId, drogeClaims.FullName, drogeClaims.Email, customerId, clt);
        var ip = GetRequesterIp();
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, drogeClaims.Email),
            new("FullName", drogeClaims.FullName),
            new("ExternalUserId", drogeClaims.ExternalUserId),
            new("login_hint", drogeClaims.LoginHint),
            new(REFRESHTOKEN, subResult.RefreshToken),
            new("idToken", drogeClaims.IdToken),
            new("ValidFrom", subResult.JwtSecurityToken!.ValidFrom.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
            new("ValidTo", subResult.JwtSecurityToken!.ValidTo.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
            new("IdentityProvider", authService.IdentityProvider.ToString()),
            new("http://schemas.microsoft.com/identity/claims/objectidentifier", userId.ToString()),
            new("http://schemas.microsoft.com/identity/claims/tenantid", customerId.ToString())
        };
        var superUsers = _configuration.GetSection("DrogeCode:SuperAdmin").Get<List<Guid>>();
        if (superUsers is not null && superUsers.Contains(userId))
        {
            claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_super_user));
            claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_configure_user_roles));
            claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_basic_access));
            claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_configure_global_all));
        }

        var accesses = await _userRoleService.GetAccessForUser(userId, customerId, subResult.JwtSecurityToken.Claims, clt);
        foreach (var access in accesses)
        {
            claims.Add(new Claim(ClaimTypes.Role, access));
        }

        await authService.AuditLogin(userId, null, ip, clientVersion, false, clt);
        return claims;
    }

    private async Task<Guid> GetCustomerIdByExternalId(string tenantId, CancellationToken clt)
    {
        var user = await _customerSettingService.GetByTenantId(tenantId, clt);
        if (user is null)
        {
            _logger.LogWarning("Failed to get or set user by external id");
            throw new UnauthorizedAccessException();
        }

        return user.Id;
    }

    private async Task<Guid> GetUserIdByExternalId(string externalUserId, string userName, string userEmail, Guid customerId, CancellationToken clt)
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