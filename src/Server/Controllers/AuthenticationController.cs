using Drogecode.Knrm.Oefenrooster.Server.Controllers.Abstract;
using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Server.Managers.Interfaces;
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
    private readonly IUserLinkCustomerService _userLinkCustomerService;
    private readonly ICustomerService _customerService;
    private readonly IReportActionSharedService _reportActionSharedService;
    private readonly IAuthenticationManager _authenticationManager;
    private readonly IConfiguration _configuration;
    private IAuthenticationService? _authService;

    private const string REFRESHTOKEN = "RefreshToken";

    public AuthenticationController(
        ILogger<AuthenticationController> logger,
        IMemoryCache memoryCache,
        IUserRoleService userRoleService,
        IUserService userService,
        IUserLinkCustomerService userLinkCustomerService,
        ICustomerService customerService,
        IReportActionSharedService reportActionSharedService,
        IAuthenticationManager authenticationManager,
        IConfiguration configuration) : base(logger)
    {
        _memoryCache = memoryCache;
        _userRoleService = userRoleService;
        _userService = userService;
        _userLinkCustomerService = userLinkCustomerService;
        _customerService = customerService;
        _reportActionSharedService = reportActionSharedService;
        _authenticationManager = authenticationManager;
        _configuration = configuration;
    }

    [HttpGet]
    [Route("login-secrets")]
    public async Task<ActionResult<GetLoginSecretsResponse>> GetLoginSecrets()
    {
        try
        {
            var authService = _authenticationManager.GetAuthenticationService();
            return await authService.GetLoginSecrets();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "GetLoginSecrets");
            return BadRequest();
        }
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
                Logger.LogWarning("found?.success = `{false}` || found?.CodeVerifier = `{null}`", found?.Success is not true, found?.CodeVerifier is null);
                return false;
            }

            _memoryCache.Remove(body.State);

            var authService = _authenticationManager.GetAuthenticationService();
            var supResult = await authService.AuthenticateUser(found, body.Code, body.State, body.SessionState, body.RedirectUrl, clt);
            if (supResult.Success is not true || supResult.JwtSecurityToken is null)
                return false;

            await SetUser(supResult, false, body.ClientVersion, clt);
            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "AuthenticateUser POST");
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
                Logger.LogInformation("AuthenticateExternal request, but user is already authenticated");
                return false;
            }

            var passwordCorrect = await _reportActionSharedService.AuthenticateExternal(body, clt);
            if (!passwordCorrect.Success) return false;
            var authService = _authenticationManager.GetAuthenticationService();
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
            Logger.LogError(e, "AuthenticateExternal");
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
            Logger.LogError(e, "CurrentUserInfo");
            return BadRequest();
        }
    }

    [Authorize]
    [HttpPatch]
    [Route("switch")]
    public async Task<ActionResult<bool>> SwitchUser([FromBody] SwitchUserRequest body, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var refreshToken = User?.FindFirstValue(REFRESHTOKEN) ?? throw new Exception("REFRESHTOKEN not found");
            var idToken = User?.FindFirstValue("idToken") ?? throw new Exception("idToken not found");
            var linkedUsers = await _userLinkCustomerService.GetAllLinkUserCustomers(userId, customerId, clt);
            var inSuperUserRole = User.IsInRole(AccessesNames.AUTH_super_user);
            if (!linkedUsers.Success || linkedUsers.UserLinkedCustomers?.Count < 2)
            {
                Logger.LogWarning("Not enough linked users found for {user} : {success} : {count}", userId, linkedUsers.Success, linkedUsers.UserLinkedCustomers?.Count);
                return false;
            }

            if (linkedUsers.UserLinkedCustomers?.Any(x => x.UserId == body.UserId) != true)
            {
                Logger.LogWarning("User {user} is trying to switch to {linkedUser} but that is not linked.", userId, body.UserId);
                return false;
            }

            var linkedUser = await _userService.GetUserById(body.CustomerId, body.UserId, true, clt);
            if (linkedUser is null)
            {
                Logger.LogWarning("Linked user {linkedUser} not found for {user}", body.UserId, userId);
                return false;
            }

            var authService = _authenticationManager.GetAuthenticationService();
            var ip = GetRequesterIp();
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, linkedUser.Email ?? ""),
                new("FullName", linkedUser.Name),
                new(REFRESHTOKEN, refreshToken),
                new("idToken", idToken),
                new("http://schemas.microsoft.com/identity/claims/objectidentifier", linkedUser.Id.ToString() ?? ""),
                new("http://schemas.microsoft.com/identity/claims/tenantid", linkedUser.CustomerId.ToString()),
                new("ExternalId", linkedUser.ExternalId ?? string.Empty),
                new("ValidFrom", DateTime.UtcNow.AddMinutes(-10).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                new("ValidTo", DateTime.UtcNow.AddHours(1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
            };

            var userClaims = await _userRoleService.GetAccessForUserByUserId(linkedUser.Id, linkedUser.CustomerId, clt);
            foreach (var userClaim in userClaims)
            {
                claims.Add(new Claim(ClaimTypes.Role, userClaim));
            }

            if (inSuperUserRole)
            {
                AddSuperUserRoles(claims);
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            await authService.AuditLogin(body.UserId, null, ip, "SwitchUser", true, clt);
            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "SwitchUser");
            return false;
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
    [Obsolete("Use RefreshUser")] // ToDo Remove when all users on v0.5.20 or above
    [Route("refresh")]
    public async Task<ActionResult<bool>> Refresh(CancellationToken clt = default)
    {
        var refresh = (await RefreshUser(clt)).Value;
        switch (refresh?.State)
        {
            case RefreshState.CurrentAuthenticationExpired:
            case RefreshState.CurrentAuthenticationValid:
            case RefreshState.NotAuthenticated:
            case RefreshState.NoRefreshToken:
            case RefreshState.None:
                return false;
            case RefreshState.AuthenticationRefreshed:
                return false; // Should be true, but disabled for now
            case null:
            default:
                return false;
        }
    }

    [Authorize]
    [HttpPost]
    [Route("refresh-user")]
    public async Task<ActionResult<RefreshResponse>> RefreshUser(CancellationToken clt = default)
    {
        var response = new RefreshResponse()
        {
            Success = false
        };
        try
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                Logger.LogInformation("Refresh request, but user is not authenticated");
                response.State = RefreshState.NotAuthenticated;
                return response;
            }

            if (User.IsInRole(AccessesNames.AUTH_External))
            {
                Logger.LogInformation("Refresh request for external user, but external will expire");
                await HttpContext.SignOutAsync();
                response.State = RefreshState.CurrentAuthenticationExpired;
                return response;
            }

            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));

            var oldRefreshToken = User?.FindFirstValue(REFRESHTOKEN);
            if (oldRefreshToken == null)
            {
                Logger.LogInformation("Refresh request, but old refresh token is null for user `{userId}` in customer `{customerId}`", userId, customerId);
                await HttpContext.SignOutAsync();
                response.State = RefreshState.NoRefreshToken;
                return response;
            }

            var authService = _authenticationManager.GetAuthenticationService();
            var supResult = await authService.Refresh(oldRefreshToken, clt);
            if (supResult.Success is not true || supResult.JwtSecurityToken is null)
            {
                Logger.LogInformation("Refresh request, but refresh failed for user `{userId}` in customer `{customerId}`", userId, customerId);
                await HttpContext.SignOutAsync();
                response.State = RefreshState.RefreshFailed;
                return response;
            }

            Logger.LogInformation("Refresh for user `{userId}` in customer `{customerId}` successful", userId, customerId);
            await SetUser(supResult, false, "refresh", clt);
            response.Success = true;
            response.ForceRefresh = false; // Set to true to enable.
            response.State = RefreshState.AuthenticationRefreshed;
            return response;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Refresh");
            await HttpContext.SignOutAsync();
            response.State = RefreshState.None;
            return response;
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
        var authService = _authenticationManager.GetAuthenticationService();
        var drogeClaims = authService.GetClaims(subResult);
        var customerId = await GetCustomerIdByExternalId(drogeClaims.TenantId, subResult.JwtSecurityToken?.Claims, clt);
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
            AddSuperUserRoles(claims);
        }

        var accesses = await _userRoleService.GetAccessForUserByClaims(userId, customerId, subResult.JwtSecurityToken.Claims, clt);
        foreach (var access in accesses)
        {
            claims.Add(new Claim(ClaimTypes.Role, access));
        }

        await authService.AuditLogin(userId, null, ip, clientVersion, false, clt);
        return claims;
    }

    private static void AddSuperUserRoles(List<Claim> claims)
    {
        claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_super_user));
        claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_configure_user_roles));
        claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_basic_access));
        claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_configure_global_all));
        claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_users_details));
        claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_users_add_role));
        claims.Add(new Claim(ClaimTypes.Role, AccessesNames.AUTH_users_settings));
    }

    private async Task<Guid> GetCustomerIdByExternalId(string tenantId, IEnumerable<Claim>? claims, CancellationToken clt)
    {
        var customers = await _customerService.GetByTenantId(tenantId, clt);
        if (customers is null || customers.Count == 0)
        {
            Logger.LogWarning("Failed to get or set customers by external id");
            throw new UnauthorizedAccessException();
        }

        if (claims is null)
        {
            Logger.LogWarning("No claims while authorization.");
            throw new UnauthorizedAccessException();
        }

        if (customers.Count == 1)
        {
            return customers.First().Id;
        }
        
        foreach (var claim in claims.Where(x => x.Type.Equals("groups")))
        {
            if (customers.Any(x => x.GroupId?.Equals( claim.Value) == true))
            {
                return customers.First(x => x.GroupId?.Equals(claim.Value) == true).Id;
            }
        }
        
        Logger.LogWarning("Failed to link user to customer");
        throw new UnauthorizedAccessException();
    }

    private async Task<Guid> GetUserIdByExternalId(string externalUserId, string userName, string userEmail, Guid customerId, CancellationToken clt)
    {
        var user = await _userService.GetOrSetUserById(null, externalUserId, userName, userEmail, customerId, true, clt);
        if (user is null)
        {
            Logger.LogWarning("Failed to get or set user by external id");
            throw new UnauthorizedAccessException();
        }

        return user.Id;
    }
}