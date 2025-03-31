using Drogecode.Knrm.Oefenrooster.Server.Hubs;
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
    private readonly IUserRoleService _userRoleService;
    private readonly ILinkUserRoleService _linkUserRoleService;
    private readonly IAuditService _auditService;
    private readonly IGraphService _graphService;
    private readonly IFunctionService _functionService;
    private readonly RefreshHub _refreshHub;

    public UserController(ILogger<UserController> logger, IUserService userService, IUserRoleService userRoleService, ILinkUserRoleService linkUserRoleService, IAuditService auditService,
        IGraphService graphService, IFunctionService functionService, RefreshHub refreshHub)
    {
        _logger = logger;
        _userService = userService;
        _userRoleService = userRoleService;
        _linkUserRoleService = linkUserRoleService;
        _auditService = auditService;
        _graphService = graphService;
        _functionService = functionService;
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

            return new GetDrogeUserResponse { DrogeUser = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in Get");
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
            var result = await _userService.GetUserById(customerId, id, clt);
            return new GetByIdResponse { User = result, Success = true };
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

    [HttpPut]
    [Route("")]
    [Authorize(Roles = AccessesNames.AUTH_users_details)]
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

    [HttpPatch]
    [Route("link-user-user")]
    [Authorize(Roles = AccessesNames.AUTH_users_settigns)]
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
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            await _auditService.Log(userId, AuditType.SyncAllUsers, customerId);
            clt = CancellationToken.None;
            return Ok(await InternalSyncAllUsers(userId, customerId, clt));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in SyncAllUsers");
            return Ok(new SyncAllUsersResponse { Success = false });
        }
    }

    internal async Task<SyncAllUsersResponse> InternalSyncAllUsers(Guid userId, Guid customerId, CancellationToken clt = default)
    {
        _graphService.InitializeGraph();
        var existingUsers = (await _userService.GetAllUsers(customerId, true, false, clt)).DrogeUsers ?? [];
        var functions = (await _functionService.GetAllFunctions(customerId, clt)).Functions ?? [];
        var users = await _graphService.ListUsersAsync();
        if (users?.Value is not null)
        {
            while (true)
            {
                foreach (var user in users!.Value!)
                {
                    if (!string.IsNullOrEmpty(user.Id) && !string.IsNullOrEmpty(user.DisplayName))
                    {
                        var newUserResponse = await _userService.GetOrSetUserById(null, user.Id, user.DisplayName, user.Mail ?? "not set", customerId, false, clt);
                        if (newUserResponse is null)
                            continue;
                        if (!newUserResponse.IsNew)
                        {
                            var index = existingUsers.FindIndex(x => x.Id == newUserResponse.Id);
                            if (index != -1)
                                existingUsers.RemoveAt(index);
                        }

                        newUserResponse.SyncedFromSharePoint = true;
                        var groups = await _graphService.GetGroupForUser(user.Id);
                        if (groups?.Value is not null && functions.Any(x => groups.Value.Any(y => y.Id == x.RoleId.ToString())))
                        {
                            newUserResponse.RoleFromSharePoint = true;
                            var newFunction = functions.FirstOrDefault(x => groups.Value.Any(y => y.Id == x.RoleId.ToString()));
                            if (newFunction is not null && newUserResponse.UserFunctionId != newFunction.Id)
                            {
                                newUserResponse.UserFunctionId = newFunction.Id;
                            }
                        }
                        else
                        {
                            newUserResponse.RoleFromSharePoint = false;
                        }

                        var linkedRoles = await _linkUserRoleService.GetLinkUserRolesAsync(newUserResponse.Id, clt);
                        var roles = await _userRoleService.GetAll(customerId, clt);
                        if (groups?.Value is not null && roles.Roles is not null && roles.Roles.Count != 0)
                        {
                            foreach (var group in groups.Value.Where(x => roles.Roles.Any(y => y.ExternalId?.ToString().Equals(x.Id) == true)))
                            {
                                if (group.Id is null) continue;
                                var role = roles.Roles.FirstOrDefault(x => x.ExternalId?.Equals(group.Id) == true);
                                if (role is null) continue;
                                linkedRoles.Remove(role.Id);
                                await _linkUserRoleService.LinkUserToRoleAsync(newUserResponse.Id, role, true, true, clt);
                            }

                            foreach (var linkedRole in linkedRoles)
                            {
                                var role = roles.Roles.FirstOrDefault(x => x.Id.Equals(linkedRole));
                                if (role is null) continue;
                                await _linkUserRoleService.LinkUserToRoleAsync(newUserResponse.Id, role, false, true, clt);
                            }
                        }

                        await _userService.UpdateUser(newUserResponse, userId, customerId);
                    }
                }

                if (users.OdataNextLink is not null)
                    users = await _graphService.NextUsersPage(users);
                else break;
            }

            if (existingUsers?.Count > 0)
            {
                await _userService.MarkUsersDeleted(existingUsers, userId, customerId);
            }
        }
        else
            return new SyncAllUsersResponse { Success = false };

        return new SyncAllUsersResponse { Success = true };
    }
}