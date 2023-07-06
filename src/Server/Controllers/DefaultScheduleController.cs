using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
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
[ApiExplorerSettings(GroupName = "DefaultSchedule")]
public class DefaultScheduleController : ControllerBase
{
    private readonly ILogger<DefaultScheduleController> _logger;
    private readonly IDefaultScheduleService _defaultScheduleService;
    private readonly IAuditService _auditService;

    public DefaultScheduleController(
        ILogger<DefaultScheduleController> logger,
        IDefaultScheduleService defaultscheduleService,
        IAuditService auditService)
    {
        _logger = logger;
        _defaultScheduleService = defaultscheduleService;
        _auditService = auditService;
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult<MultipleDefaultSchedulesResponse>> GetAll(CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            List<DefaultSchedule> result = await _defaultScheduleService.GetAlldefaultsForUser(customerId, userId);

            return Ok(new MultipleDefaultSchedulesResponse { DefaultSchedules = result });
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetAll default schedules");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("")]
    public async Task<ActionResult<PatchDefaultScheduleForUserResponse>> PatchDefaultScheduleForUser([FromBody] DefaultSchedule body)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            PatchDefaultScheduleForUserResponse result = await _defaultScheduleService.PatchDefaultScheduleForUser(body, customerId, userId);

            return Ok(result);
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PatchDefaultScheduleForUser");
            return BadRequest();
        }
    }
}
