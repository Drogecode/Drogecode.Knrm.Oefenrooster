using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers.Obsolete;

/// <summary>
/// Can only be deleted if all user clients are updated to a version after the split
/// </summary>
[Obsolete("Use DayItem or MonthItem controller")]
[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class CalendarItemController : ControllerBase
{
    private readonly ILogger<CalendarItemController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDayItemService _dayItemService;
    private readonly IMonthItemService _monthItemService;
    private readonly IAuditService _auditService;
    private readonly IUserSettingService _userSettingService;
    private readonly IGraphService _graphService;

    public CalendarItemController(
        ILogger<CalendarItemController> logger,
        IConfiguration configuration,
        IDayItemService dayItemService,
        IMonthItemService monthItemService,
        IAuditService auditService,
        IUserSettingService userSettingService,
        IGraphService graphService)
    {
        _logger = logger;
        _configuration = configuration;
        _dayItemService = dayItemService;
        _monthItemService = monthItemService;
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
            result = await _monthItemService.GetItems(year, month, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetMonthItem");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("day/{yearStart:int}/{monthStart:int}/{dayStart:int}/{yearEnd:int}/{monthEnd:int}/{dayEnd:int}/{userId:guid}")]
    public async Task<ActionResult<GetMultipleDayItemResponse>> GetDayItems(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, Guid userId, CancellationToken clt = default)
    {
        try
        {
            var result = new GetMultipleDayItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            result = await _dayItemService.GetDayItems(yearStart, monthStart, dayStart, yearEnd, monthEnd, dayEnd, customerId, userId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetDayItems");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("day/all/{count:int}/{skip:int}/{forAllUsers:bool}")]
    public async Task<ActionResult<GetMultipleDayItemResponse>> GetAllFutureDayItems(int count, int skip, bool forAllUsers, CancellationToken clt = default)
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
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            result = await _dayItemService.GetAllFutureDayItems(customerId, count, skip, forAllUsers, userId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
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
            result = await _dayItemService.GetDayItemById(customerId, id, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetDayItemById");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("day/dashboard")]
    public async Task<ActionResult<GetMultipleDayItemResponse>> GetDayItemDashboard(CancellationToken clt = default)
    {

        try
        {
            var result = new GetMultipleDayItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            result = await _dayItemService.GetDayItemDashboard(userId, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
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
            result = await _monthItemService.PutItem(roosterItemMonth, customerId, userId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PutMonthtem");
            return BadRequest();
        }
    }

    [HttpPut]
    [Authorize(Roles = AccessesNames.AUTH_scheduler_dayitem)]
    [Route("day")]
    public async Task<ActionResult<PutDayItemResponse>> PutDayItem([FromBody] RoosterItemDay roosterItemDay, CancellationToken clt = default)
    {
        try
        {
            var result = new PutDayItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            result = await _dayItemService.PutDayItem(roosterItemDay, customerId, userId, clt);

            if (roosterItemDay.LinkedUsers is not null)
            {
                var newd = await _dayItemService.GetDayItemById(customerId, result.NewId, clt);
                if (newd.DayItem?.LinkedUsers is not null)
                    foreach (var user in newd.DayItem.LinkedUsers)
                        await ToOutlookCalendar(user, true, newd.DayItem, customerId, clt);
            }
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PutDayItem");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Authorize(Roles = AccessesNames.AUTH_scheduler_dayitem)]
    [Route("day")]
    public async Task<ActionResult<PatchDayItemResponse>> PatchDayItem([FromBody] RoosterItemDay roosterItemDay, CancellationToken clt = default)
    {
        try
        {
            if (roosterItemDay is null)
                throw new NullReferenceException("roosterItemDay is null");
            var result = new PatchDayItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var old = await _dayItemService.GetDayItemById(customerId, roosterItemDay.Id, clt);
            if (old.DayItem?.Type == CalendarItemType.SpecialDate)
                return Unauthorized();
            result = await _dayItemService.PatchDayItem(roosterItemDay, customerId, userId, clt);
            if (old.DayItem?.LinkedUsers?.Count > 0 is true)
            {
                foreach (var user in old.DayItem.LinkedUsers)
                {
                    if (roosterItemDay.LinkedUsers?.Any(x => x.UserId == user.UserId) is true)
                        continue;
                    await ToOutlookCalendar(user, false, old.DayItem, customerId, clt);

                }
            }
            if (roosterItemDay.LinkedUsers is not null)
            {
                var newd = await _dayItemService.GetDayItemById(customerId, roosterItemDay.Id, clt);
                if (newd.DayItem?.LinkedUsers is not null)
                    foreach (var user in newd.DayItem.LinkedUsers)
                        await ToOutlookCalendar(user, true, newd.DayItem, customerId, clt);
            }
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PatchDayItem");
            return BadRequest();
        }
    }

    [HttpDelete]
    [Authorize(Roles = AccessesNames.AUTH_scheduler_dayitem)]
    [Route("day")]
    public async Task<ActionResult<bool>> DeleteDayItem([FromBody] Guid idToDelete, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var old = await _dayItemService.GetDayItemById(customerId, idToDelete, clt);
            if (old.DayItem?.Type == CalendarItemType.SpecialDate)
                return Unauthorized();
            bool result = await _dayItemService.DeleteDayItem(idToDelete, customerId, userId, clt);
            if (old.DayItem?.LinkedUsers is not null)
            {
                foreach (var user in old.DayItem.LinkedUsers)
                    await ToOutlookCalendar(user, false, old.DayItem, customerId, clt);
            }
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in DeleteDayItem");
            return BadRequest();
        }
    }

    private async Task ToOutlookCalendar(RoosterItemDayLinkedUsers user, bool assigned, RoosterItemDay roosterItemDay, Guid customerId, CancellationToken clt)
    {
        if (roosterItemDay.DateStart is null)
            return;
        if (roosterItemDay.DateEnd is null)
            roosterItemDay.DateEnd = roosterItemDay.DateStart;
        if (assigned && await _userSettingService.TrainingToCalendar(customerId, user.UserId))
        {
            _graphService.InitializeGraph();
            if (string.IsNullOrEmpty(user.CalendarEventId))
            {
                var eventResult = await _graphService.AddToCalendar(user.UserId, roosterItemDay.Text, roosterItemDay.DateStart.Value, roosterItemDay.DateEnd.Value, true);
                await _dayItemService.PatchCalendarEventId(roosterItemDay.Id, user.UserId, customerId, eventResult.Id, clt);
            }
            else
            {
                await _graphService.PatchCalender(user.UserId, user.CalendarEventId, roosterItemDay.Text, roosterItemDay.DateStart.Value, roosterItemDay.DateEnd.Value, true);
            }
        }
        else if (!string.IsNullOrEmpty(user.CalendarEventId))
        {
            _graphService.InitializeGraph();
            await _graphService.DeleteCalendarEvent(user.UserId, user.CalendarEventId, clt);
            await _dayItemService.PatchCalendarEventId(roosterItemDay.Id, user.UserId, customerId, null, clt);
        }
    }
}
