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
    private readonly IAuditService _auditService;

    public UserController(ILogger<UserController> logger, IUserService userService, IAuditService auditService)
    {
        _logger = logger;
        _userService = userService;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DrogeUser>>> GetAll()
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _userService.GetAllUsers(customerId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpgradeDatabase");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<DrogeUser>> Get()
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var userName = User?.FindFirstValue("name") ?? throw new Exception("No userName found");
            var userEmail = User?.FindFirstValue("preferred_username") ?? throw new Exception("No userEmail found");
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _userService.GetOrSetUserFromDb(userId, userName, userEmail, customerId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpgradeDatabase");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult<bool>> AddUser(DrogeUser user)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var userName = User?.FindFirstValue("name") ?? throw new Exception("No userName found");
            var userEmail = User?.FindFirstValue("preferred_username") ?? throw new Exception("No userEmail found");
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            user.Id = Guid.NewGuid();
            var result = await _userService.AddUser(user, customerId);
            await _auditService.Log(userId, AuditType.AddUser, customerId, objectKey: user.Id, objectName: user.Name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpgradeDatabase");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult<bool>> UpdateUser(DrogeUser user)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var userName = User?.FindFirstValue("name") ?? throw new Exception("No userName found");
            var userEmail = User?.FindFirstValue("preferred_username") ?? throw new Exception("No userEmail found");
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _userService.UpdateUser(user, userId, userName, userEmail, customerId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpgradeDatabase");
            return BadRequest();
        }
    }
}
