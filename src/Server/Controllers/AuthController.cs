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

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Authentication")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }
    /*private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }*/

    /*[HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null) return BadRequest("User does not exist");
        var singInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!singInResult.Succeeded) return BadRequest("Invalid password");
        await _signInManager.SignInAsync(user, request.RememberMe);
        return Ok();
    }*/

    [HttpPost]
    [Route("logincallback")]
    public async Task Logincallback([FromForm] string id_token, [FromForm] string state, [FromForm] string session_state, CancellationToken clt = default)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(id_token);
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
            return BadRequest($"Failed: {e.Message}");
        }
    }

    private async Task SetUser(JwtSecurityToken jwtSecurityToken, bool rememberMe, CancellationToken clt)
    {
        var claims = GetClaimsList(jwtSecurityToken);

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

    public static IEnumerable<Claim> GetClaimsList(JwtSecurityToken jwtSecurityToken)
    {
        var fullName = $"durp";
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "dam"),
            new("FullName", fullName),
            new("http://schemas.microsoft.com/identity/claims/objectidentifier", jwtSecurityToken.Claims.FirstOrDefault(x=>x.Type == "oid").Value),
        };
        /*if (userLoginResponse.Roles == null) return claims;
        foreach (var roleString in userLoginResponse.Roles.Select(role => role.ToString()).Where(roleString => !claims.Any(c => c.Type == ClaimTypes.Role && c.Value == roleString)))
        {
            claims.Add(new Claim(ClaimTypes.Role, roleString));
        }*/

        return claims;
    }

    /*[HttpPost]
    public async Task<IActionResult> Register(RegisterRequest parameters)
    {
        var user = new ApplicationUser();
        user.UserName = parameters.UserName;
        var result = await _userManager.CreateAsync(user, parameters.Password);
        if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
        return await Login(new LoginRequest
        {
            UserName = parameters.UserName,
            Password = parameters.Password
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [HttpGet]
    public CurrentUser CurrentUserInfo()
    {
        return new CurrentUser
        {
            IsAuthenticated = User.Identity.IsAuthenticated,
            UserName = User.Identity.Name,
            Claims = User.Claims
            .ToDictionary(c => c.Type, c => c.Value)
        };
    }*/
}
