﻿using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "MonthItem")]
public class MonthItemController : ControllerBase
{
    private readonly ILogger<MonthItemController> _logger;
    private readonly IMonthItemService _monthItemService;

    public MonthItemController(
        ILogger<MonthItemController> logger,
        IMonthItemService monthItemService)
    {
        _logger = logger;
        _monthItemService = monthItemService;
    }

    [HttpGet]
    [Authorize(Roles = AccessesNames.AUTH_scheduler_monthitem)]
    [Route("{id:guid}")]
    public async Task<ActionResult<GetMonthItemResponse>> ById(Guid id, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _monthItemService.GetById(customerId, id, clt);
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
    [Route("{year:int}/{month:int}")]
    public async Task<ActionResult<GetMultipleMonthItemResponse>> GetItems(int year, int month, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _monthItemService.GetItems(year, month, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in month GetItems");
            return BadRequest();
        }
    }

    [HttpGet]
    [Authorize(Roles = AccessesNames.AUTH_scheduler_monthitem)]
    [Route("all/{take:int}/{skip:int}/{includeExpired:bool}")]
    public async Task<ActionResult<GetMultipleMonthItemResponse>> GetAllItems(int take, int skip, bool includeExpired, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _monthItemService.GetAllItems(take, skip, includeExpired, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in month GetAllItems");
            return BadRequest();
        }
    }

    [HttpPut]
    [Authorize(Roles = AccessesNames.AUTH_scheduler_monthitem)]
    [Route("")]
    public async Task<ActionResult<PutMonthItemResponse>> PutItem([FromBody] RoosterItemMonth roosterItemMonth, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _monthItemService.PutItem(roosterItemMonth, customerId, userId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in month PutItem");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Authorize(Roles = AccessesNames.AUTH_scheduler_monthitem)]
    [Route("")]
    public async Task<ActionResult<PatchMonthItemResponse>> PatchItem([FromBody] RoosterItemMonth roosterItemMonth, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _monthItemService.PatchItem(roosterItemMonth, customerId, userId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in month PatchItem");
            return BadRequest();
        }
    }

    [HttpDelete]
    [Authorize(Roles = AccessesNames.AUTH_scheduler_monthitem)]
    [Route("")]
    public async Task<ActionResult<bool>> DeleteMonthItem([FromBody] Guid idToDelete, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _monthItemService.DeleteItem(idToDelete, customerId, userId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in DeleteMonthItem");
            return BadRequest();
        }
    }
}