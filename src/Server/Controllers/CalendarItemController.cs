using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class CalendarItemController : ControllerBase
{
    private readonly ILogger<CalendarItemController> _logger;
    private readonly IConfiguration _configuration;
    private readonly ICalendarItemService _calendarItemService;
    private readonly IAuditService _auditService;

    public CalendarItemController(
        ILogger<CalendarItemController> logger,
        IConfiguration configuration,
        ICalendarItemService calendarItemService,
        IAuditService auditService)
    {
        _logger = logger;
        _configuration = configuration;
        _calendarItemService = calendarItemService;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<ActionResult<GetMonthItemResponse>> GetMonthItem(int year, int month, CancellationToken token = default)
    {
        try
        {
            var result = new GetMonthItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            result = await _calendarItemService.GetMonthItem(year, month, customerId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetMonthItem");
            return BadRequest();
        }
    }
}
