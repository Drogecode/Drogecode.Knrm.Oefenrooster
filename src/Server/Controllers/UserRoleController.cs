using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "UserRole")]
public class UserRoleController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRoleService _userRoleService;
    private readonly ILinkUserRoleService _linkUserRoleService;
    private readonly RefreshHub _refreshHub;

    public UserRoleController(ILogger<UserController> logger, IUserRoleService userRoleService, ILinkUserRoleService linkUserRoleService, RefreshHub refreshHub)
    {
        _logger = logger;
        _userRoleService = userRoleService;
        _linkUserRoleService = linkUserRoleService;
        _refreshHub = refreshHub;
    }

    [HttpPost]
    [Authorize(Roles = AccessesNames.AUTH_configure_user_roles)]
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
    [Authorize(Roles = AccessesNames.AUTH_configure_user_roles)]
    [Route("all")]
    public async Task<ActionResult<MultipleDrogeUserRolesResponse>> GetAll(CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _userRoleService.GetAll(customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAll");
            return BadRequest();
        }
    }

    [HttpGet]
    [Authorize(Roles = AccessesNames.AUTH_configure_user_roles)]
    [Route("{id:guid}")]
    public async Task<ActionResult<GetUserRoleResponse>> GetById(Guid id, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _userRoleService.GetById(id, userId, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAll");
            return BadRequest();
        }
    }
    
    [HttpGet]
    [Authorize(Roles = AccessesNames.AUTH_configure_user_roles)]
    [Route("{id:guid}/users")]
    public async Task<ActionResult<GetLinkedUsersByIdResponse>> GetLinkedUsersById(Guid id, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            GetLinkedUsersByIdResponse result = await _linkUserRoleService.GetLinkedUsersById(id, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLinkedUsersById");
            return BadRequest();
        }
    }
    

    [HttpPatch]
    [Authorize(Roles = AccessesNames.AUTH_configure_user_roles)]
    [Route("")]
    public async Task<ActionResult<UpdateUserRoleResponse>> PatchUserRole([FromBody] DrogeUserRole userRole, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _userRoleService.PatchUserRole(userRole, userId, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in PatchUserRole");
            return BadRequest();
        }
    }
}