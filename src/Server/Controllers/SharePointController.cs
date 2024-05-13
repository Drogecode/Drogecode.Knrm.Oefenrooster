using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
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
[ApiExplorerSettings(GroupName = "SharePoint")]
public class SharePointController : ControllerBase
{
    private readonly ILogger<SharePointController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IAuditService _auditService;
    private readonly IGraphService _graphService;

    public SharePointController(
        ILogger<SharePointController> logger,
        IConfiguration configuration,
        IAuditService auditService,
        IGraphService graphService)
    {
        _logger = logger;
        _configuration = configuration;
        _auditService = auditService;
        _graphService = graphService;
    }

    [HttpGet]
    [Route("training/user/{count:int}/{skip:int}")]
    public async Task<ActionResult<MultipleSharePointTrainingsResponse>> GetLastTrainingsForCurrentUser(int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 30) return Forbid();
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var users = new List<Guid>() { userId };

            _graphService.InitializeGraph();
            var result = await _graphService.GetListTrainingUser(users, userId, count, skip, customerId, clt);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastTrainingsForCurrentUser");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("training/{users}/{count:int}/{skip:int}")]
    public async Task<ActionResult<MultipleSharePointTrainingsResponse>> GetLastTrainings(string users, int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 30) return Forbid();
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var usersAsList = System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(users);
            if (usersAsList is null)
                return BadRequest("users is null");

            _graphService.InitializeGraph();
            var result = await _graphService.GetListTrainingUser(usersAsList, userId, count, skip, customerId, clt);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastTrainings");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("action/user/{count:int}/{skip:int}")]
    public async Task<ActionResult<MultipleSharePointActionsResponse>> GetLastActionsForCurrentUser(int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 30) return Forbid();
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var users = new List<Guid>() { userId };

            _graphService.InitializeGraph();
            var result = await _graphService.GetListActionsUser(users, userId, count, skip, customerId, clt);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastActionsForCurrentUser");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("action/{users}/{count:int}/{skip:int}")]
    public async Task<ActionResult<MultipleSharePointActionsResponse>> GetLastActions(string users, int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 30) return Forbid();
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var usersAsList = System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(users);
            if (usersAsList is null)
                return BadRequest("users is null");

            _graphService.InitializeGraph();
            var result = await _graphService.GetListActionsUser(usersAsList, userId, count, skip, customerId, clt);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastActions");
            return BadRequest();
        }
    }
}
