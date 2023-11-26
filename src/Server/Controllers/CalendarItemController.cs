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
    public async Task<ActionResult<GetMultipleMonthItemResponse>> GetMonthItems(int year, int month, CancellationToken clt = default)
    {
        try
        {
            var result = new GetMultipleMonthItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            result = await _calendarItemService.GetMonthItems(year, month, customerId, clt);
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
    public async Task<ActionResult<GetMultipleDayItemResponse>> GetDayItems(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, Guid userId, CancellationToken clt = default)
    {
        try
        {
            var result = new GetMultipleDayItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            result = await _calendarItemService.GetDayItems(yearStart, monthStart, dayStart, yearEnd, monthEnd, dayEnd, customerId, userId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetDayItems");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("day/all/{count:int}/{skip:int}")]
    public async Task<ActionResult<GetMultipleDayItemResponse>> GetAllFutureDayItems(int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 50)
            {
                _logger.LogWarning("GetAllFutureDayItems count to big {0}", count);
                return BadRequest("Count to big");
            }
            var result = new GetMultipleDayItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            result = await _calendarItemService.GetAllFutureDayItems(customerId, count, skip, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAllFutureDayItems");
            return BadRequest();
        }
    }
    [HttpGet]
    [Route("day/{id:guid}")]
    public async Task<ActionResult<GetDayItemResponse>> GetDayItemById(Guid id, CancellationToken clt = default)
    {

        try
        {
            var result = new GetDayItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            result = await _calendarItemService.GetDayItemById(customerId, id, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetDayItemById");
            return BadRequest();
        }
    }


    [HttpPut]
    [Route("month")]
    public async Task<ActionResult<PutMonthItemResponse>> PutMonthItem([FromBody] RoosterItemMonth roosterItemMonth, CancellationToken clt = default)
    {
        try
        {
            var result = new PutMonthItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            result = await _calendarItemService.PutMonthItem(roosterItemMonth, customerId, userId, clt);
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
            /* ToDo
            if (roosterItemDay.UserIds is not null)
            {
                foreach (var user in roosterItemDay.UserIds)
                    await ToOutlookCalendar(userId, true, roosterItemDay, customerId, null, clt);
            }
            */
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in PutDayItem");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("day")]
    public async Task<ActionResult<PatchDayItemResponse>> PatchDayItem([FromBody] RoosterItemDay roosterItemDay, CancellationToken clt = default)
    {
        try
        {
            var result = new PatchDayItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            result = await _calendarItemService.PatchDayItem(roosterItemDay, customerId, userId, clt);
            /* ToDo
            if (roosterItemDay.UserIds is not null)
            {
                foreach (var user in roosterItemDay.UserIds)
                    await ToOutlookCalendar(userId, true, roosterItemDay, customerId, null, clt);
            }
            */
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in PatchDayItem");
            return BadRequest();
        }
    }



    private async Task ToOutlookCalendar(Guid planUserId, bool assigned, RoosterItemDay roosterItemDay, Guid customerId, string? calendarEventId, CancellationToken clt)
    {
        if (roosterItemDay?.DateStart is null || roosterItemDay?.DateEnd is null)
            return;
        if (assigned && await _userSettingService.TrainingToCalendar(customerId, planUserId))
        {
            _graphService.InitializeGraph();
            if (string.IsNullOrEmpty(calendarEventId))
            {
                var eventResult = await _graphService.AddToCalendar(planUserId, roosterItemDay.Text, roosterItemDay.DateStart.Value, roosterItemDay.DateEnd.Value, roosterItemDay.IsFullDay);
            }
            else
            {
                await _graphService.PatchCalender(planUserId, calendarEventId, roosterItemDay.Text, roosterItemDay.DateStart.Value, roosterItemDay.DateEnd.Value, roosterItemDay.IsFullDay);
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
