using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
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

    [HttpPatch]
    [Route("historical")]
    [Authorize(Roles = AccessesNames.AUTH_super_user)]
    public async Task<ActionResult<GetHistoricalResponse>> SyncHistorical(CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));

            _graphService.InitializeGraph();
            var result = await _graphService.SyncHistorical(customerId, clt);
            return Ok(result);
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in SyncHistorical");
            return BadRequest();
        }
    }
}