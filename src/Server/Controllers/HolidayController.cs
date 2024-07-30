using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
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
[ApiExplorerSettings(GroupName = "Holiday")]
public class HolidayController : ControllerBase
{
    private readonly ILogger<HolidayController> _logger;
    private readonly IHolidayService _holidayService;
    private readonly IAuditService _auditService;
    private readonly RefreshHub _refreshHub;

    public HolidayController(
        ILogger<HolidayController> logger,
        IHolidayService holidayService,
        IAuditService auditService,
        RefreshHub refreshHub)
    {
        _logger = logger;
        _holidayService = holidayService;
        _auditService = auditService;
        _refreshHub = refreshHub;
    }

    [HttpGet]
    [Route("all/user")]
    [Route("")]// from version v0.3.82 and older
    public async Task<ActionResult<MultipleHolidaysResponse>> GetAll(CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _holidayService.GetAllHolidaysForUser(customerId, userId, clt);

            return result;
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

    [HttpGet]
    [Authorize(Roles = AccessesNames.AUTH_dashboard_holidays)]
    [Route("all/future/{days:int}")]
    public async Task<ActionResult<MultipleHolidaysResponse>> GetAllFuture(int days, bool callHub, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            MultipleHolidaysResponse result = await _holidayService.GetAllHolidaysForFuture(customerId, userId, days, clt);

            if (callHub)
            {
                _logger.LogTrace("Calling hub GetAllFuture holidays");
                await _refreshHub.SendMessage(userId, ItemUpdated.FutureHolidays);
            }
            return result;
        }
        catch (OperationCanceledException)
        {
            return BadRequest();
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

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<GetResponse>> Get(Guid id, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _holidayService.Get(id, customerId, userId, clt);

            return result;
        }
        catch (OperationCanceledException)
        {
            return BadRequest();
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
    [Route("")]
    public async Task<ActionResult<PutHolidaysForUserResponse>> PutHolidayForUser([FromBody] Holiday body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _holidayService.PutHolidaysForUser(body, customerId, userId, clt);

            return result;
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
    [Route("")]
    public async Task<ActionResult<PatchHolidaysForUserResponse>> PatchHolidayForUser([FromBody] Holiday body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _holidayService.PatchHolidaysForUser(body, customerId, userId, clt);

            return result;
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

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<ActionResult<DeleteResonse>> Delete(Guid id, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _holidayService.Delete(id, customerId, userId, clt);

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in Delete holiday");
            return BadRequest();
        }
    }
}
