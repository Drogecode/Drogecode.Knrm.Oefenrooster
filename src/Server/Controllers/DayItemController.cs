using Drogecode.Knrm.Oefenrooster.Server.Controllers.Obsolite;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Server.Hubs;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "DayItem")]
public class DayItemController : ControllerBase
{
    private readonly ILogger<DayItemController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDayItemService _dayItemService;
    private readonly IAuditService _auditService;
    private readonly IUserSettingService _userSettingService;
    private readonly IGraphService _graphService;
    private readonly RefreshHub _refreshHub;

    public DayItemController(
        ILogger<DayItemController> logger,
        IConfiguration configuration,
        IDayItemService dayItemService,
        IAuditService auditService,
        IUserSettingService userSettingService,
        IGraphService graphService, 
        RefreshHub refreshHub)
    {
        _logger = logger;
        _configuration = configuration;
        _dayItemService = dayItemService;
        _auditService = auditService;
        _userSettingService = userSettingService;
        _graphService = graphService;
        _refreshHub = refreshHub;
    }

    [HttpGet]
    [Route("{yearStart:int}/{monthStart:int}/{dayStart:int}/{yearEnd:int}/{monthEnd:int}/{dayEnd:int}/{userId:guid}")]
    public async Task<ActionResult<GetMultipleDayItemResponse>> GetItems(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, Guid userId, CancellationToken clt = default)
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
    [Route("all/{count:int}/{skip:int}/{forAllUsers:bool}/{callHub:bool}")]
    public async Task<ActionResult<GetMultipleDayItemResponse>> GetAllFuture(int count, int skip, bool forAllUsers, bool callHub = false, CancellationToken clt = default)
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
            if (callHub)
            {
                _logger.LogInformation("Calling hub AllFutureDayItems");
                await _refreshHub.SendMessage(userId, ItemUpdated.AllFutureDayItems);
            }
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
    [Route("{id:guid}")]
    public async Task<ActionResult<GetDayItemResponse>> ById(Guid id, CancellationToken clt = default)
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
    [Route("dashboard/{callHub:bool}")]
    public async Task<ActionResult<GetMultipleDayItemResponse>> GetDashboard(bool callHub = false, CancellationToken clt = default)
    {

        try
        {
            var result = new GetMultipleDayItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            result = await _dayItemService.GetDayItemDashboard(userId, customerId, clt);
            if (callHub)
            {
                _logger.LogInformation("Calling hub AllFutureDayItems");
                await _refreshHub.SendMessage(userId, ItemUpdated.DayItemDashboard);
            }
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
    [Authorize(Roles = AccessesNames.AUTH_scheduler_dayitem)]
    [Route("")]
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
    [Route("")]
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
    [Route("")]
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
