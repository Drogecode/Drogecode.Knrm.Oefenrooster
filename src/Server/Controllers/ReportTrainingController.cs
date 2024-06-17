using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportTraining;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "ReportTraining")]
public class ReportTrainingController : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger;
    private readonly IReportTrainingService _reportTrainingService;
    private readonly IAuditService _auditService;

    public ReportTrainingController(
        ILogger<ScheduleController> logger,
        IReportTrainingService reportTrainingService,
        IAuditService auditService)
    {
        _logger = logger;
        _reportTrainingService = reportTrainingService;
        _auditService = auditService;
    }
    
    [HttpGet]
    [Route("user/{count:int}/{skip:int}")]
    public async Task<ActionResult<MultipleReportTrainingsResponse>> GetLastTrainingsForCurrentUser(int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 30) return Forbid();
            var userName = User?.FindFirstValue("FullName") ?? throw new Exception("No userName found");
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var users = new List<Guid>() { userId };

            var result = await _reportTrainingService.GetListTrainingUser(users, userId, count, skip, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastTrainingsForCurrentUser");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{users}/{count:int}/{skip:int}")]
    public async Task<ActionResult<MultipleReportTrainingsResponse>> GetLastTrainings(string users, int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 30) return Forbid();
            var userName = User?.FindFirstValue("FullName") ?? throw new Exception("No userName found");
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var usersAsList = System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(users);
            if (usersAsList is null)
                return BadRequest("users is null");

            var result = await _reportTrainingService.GetListTrainingUser(usersAsList, userId, count, skip, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastTrainings");
            return BadRequest();
        }
    }
}