using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "Holiday")]
public class HolidayController : ControllerBase
{
    private readonly ILogger<HolidayController> _logger;
    private readonly IHolidayService _holidayService;
    private readonly IAuditService _auditService;

    public HolidayController(
        ILogger<HolidayController> logger,
        IHolidayService holidayService,
        IAuditService auditService)
    {
        _logger = logger;
        _holidayService = holidayService;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<ActionResult<MultipleHolidaysResponse>> GetAll(CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            List<Holiday> result = await _holidayService.GetAllHolidaysForUser(customerId, userId);

            return Ok(new MultipleHolidaysResponse { Holidays = result });
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetAll Holidays");
            return BadRequest();
        }
    }

    [HttpPut]
    public async Task<ActionResult<PutHolidaysForUserResponse>> PutHolidayForUser([FromBody] Holiday body)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            PutHolidaysForUserResponse result = await _holidayService.PutHolidaysForUser(body, customerId, userId);

            return Ok(result);
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PutHolidaysForUser");
            return BadRequest();
        }
    }

    [HttpPatch]
    public async Task<ActionResult<PatchHolidaysForUserResponse>> PatchHolidayForUser([FromBody] Holiday body)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            PatchHolidaysForUserResponse result = await _holidayService.PatchHolidaysForUser(body, customerId, userId);

            return Ok(result);
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PatchHolidayForUser");
            return BadRequest();
        }
    }
}
