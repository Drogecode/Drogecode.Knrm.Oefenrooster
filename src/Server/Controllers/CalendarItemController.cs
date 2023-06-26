﻿using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
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
    [Route("month/{year:int}/{month:int}")]
    public async Task<ActionResult<GetMonthItemResponse>> GetMonthItems(int year, int month, CancellationToken token = default)
    {
        try
        {
            var result = new GetMonthItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            result = await _calendarItemService.GetMonthItems(year, month, customerId, token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetMonthItem");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("day/{yearStart:int}/{monthStart:int}/{dayStart:int}/{yearEnd:int}/{monthEnd:int}/{dayEnd:int}")]
    public async Task<ActionResult<GetDayItemResponse>> GetDayItems(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, CancellationToken token = default)
    {
        try
        {
            var result = new GetDayItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            result = await _calendarItemService.GetDayItems(yearStart, monthStart, dayStart, yearEnd, monthEnd, dayEnd, customerId, token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetDayItems");
            return BadRequest();
        }
    }
}
