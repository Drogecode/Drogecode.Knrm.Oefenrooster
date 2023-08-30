using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "Audit")]
public class AuditController : ControllerBase
{
    private readonly ILogger<AuditController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IAuditService _auditService;

    public AuditController(
        ILogger<AuditController> logger,
        IConfiguration configuration,
        IAuditService auditService)
    {
        _logger = logger;
        _configuration = configuration;
        _auditService = auditService;
    }

    [HttpGet]
    [Route("training/{id:guid}")]
    public async Task<ActionResult<GetTrainingAuditResponse>> GetTrainingAudit(Guid id)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            GetTrainingAuditResponse result = await _auditService.GetTrainingAudit(customerId, userId, id);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetTrainingAudit");
            return BadRequest();
        }
    }
}
