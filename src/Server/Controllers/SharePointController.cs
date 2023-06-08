using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
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
    public async Task<ActionResult<List<SharePointTraining>>> GetLastTrainingsForCurrentUser(int count, CancellationToken clt = default)
    {
        try
        {
            var userName = User?.FindFirstValue("name") ?? throw new Exception("No userName found");
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            _graphService.InitializeGraph();
            var result = await _graphService.GetListTrainingUser(userName, userId, count, customerId, clt);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastTrainingsForCurrentUser");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<SharePointAction>>> GetLastActionsForCurrentUser(int count, CancellationToken clt = default)
    {
        try
        {
            var userName = User?.FindFirstValue("name") ?? throw new Exception("No userName found");
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            _graphService.InitializeGraph();
            var result = await _graphService.GetListActionsUser(userName, userId, count, customerId, clt);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastActionsForCurrentUser");
            return BadRequest();
        }
    }
}
