using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
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
            var users = new List<Guid>() { userId };

            var result = await _reportActionService.GetListActionsUser(users, null, null, count, skip, customerId, false, null, null, clt);
            _logger.LogInformation("Loading actions {count} skipping {skip} for user {userName}", count, skip, userName);
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

    [HttpPost]
    [Route("get")]
    public async Task<ActionResult<MultipleReportActionsResponse>> GetLastActions([FromBody] GetLastActionsRequest body, CancellationToken clt = default)
    {
        try
        {
            if (body.Count > 50) return Forbid();
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));

            var advancedSearchAllowed = User.IsInRole(AccessesNames.AUTH_action_search);
            if (!advancedSearchAllowed && body.Search != null && body.Search.Count != 0)
            {
                // Search could be a performance problem and should have a kill switch or be limited to a select group of users.
                _logger.LogWarning("Search request from user `{userId}` without proper permission", userId);
            }
            var cleanedSearch = advancedSearchAllowed ? FilthyInputHelper.CleanList(body.Search, 10, _logger) : null;
            var cleanedTypes = FilthyInputHelper.CleanList(body.Types, 10, _logger);

            var result = await _reportActionService.GetListActionsUser(body.Users, cleanedTypes, cleanedSearch, body.Count, body.Skip, customerId, false, null, null, clt);
            _logger.LogInformation("Loading actions {count} skipping {skip} for user {users} ({userId})", body.Count, body.Skip, body.Users, userId);
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
    [Authorize]
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

    [HttpPost]
    [Route("analyze/hours")]
    [Authorize(Roles = AccessesNames.AUTH_dashboard_Statistics_user_tabel)]
    public async Task<ActionResult<AnalyzeHoursResult>> GetAnalyzeHours([FromBody] AnalyzeHoursRequest body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var timeZone = await _customerSettingService.GetTimeZone(customerId);

            if (body.Type is null)
            {
                _logger.LogWarning("GetAnalyzeHours request but Type is null");
                return BadRequest();
            }
            
            var result = await _reportActionService.AnalyzeHours(body.Year, body.Type, body.Boats, timeZone, customerId, clt);
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
    [Authorize(Roles = AccessesNames.AUTH_super_user)]
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