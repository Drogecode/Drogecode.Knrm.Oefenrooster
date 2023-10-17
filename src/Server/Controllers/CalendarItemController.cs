using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "CalendarItem")]
public class CalendarItemController : ControllerBase
{
    private readonly ILogger<CalendarItemController> _logger;
    private readonly IConfiguration _configuration;
    private readonly ICalendarItemService _calendarItemService;
    private readonly IAuditService _auditService;
    private readonly IUserSettingService _userSettingService;
    private readonly IGraphService _graphService;

    public CalendarItemController(
        ILogger<CalendarItemController> logger,
        IConfiguration configuration,
        ICalendarItemService calendarItemService,
        IAuditService auditService,
        IUserSettingService userSettingService,
        IGraphService graphService)
    {
        _logger = logger;
        _configuration = configuration;
        _calendarItemService = calendarItemService;
        _auditService = auditService;
        _userSettingService = userSettingService;
        _graphService = graphService;
    }

    [HttpGet]
    [Route("month/{year:int}/{month:int}")]
    public async Task<ActionResult<GetMonthItemResponse>> GetMonthItems(int year, int month, CancellationToken token = default)
    {
        try
        {
            var result = new GetMonthItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            result = await _calendarItemService.GetMonthItems(year, month, customerId, token);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetMonthItem");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("day/{yearStart:int}/{monthStart:int}/{dayStart:int}/{yearEnd:int}/{monthEnd:int}/{dayEnd:int}")]
    public async Task<ActionResult<GetDayItemResponse>> GetDayItems(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, Guid userId, CancellationToken token = default)
    {
        try
        {
            var result = new GetDayItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            result = await _calendarItemService.GetDayItems(yearStart, monthStart, dayStart, yearEnd, monthEnd, dayEnd, customerId, userId, token);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetDayItems");
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("month")]
    public async Task<ActionResult<PutMonthItemResponse>> PutMonthItem([FromBody] RoosterItemMonth roosterItemMonth, CancellationToken token = default)
    {
        try
        {
            var result = new PutMonthItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            result = await _calendarItemService.PutMonthItem(roosterItemMonth, customerId, userId, token);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in PutMonthtem");
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("day")]
    public async Task<ActionResult<PutDayItemResponse>> PutDayItem([FromBody] RoosterItemDay roosterItemDay, CancellationToken clt = default)
    {
        try
        {
            var result = new PutDayItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            result = await _calendarItemService.PutDayItem(roosterItemDay, customerId, userId, clt);
            if (roosterItemDay.UserId is not null && !roosterItemDay.UserId.Equals(Guid.Empty))
            {
                await ToOutlookCalendar(roosterItemDay.UserId.Value, true, roosterItemDay, customerId, null, clt);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in PutDayItem");
            return BadRequest();
        }
    }



    private async Task ToOutlookCalendar(Guid planUserId, bool assigned, RoosterItemDay roosterItemDay, Guid customerId, string? calendarEventId, CancellationToken clt)
    {
        if (assigned && await _userSettingService.TrainingToCalendar(customerId, planUserId))
        {
            _graphService.InitializeGraph();
            if (string.IsNullOrEmpty(calendarEventId))
            {
                var eventResult = await _graphService.AddToCalendar(planUserId, roosterItemDay.Text, roosterItemDay!.DateStart, roosterItemDay.DateEnd, roosterItemDay.IsFullDay);
            }
            else
            {
                await _graphService.PatchCalender(planUserId, calendarEventId, roosterItemDay.Text, roosterItemDay!.DateStart, roosterItemDay.DateEnd, roosterItemDay.IsFullDay);
                //await _auditService.Log(currentUserId, AuditType.PatchTraining, customerId, $"Preventing duplicate event '{type?.TrainingType?.Name}' on '{training?.DateStart.ToString("o")}' : '{training?.DateEnd.ToString("o")}'");
            }
        }
        else if (!string.IsNullOrEmpty(calendarEventId))
        {
            _graphService.InitializeGraph();
            await _graphService.DeleteCalendarEvent(planUserId, calendarEventId, clt);
        }
    }
}
