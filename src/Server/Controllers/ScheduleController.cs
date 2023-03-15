using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;
using Drogecode.Knrm.Oefenrooster.Server.Services;
using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
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
    public async Task<ActionResult<ScheduleForUserResponse>> ForUser(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, CancellationToken token)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            ScheduleForUserResponse result = await _scheduleService.ScheduleForUserAsync(userId, customerId, yearStart, monthStart, dayStart, yearEnd, monthEnd, dayEnd, token);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForUser");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<ScheduleForAllResponse>> ForAll(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, CancellationToken token)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            ScheduleForAllResponse result = await _scheduleService.ScheduleForAllAsync(userId, customerId, yearStart, monthStart, dayStart, yearEnd, monthEnd, dayEnd, token);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForAll");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult<bool>> PatchTraining(EditTraining patchedTraining, CancellationToken token)
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

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddTraining");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> AddTraining(EditTraining newTraining, CancellationToken token)
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

            return trainingId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddTraining");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult<Training>> PatchScheduleForUser(Training training, CancellationToken token)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _scheduleService.PatchScheduleForUserAsync(userId, customerId, training, token);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Patch");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult> PatchAvailabilityUser(PatchScheduleUserRequest body, CancellationToken token)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            await _scheduleService.PatchAvailabilityUserAsync(userId, customerId, body, token);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PatchScheduleUser");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult> OtherScheduleUser(OtherScheduleUserRequest body, CancellationToken token)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            await _scheduleService.OtherScheduleUserAsync(userId, customerId, body, token);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OtherScheduleUser");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<GetScheduledTrainingsForUserResponse>> GetScheduledTrainingsForUser(CancellationToken token)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var fromDate = DateTime.Today.AddDays(-7).ToUniversalTime();
            var result = await _scheduleService.GetScheduledTrainingsForUser(userId, customerId, fromDate, token);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetScheduledTrainingsForUser");
            return BadRequest();
        }
    }
}
