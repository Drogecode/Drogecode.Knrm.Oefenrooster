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

    public ReportActionController(
        ILogger<ScheduleController> logger,
        IReportActionService reportActionService)
    {
        _logger = logger;
        _reportActionService = reportActionService;
    }

    [HttpGet]
    [Route("user/{count:int}/{skip:int}")]
    public async Task<ActionResult<MultipleReportActionsResponse>> GetLastActionsForCurrentUser(int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 30) return Forbid();
            var userName = User?.FindFirstValue("FullName") ?? throw new Exception("No userName found");
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var users = new List<Guid>() { userId };

            var result = await _reportActionService.GetListActionsUser(users, userId, count, skip, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastActionsForCurrentUser");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{users}/{count:int}/{skip:int}")]
    public async Task<ActionResult<MultipleReportActionsResponse>> GetLastActions(string users, int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 30) return Forbid();
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var usersAsList = JsonSerializer.Deserialize<List<Guid>>(users);
            if (usersAsList is null)
                return BadRequest("users is null");

            var result = await _reportActionService.GetListActionsUser(usersAsList, userId, count, skip, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastActions");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("analyze/years")]
    [Authorize(Roles = AccessesNames.AUTH_dashboard_Statistics)]
    public async Task<ActionResult<AnalyzeYearChartAllResponse>> AnalyzeYearChartsAll(string users, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var usersAsList = JsonSerializer.Deserialize<List<Guid>>(users);
            if (usersAsList is null)
                return BadRequest("users is null");

            var result = await _reportActionService.AnalyzeYearChartsAll(usersAsList, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in AnalyzeYearChartsAll");
            return BadRequest();
        }
    }
}
