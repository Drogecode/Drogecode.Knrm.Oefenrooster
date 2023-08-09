using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "User")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IAuditService _auditService;
    private readonly IGraphService _graphService;

    public UserController(ILogger<UserController> logger, IUserService userService, IAuditService auditService, IGraphService graphService)
    {
        _logger = logger;
        _userService = userService;
        _auditService = auditService;
        _graphService = graphService;
    }

    [HttpGet]
    [Route("all/{includeHidden:bool}")]
    public async Task<ActionResult<MultipleDrogeUsersResponse>> GetAll(bool includeHidden, CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _userService.GetAllUsers(customerId, includeHidden);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAll");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("me")]
    public async Task<ActionResult<GetDrogeUserResponse>> GetCurrentUser(CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var userName = User?.FindFirstValue("FullName") ?? throw new Exception("No userName found");
            var userEmail = User?.FindFirstValue(ClaimTypes.Name) ?? throw new Exception("No userEmail found");
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _userService.GetOrSetUserFromDb(userId, userName, userEmail, customerId, true);

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
            var result = await _userService.GetUserFromDb(id);
            return new GetByIdResponse { User = result, Success = true };
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Exception in GetById");
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles = AccessesNames.AUTH_Taco)]
    public async Task<ActionResult<AddUserResponse>> AddUser([FromBody] DrogeUser user, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
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

    [HttpPatch]
    [Route("")]
    [Authorize(Roles = AccessesNames.AUTH_Taco)]
    public async Task<ActionResult<UpdateUserResponse>> UpdateUser([FromBody] DrogeUser user, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var userName = User?.FindFirstValue("FullName") ?? throw new Exception("No userName found");
            var userEmail = User?.FindFirstValue(ClaimTypes.Name) ?? throw new Exception("No userEmail found");
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _userService.UpdateUser(user, userId, userName, userEmail, customerId);

            return Ok(new UpdateUserResponse { Success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpdateUser");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("sync")]
    [Authorize(Roles = AccessesNames.AUTH_Taco)]
    public async Task<ActionResult<SyncAllUsersResponse>> SyncAllUsers(CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            _graphService.InitializeGraph();
            var existingUsers = (await _userService.GetAllUsers(customerId, true)).DrogeUsers;
            var users = await _graphService.ListUsersAsync();
            if (users?.Value != null)
            {
                await _auditService.Log(userId, AuditType.SyncAllUsers, customerId);
                while (true)
                {
                    foreach (var user in users!.Value!)
                    {
                        if (user.Id != null && user.DisplayName != null && user.Mail != null)
                        {
                            var id = new Guid(user.Id);
                            var index = existingUsers.FindIndex(x => x.Id == id);
                            if (index != -1)
                                existingUsers.RemoveAt(index);
                            var groups = await _graphService.GetGroupForUser(user.Id);
                            if (groups?.Value != null)
                            {
                                //ToDo
                            }
                            var newUserResponse = await _userService.GetOrSetUserFromDb(id, user.DisplayName, user.Mail, customerId, false);
                        }
                    }
                    if (users.OdataNextLink != null)
                        users = await _graphService.NextUsersPage(users);
                    else break;
                }
                if (existingUsers.Count > 0)
                {
                    await _userService.MarkUsersDeleted(existingUsers, userId, customerId);
                }
            }
            else
                return Ok(new SyncAllUsersResponse { Success = false });
            return Ok(new SyncAllUsersResponse { Success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in SyncAllUsers");
            return Ok(new SyncAllUsersResponse { Success = false });
        }
    }
}
