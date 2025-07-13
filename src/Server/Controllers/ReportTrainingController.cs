using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportTraining;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "ReportTraining")]
public class ReportTrainingController : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger;
    private readonly IReportTrainingService _reportTrainingService;
    private readonly IAuditService _auditService;
    private readonly ICustomerSettingService _customerSettingService;

    public ReportTrainingController(
        ILogger<ScheduleController> logger,
        IReportTrainingService reportTrainingService,
        IAuditService auditService, 
        ICustomerSettingService customerSettingService)
    {
        _logger = logger;
        _reportTrainingService = reportTrainingService;
        _auditService = auditService;
        _customerSettingService = customerSettingService;
    }
    
    [HttpGet]
    [Route("user/{count:int}/{skip:int}")]
    public async Task<ActionResult<MultipleReportTrainingsResponse>> GetLastTrainingsForCurrentUser(int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 50) return Forbid();
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var users = new List<Guid?>() { userId };

            var result = await _reportTrainingService.GetListTrainingUser(users, null, userId, count, skip, customerId, clt);
            _logger.LogInformation("Loading trainings {count} skipping {skip} for user {userId}", count, skip, userId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastTrainingsForCurrentUser");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{users}/{count:int}/{skip:int}/{types}")]
    public async Task<ActionResult<MultipleReportTrainingsResponse>> GetLastTrainings(string users, int count, int skip, string? types = null, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            
            if (count > 50)
            {
                _logger.LogWarning("GetLastTrainings request but count is too high {count} by user {userId}", count, userId);
                return Forbid();
            }
            
            var usersAsList = System.Text.Json.JsonSerializer.Deserialize<List<Guid?>>(users);
            if (usersAsList is null)
                return BadRequest("users is null");
            List<string>? typesAsList = null;
            if(types is not null) typesAsList = JsonSerializer.Deserialize<List<string>>(types);

            var result = await _reportTrainingService.GetListTrainingUser(usersAsList, typesAsList, userId, count, skip, customerId, clt);
            _logger.LogInformation("Loading trainings {count} skipping {skip} for user {users} ({userId})", count, skip, users.CleanStringForLogging(), userId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastTrainings");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("training/{trainingId:guid}/{count:int}/{skip:int}")]
    public async Task<ActionResult<MultipleReportTrainingsResponse>> GetReportsLinkedToTraining(Guid trainingId, int count, int skip, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));

            if (count > 50)
            {
                _logger.LogWarning("GetReportsLinkedToTraining request but count is too high {count} by user {userId}", count, userId);
                return Forbid();
            }
            
            var result = await _reportTrainingService.GetReportsLinkedToTraining(userId, customerId, trainingId, count, skip, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetReportsLinkedToTraining");
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("analyze/years")]
    [Authorize(Roles = AccessesNames.AUTH_dashboard_Statistics)]
    public async Task<ActionResult<AnalyzeYearChartAllResponse>> AnalyzeYearChartsAll([FromBody] AnalyzeTrainingRequest request, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var timeZone = await _customerSettingService.GetTimeZone(customerId);
            if (request.Users is null)
                return BadRequest("Users is null");
            if (request.Users.Count > 5)
                return new AnalyzeYearChartAllResponse() { Message = "To many users" };

            var result = await _reportTrainingService.AnalyzeYearChartsAll(request, customerId, timeZone, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in AnalyzeYearChartsAll");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("distinct/{column}")]
    public async Task<ActionResult<DistinctResponse>> Distinct(DistinctReport column, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));

            var result = await _reportTrainingService.Distinct(column, customerId, userId, clt);
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
            _logger.LogError(ex, "Exception in Distinct Training");
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

            var result = await _reportTrainingService.AnalyzeHours(year, type, timeZone, customerId, clt);
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
            _logger.LogError(ex, "Exception in AnalyzeHours Training");
            return BadRequest();
        }
    }
}