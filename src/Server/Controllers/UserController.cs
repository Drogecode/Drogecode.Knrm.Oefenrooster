using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;
using Tavis.UriTemplates;

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
    private readonly IGraphService _graphService;

    public UserController(ILogger<UserController> logger, IUserService userService, IAuditService auditService, IGraphService graphService)
    {
        _logger = logger;
        _userService = userService;
        _auditService = auditService;
        _graphService = graphService;
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
            _logger.LogError(ex, "Exception in GetAll");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<DrogeUser>> Get(CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var userName = User?.FindFirstValue("name") ?? throw new Exception("No userName found");
            var userEmail = User?.FindFirstValue("preferred_username") ?? throw new Exception("No userEmail found");
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _userService.GetOrSetUserFromDb(userId, userName, userEmail, customerId, true);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in Get");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult<bool>> AddUser(DrogeUser user, CancellationToken token = default)
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
            _logger.LogError(ex, "Exception in AddUser");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult<bool>> UpdateUser(DrogeUser user, CancellationToken token = default)
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
            _logger.LogError(ex, "Exception in UpdateUser");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<bool>> SyncAllUsers(CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var userName = User?.FindFirstValue("name") ?? throw new Exception("No userName found");
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            _graphService.InitializeGraph();
            var existingUsers = await _userService.GetAllUsers(customerId);
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
                return false;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in SyncAllUsers");
            return false;
        }
    }
}
