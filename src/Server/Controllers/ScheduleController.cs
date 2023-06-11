using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "Schedule")]
public class ScheduleController : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger;
    private readonly IScheduleService _scheduleService;
    private readonly IAuditService _auditService;

    public ScheduleController(
        ILogger<ScheduleController> logger,
        IScheduleService scheduleService,
        IAuditService auditService)
    {
        _logger = logger;
        _scheduleService = scheduleService;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<ActionResult<ScheduleForUserResponse>> ForUser(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            ScheduleForUserResponse result = await _scheduleService.ScheduleForUserAsync(userId, customerId, yearStart, monthStart, dayStart, yearEnd, monthEnd, dayEnd, token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForUser");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<ScheduleForAllResponse>> ForAll(int forMonth, int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            ScheduleForAllResponse result = await _scheduleService.ScheduleForAllAsync(userId, customerId, forMonth, yearStart, monthStart, dayStart, yearEnd, monthEnd, dayEnd, token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForAll");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult<PatchTrainingResponse>> PatchTraining(EditTraining patchedTraining, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            bool result = await _scheduleService.PatchTraining(customerId, patchedTraining, token);
            if (result)
                await _auditService.Log(userId, AuditType.PatchTraining, customerId, objectKey: patchedTraining.Id, objectName: patchedTraining.Name);
            else
                await _auditService.Log(userId, AuditType.PatchTraining, customerId, "Failed", patchedTraining.Id, patchedTraining.Name);

            return Ok(new PatchTrainingResponse { Success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddTraining");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult<AddTrainingResponse>> AddTraining(EditTraining newTraining, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var trainingId = Guid.NewGuid();
            var result = await _scheduleService.AddTrainingAsync(customerId, newTraining, trainingId, token);
            if (result)
                await _auditService.Log(userId, AuditType.AddTraining, customerId, objectKey: trainingId, objectName: newTraining.Name);
            else
                await _auditService.Log(userId, AuditType.AddTraining, customerId, "Failed", trainingId, newTraining.Name);

            return Ok(new AddTrainingResponse { NewId = trainingId, Success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddTraining");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult<PatchScheduleForUserResponse>> PatchScheduleForUser(Training training, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _scheduleService.PatchScheduleForUserAsync(userId, customerId, training, token);
            return Ok(new PatchScheduleForUserResponse { PatchedTraining = result, Success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Patch");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult> PatchAssignedUser(PatchAssignedUserRequest body, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            await _scheduleService.PatchAssignedUserAsync(userId, customerId, body, token);
            await _auditService.Log(userId, AuditType.PatchAssignedUser, customerId, JsonSerializer.Serialize(new AuditAssignedUser { TrainingId = body.TrainingId, Assigned = body?.User?.Assigned }), body?.User?.UserId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PatchScheduleUser");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult> PutAssignedUser(OtherScheduleUserRequest body, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            await _scheduleService.PutAssignedUserAsync(userId, customerId, body, token);
            await _auditService.Log(userId, AuditType.PatchAssignedUser, customerId, JsonSerializer.Serialize(new AuditAssignedUser { TrainingId = body.TrainingId, Assigned = body?.Assigned }), body?.UserId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OtherScheduleUser");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<GetScheduledTrainingsForUserResponse>> GetScheduledTrainingsForUser(CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var fromDate = DateTime.Today.AddDays(-7).ToUniversalTime();
            var result = await _scheduleService.GetScheduledTrainingsForUser(userId, customerId, fromDate, token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetScheduledTrainingsForUser");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<GetPinnedTrainingsForUserResponse>> GetPinnedTrainingsForUser(CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var fromDate = DateTime.Today.ToUniversalTime();
            GetPinnedTrainingsForUserResponse result = await _scheduleService.GetPinnedTrainingsForUser(userId, customerId, fromDate, token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetPinnedTrainingsForUser");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<MultiplePlannerTrainingTypesResponse>> GetTrainingTypes(CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _scheduleService.GetTrainingTypes(customerId, token);
            return Ok(new MultiplePlannerTrainingTypesResponse { PlannerTrainingTypes = result, Success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetTrainingTypes");
            return BadRequest();
        }
    }
}
