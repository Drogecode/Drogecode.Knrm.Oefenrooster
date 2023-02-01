using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpGet]
    public async Task<List<DrogeUser>> GetAll()
    {
        var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
        var result = await _userService.GetAllUsers(customerId);

        return result;
    }

    [HttpGet]
    public async Task<DrogeUser> Get()
    {
        var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
        var userName = User?.FindFirstValue("name") ?? throw new Exception("No userName found");
        var userEmail = User?.FindFirstValue("preferred_username") ?? throw new Exception("No userEmail found");
        var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
        var result = await _userService.GetOrSetUserFromDb(userId, userName, userEmail, customerId);

        return result;
    }

    [HttpPost]
    public async Task<bool> AddUser(DrogeUser user)
    {
        var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
        var userName = User?.FindFirstValue("name") ?? throw new Exception("No userName found");
        var userEmail = User?.FindFirstValue("preferred_username") ?? throw new Exception("No userEmail found");
        var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
        user.Id = Guid.NewGuid();
        var result = await _userService.AddUser(user, customerId);
        return result;
    }

    [HttpPost]
    public async Task<bool> UpdateUser(DrogeUser user)
    {
        var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
        var userName = User?.FindFirstValue("name") ?? throw new Exception("No userName found");
        var userEmail = User?.FindFirstValue("preferred_username") ?? throw new Exception("No userEmail found");
        var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
        var result = await _userService.UpdateUser(user, userId, userName, userEmail, customerId);

        return result;
    }
}
