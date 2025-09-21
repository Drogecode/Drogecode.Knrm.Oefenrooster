using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Server.Managers.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "User")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IAuditService _auditService;
    private readonly IUserSyncManager _userSyncManager;
    private readonly RefreshHub _refreshHub;

    private bool _syncUsers;

    public UserController(ILogger<UserController> logger, IUserService userService, IAuditService auditService, IUserSyncManager userSyncManager, RefreshHub refreshHub)
    {
        _logger = logger;
        _userService = userService;
        _auditService = auditService;
        _userSyncManager = userSyncManager;
        _refreshHub = refreshHub;
    }

    [HttpGet]
    [Route("all/{includeHidden:bool}/{callHub:bool}")]
    public async Task<ActionResult<MultipleDrogeUsersResponse>> GetAll(bool includeHidden, bool callHub = false, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var includeLastLogin = User.IsInRole(AccessesNames.AUTH_super_user);
            if (includeHidden && includeLastLogin)
                await _userService.PatchLastOnline(userId, clt);
            var result = await _userService.GetAllUsers(customerId, includeHidden, includeLastLogin, clt);

            if (callHub)
            {
                _logger.LogTrace("Calling hub AllUsers");
                await _refreshHub.SendMessage(userId, ItemUpdated.AllUsers);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAll");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("all/customer/{customerId:guid}/{includeHidden:bool}")]
    [Authorize(Roles = AccessesNames.AUTH_super_user)]
    public async Task<ActionResult<MultipleDrogeUsersResponse>> GetAllDifferentCustomer(Guid customerId, bool includeHidden, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var includeLastLogin = User.IsInRole(AccessesNames.AUTH_super_user);
            if (includeHidden && includeLastLogin)
                await _userService.PatchLastOnline(userId, clt);
            var result = await _userService.GetAllUsers(customerId, includeHidden, includeLastLogin, clt);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAllDifferentCustomer");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("me")]
    public async Task<ActionResult<GetDrogeUserResponse>> GetCurrentUser(CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var userName = User?.FindFirstValue("FullName") ?? throw new Exception("No userName found");
            var userEmail = User?.FindFirstValue(ClaimTypes.Name) ?? throw new Exception("No userEmail found");
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _userService.GetOrSetUserById(userId, null, userName, userEmail, customerId, true, clt);

            var resposne = new GetDrogeUserResponse { DrogeUser = result };
            return resposne;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetCurrentUser");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<GetByIdResponse>> GetById(Guid id, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _userService.GetUserById(customerId, id, false, clt);
            return new GetByIdResponse { User = result, Success = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetById");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{id:guid}/roles")]
    public async Task<ActionResult<MultipleLinkedUserRolesResponse>> GetRolesForUserById(Guid id, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _userService.GeRolesForUserById(customerId, id, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetById");
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles = AccessesNames.AUTH_super_user)]
    public async Task<ActionResult<AddUserResponse>> AddUser([FromBody] DrogeUser user, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            user.Id = Guid.NewGuid();
            var result = await _userService.AddUser(user, customerId);
            await _auditService.Log(userId, AuditType.AddUser, customerId, objectKey: user.Id, objectName: user.Name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in AddUser");
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("{customerId:guid}")]
    [Authorize(Roles = AccessesNames.AUTH_super_user)]
    public async Task<ActionResult<AddUserResponse>> AddUserDifferentCustomer(Guid customerId, [FromBody] DrogeUser user, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            user.Id = Guid.NewGuid();
            var result = await _userService.AddUser(user, customerId);
            await _auditService.Log(userId, AuditType.AddUser, customerId, objectKey: user.Id, objectName: user.Name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in AddUser");
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("")]
    [Authorize(Roles = $"{AccessesNames.AUTH_users_details},{AccessesNames.AUTH_super_user}")]
    public async Task<ActionResult<UpdateUserResponse>> UpdateUser([FromBody] DrogeUser user, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _userService.UpdateUser(user, userId, customerId);

            return new UpdateUserResponse { Success = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpdateUser");
            return BadRequest();
        }
    }

    [HttpDelete]
    [Route("")]
    [Authorize(Roles = $"{AccessesNames.AUTH_users_delete},{AccessesNames.AUTH_super_user}")]
    public async Task<ActionResult<UpdateUserResponse>> DeleteUser([FromBody] DrogeUser user, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            await _userService.MarkUserDeleted(user, userId, customerId, false);
            var result = await _userService.SaveDb(clt) > 0;

            return new UpdateUserResponse { Success = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in DeleteUser");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("link-user-user")]
    [Authorize(Roles = AccessesNames.AUTH_users_settings)]
    public async Task<ActionResult<UpdateLinkUserUserForUserResponse>> UpdateLinkUserUserForUser([FromBody] UpdateLinkUserUserForUserRequest body, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            UpdateLinkUserUserForUserResponse result;
            if (body.Add)
                result = await _userService.UpdateLinkUserUserForUser(body, userId, customerId, clt);
            else
                result = await _userService.RemoveLinkUserUserForUser(body, userId, customerId, clt);
            await _auditService.Log(userId, AuditType.UpdateLinkUserUserForUser, customerId,
                JsonSerializer.Serialize(new AuditLinkUserUser
                { UserA = body.UserAId, UserB = body.UserBId, Add = body.Add, LinkType = body.LinkType, Success = result.Success, ElapsedMilliseconds = result.ElapsedMilliseconds }));
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpdateLinkUserUserForUser");
            return BadRequest();
        }
    }


    [HttpPatch]
    [Route("sync")]
    [Authorize(Roles = AccessesNames.AUTH_users_details)]
    public async Task<ActionResult<SyncAllUsersResponse>> SyncAllUsers(CancellationToken clt = default)
    {
        try
        {
            if (_syncUsers)
                return Ok(new SyncAllUsersResponse { Success = false });
            _syncUsers = true;
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            await _auditService.Log(userId, AuditType.SyncAllUsers, customerId);
            clt = CancellationToken.None;
            return Ok(await _userSyncManager.SyncAllUsers(userId, customerId, clt));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in SyncAllUsers");
            return Ok(new SyncAllUsersResponse { Success = false });
        }
        finally
        {
            _syncUsers = false;
        }
    }
}
