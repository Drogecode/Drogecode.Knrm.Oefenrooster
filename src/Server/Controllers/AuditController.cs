using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
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
    [Authorize(Roles = AccessesNames.AUTH_Taco)]
    [Route("training/{id:guid}/{count:int}/{skip:int}")]
    public async Task<ActionResult<GetTrainingAuditResponse>> GetTrainingAudit(Guid id, int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 50)
            {
                _logger.LogWarning("GetAllFutureDayItems count to big {0}", count);
                return BadRequest("Count to big");
            }
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            GetTrainingAuditResponse result = await _auditService.GetTrainingAudit(customerId, userId, count, skip, id, clt);
            return result;
        }
        catch (OperationCanceledException) { return Ok(); }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetTrainingAudit");
            return BadRequest();
        }
    }

    [HttpGet]
    [Authorize(Roles = AccessesNames.AUTH_Taco)]
    [Route("training/{count:int}/{skip:int}")]
    public async Task<ActionResult<GetTrainingAuditResponse>> GetAllTrainingsAudit(int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count >= 50)
            {
                _logger.LogWarning("GetAllFutureDayItems count to big {0}", count);
                return BadRequest("Count to big");
            }
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            GetTrainingAuditResponse result = await _auditService.GetTrainingAudit(customerId, userId, count, skip, Guid.Empty, clt);
            return result;
        }
        catch (OperationCanceledException) { return Ok(); }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetTrainingAudit");
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("log/{message}")]
    public async Task<ActionResult> PostLog(string message)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            _logger.LogInformation("Message from user `{userId}` `{customerId}` message: `{message}`",userId, customerId, message.Replace(Environment.NewLine, ""));
            return Ok();
        }
        catch (OperationCanceledException) { return Ok(); }
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
