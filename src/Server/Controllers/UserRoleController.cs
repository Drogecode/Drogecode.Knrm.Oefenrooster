using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "UserRole")]
public class UserRoleController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRoleService _userRoleService;
    private readonly RefreshHub _refreshHub;

    public UserRoleController(ILogger<UserController> logger, IUserRoleService userRoleService, RefreshHub refreshHub)
    {
        _logger = logger;
        _userRoleService = userRoleService;
        _refreshHub = refreshHub;
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<NewUserRoleResponse>> NewUserRole([FromBody] DrogeUserRole userRole, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _userRoleService.NewUserRole(userRole, userId, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAll");
            return BadRequest();
        }
    }
    
    [HttpGet]
    [Route("all")]
    public async Task<ActionResult<MultipleDrogeUserRolesResponse>> GetAll(CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _userRoleService.GetAll(userId, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAll");
            return BadRequest();
        }
    }
}