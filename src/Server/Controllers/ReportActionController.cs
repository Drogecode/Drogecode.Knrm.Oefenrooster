using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;
using System.Text.Json;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "ReportAction")]
public class ReportActionController : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger;
    private readonly IReportActionService _reportActionService;
    private readonly ICustomerSettingService _customerSettingService;

    public ReportActionController(
        ILogger<ScheduleController> logger,
        IReportActionService reportActionService,
        ICustomerSettingService customerSettingService)
    {
        _logger = logger;
        _reportActionService = reportActionService;
        _customerSettingService = customerSettingService;
    }

    [HttpGet]
    [Route("user/{count:int}/{skip:int}")]
    public async Task<ActionResult<MultipleReportActionsResponse>> GetLastActionsForCurrentUser(int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 50) return Forbid();
            var userName = User?.FindFirstValue("FullName") ?? throw new Exception("No userName found");
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var users = new List<Guid?>() { userId };

            var result = await _reportActionService.GetListActionsUser(users, null, userId, count, skip, customerId, clt);
            _logger.LogInformation("Loading actions {count} skipping {}", count);
            return result;
        }
        catch (OperationCanceledException)
        {
            return Ok();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetLastActionsForCurrentUser");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{users}/{count:int}/{skip:int}/{types}", Order = 0)]
    [Route("{users}/{count:int}/{skip:int}", Order = 1)]// ToDo Remove when all users on v0.4.22 or above
    public async Task<ActionResult<MultipleReportActionsResponse>> GetLastActions(string users, int count, int skip, string? types = null, CancellationToken clt = default)
    {
        try
        {
            if (count > 50) return Forbid();
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var usersAsList = JsonSerializer.Deserialize<List<Guid?>>(users);
            if (usersAsList is null)
                return BadRequest("users is null");
            List<string>? typesAsList = null;
            if(types is not null) typesAsList = JsonSerializer.Deserialize<List<string>>(types);

            var result = await _reportActionService.GetListActionsUser(usersAsList, typesAsList, userId, count, skip, customerId, clt);
            return result;
        }
        catch (OperationCanceledException)
        {
            return Ok();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetLastActions");
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("analyze/years")]
    [Authorize(Roles = AccessesNames.AUTH_dashboard_Statistics)]
    public async Task<ActionResult<AnalyzeYearChartAllResponse>> AnalyzeYearChartsAll([FromBody] AnalyzeActionRequest request, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var timeZone = await _customerSettingService.GetTimeZone(customerId);
            if (request.Users is null)
                return BadRequest("Users is null");
            if (request.Users.Count > 5)
                return new AnalyzeYearChartAllResponse() { Message = "To many users" };
            if (request.Prio?.Count() > 5)
                return new AnalyzeYearChartAllResponse() { Message = "To many prio" };
            if (request.Prio?.Any(x => x.Length > 15) == true)
                return BadRequest("To long");

            var result = await _reportActionService.AnalyzeYearChartsAll(request, customerId, timeZone, clt);
            return result;
        }
        catch (OperationCanceledException)
        {
            return Ok();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in AnalyzeYearChartsAll");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("distinct/{column}")]
    [Authorize(Roles = AccessesNames.AUTH_dashboard_Statistics)]
    public async Task<ActionResult<DistinctResponse>> Distinct(DistinctReport column, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));

            var result = await _reportActionService.Distinct(column, customerId, clt);
            return result;
        }
        catch (OperationCanceledException)
        {
            return Ok();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in Distinct Action");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("analyze/hours/{year:int}/{type}")]
    [Authorize(Roles = AccessesNames.AUTH_dashboard_Statistics_user_tabel)]
    public async Task<ActionResult<AnalyzeHoursResult>> AnalyzeHours(int year, string type, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var timeZone = await _customerSettingService.GetTimeZone(customerId);

            var result = await _reportActionService.AnalyzeHours(year, type, timeZone, customerId, clt);
            return result;
        }
        catch (OperationCanceledException)
        {
            return Ok();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in AnalyzeHours Action");
            return BadRequest();
        }
    }
    
    [HttpGet]
    [Route("kill")]
    [Authorize(Roles = AccessesNames.AUTH_Taco)]
    public async Task<ActionResult<KillDbResponse>> KillDb(CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));

            var result = await _reportActionService.KillDb(customerId, clt);
            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            return Ok();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in KillDb");
            return BadRequest();
        }
    }
}