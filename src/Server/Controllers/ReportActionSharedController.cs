using System.Diagnostics;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportActionShared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "ReportActionShared")]
public class ReportActionSharedController : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger;
    private readonly IReportActionSharedService _reportActionSharedService;

    public ReportActionSharedController(
        ILogger<ScheduleController> logger,
        IReportActionSharedService reportActionSharedService)
    {
        _logger = logger;
        _reportActionSharedService = reportActionSharedService;
    }

    [HttpPut]
    [Route("")]
    public async Task<ActionResult<PutReportActionSharedResponse>> PutReportActionShared([FromBody] ReportActionSharedConfiguration body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _reportActionSharedService.PutReportActionShared(body, customerId, userId, clt);
            return result;
        }
        catch (OperationCanceledException)
        {
            return Ok();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PutNewReportActionShared");
            return BadRequest();
        }
    }
}