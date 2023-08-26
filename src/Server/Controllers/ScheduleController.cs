﻿using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "Schedule")]
public class ScheduleController : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger;
    private readonly IScheduleService _scheduleService;
    private readonly IAuditService _auditService;
    private readonly IGraphService _graphService;
    private readonly ITrainingTypesService _trainingTypesService;
    private readonly IUserSettingService _userSettingService;
    private readonly IDateTimeService _dateTimeService;

    public ScheduleController(
        ILogger<ScheduleController> logger,
        IScheduleService scheduleService,
        IAuditService auditService,
        IGraphService graphService,
        ITrainingTypesService trainingTypesService,
        IUserSettingService userSettingService,
        IDateTimeService dateTimeService)
    {
        _logger = logger;
        _scheduleService = scheduleService;
        _auditService = auditService;
        _graphService = graphService;
        _trainingTypesService = trainingTypesService;
        _userSettingService = userSettingService;
        _dateTimeService = dateTimeService;
    }

    [HttpGet]
    [Route("me/period/{yearStart:int}/{monthStart:int}/{dayStart:int}/{yearEnd:int}/{monthEnd:int}/{dayEnd:int}")]
    public async Task<ActionResult<MultipleTrainingsResponse>> ForUser(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            MultipleTrainingsResponse result = await _scheduleService.ScheduleForUserAsync(userId, customerId, yearStart, monthStart, dayStart, yearEnd, monthEnd, dayEnd, token);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForUser");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("all/period/{forMonth:int}/{yearStart:int}/{monthStart:int}/{dayStart:int}/{yearEnd:int}/{monthEnd:int}/{dayEnd:int}")]
    public async Task<ActionResult<ScheduleForAllResponse>> ForAll(int forMonth, int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var countPerUser = User.IsInRole(AccessesNames.AUTH_users_counter);
            ScheduleForAllResponse result = await _scheduleService.ScheduleForAllAsync(userId, customerId, forMonth, yearStart, monthStart, dayStart, yearEnd, monthEnd, dayEnd, countPerUser, token);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForAll");
            return BadRequest();
        }
    }
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<GetTrainingByIdResponse>> GetTrainingById(Guid id)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            GetTrainingByIdResponse result = await _scheduleService.GetTrainingById(userId, customerId, id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForAll");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("training")]
    public async Task<ActionResult<PatchTrainingResponse>> PatchTraining([FromBody] PlannedTraining patchedTraining, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            PatchTrainingResponse result = await _scheduleService.PatchTraining(customerId, patchedTraining, token);
            if (result.Success)
                await _auditService.Log(userId, AuditType.PatchTraining, customerId, objectKey: patchedTraining.TrainingId, objectName: patchedTraining.Name);
            else
                await _auditService.Log(userId, AuditType.PatchTraining, customerId, "Failed", patchedTraining.TrainingId, patchedTraining.Name);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddTraining");
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("training")]
    public async Task<ActionResult<AddTrainingResponse>> AddTraining([FromBody] PlannedTraining newTraining, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var trainingId = Guid.NewGuid();
            var result = await _scheduleService.AddTrainingAsync(customerId, newTraining, trainingId, token);
            if (result.Success)
                await _auditService.Log(userId, AuditType.AddTraining, customerId, objectKey: trainingId, objectName: newTraining.Name);
            else
                await _auditService.Log(userId, AuditType.AddTraining, customerId, "Failed", trainingId, newTraining.Name);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddTraining");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("schedule")]
    public async Task<ActionResult<PatchScheduleForUserResponse>> PatchScheduleForUser([FromBody] Training training, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _scheduleService.PatchScheduleForUserAsync(userId, customerId, training, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PatchScheduleForUser");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("assigned-user")]
    public async Task<ActionResult<PatchAssignedUserResponse>> PatchAssignedUser([FromBody] PatchAssignedUserRequest body, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            PatchAssignedUserResponse result = await _scheduleService.PatchAssignedUserAsync(userId, customerId, body, clt);
            await _auditService.Log(userId, AuditType.PatchAssignedUser, customerId, JsonSerializer.Serialize(new AuditAssignedUser { TrainingId = body.TrainingId, Assigned = body?.User?.Assigned }), body?.User?.UserId);
            if (result.Success && body?.User?.UserId is not null)
                await ToOutlookCalendar(body.User.UserId, body.User.Assigned, body.Training, userId, customerId, result.AvailableId, result.CalendarEventId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PatchAssignedUser");
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("assigned-user")]
    public async Task<ActionResult<PutAssignedUserResponse>> PutAssignedUser([FromBody] OtherScheduleUserRequest body, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _scheduleService.PutAssignedUserAsync(userId, customerId, body, clt);
            await _auditService.Log(userId, AuditType.PatchAssignedUser, customerId, JsonSerializer.Serialize(new AuditAssignedUser { TrainingId = body.TrainingId, Assigned = body?.Assigned }), body?.UserId);
            if (result.Success && body?.UserId is not null)
                await ToOutlookCalendar(body.UserId.Value, body.Assigned, body.Training, userId, customerId, result.AvailableId, result.CalendarEventId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PutAssignedUser");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("me/scheduled-trainings")]
    public async Task<ActionResult<GetScheduledTrainingsForUserResponse>> GetScheduledTrainingsForUser(CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var fromDate = _dateTimeService.Today().AddDays(-1).ToUniversalTime();
            var result = await _scheduleService.GetScheduledTrainingsForUser(userId, customerId, fromDate, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetScheduledTrainingsForUser");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("training/user/all/{id:guid}")]
    [Authorize(Roles = AccessesNames.AUTH_users_details)]
    public async Task<ActionResult<GetScheduledTrainingsForUserResponse>> AllTrainingsForUser(Guid id, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            GetScheduledTrainingsForUserResponse result = await _scheduleService.GetScheduledTrainingsForUser(id, customerId, null, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AllTrainingsForUser");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("me/pinned")]
    public async Task<ActionResult<GetPinnedTrainingsForUserResponse>> GetPinnedTrainingsForUser(CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var fromDate = DateTime.Today.ToUniversalTime();
            GetPinnedTrainingsForUserResponse result = await _scheduleService.GetPinnedTrainingsForUser(userId, customerId, fromDate, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetPinnedTrainingsForUser");
            return BadRequest();
        }
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<ActionResult<bool>> DeleteTraining(Guid id, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var training = await _scheduleService.GetPlannedTrainingById(id);
            foreach (var user in training.PlanUsers)
            {
                user.Assigned = false;
                var body = new PatchAssignedUserRequest
                {
                    TrainingId = id,
                    User = user,
                    Training = training
                };
                await PatchAssignedUser(body, clt);
            }
            var result = await _scheduleService.DeleteTraining(userId, customerId, id, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteTraining");
            return BadRequest();
        }
    }

    private async Task ToOutlookCalendar(Guid planUserId, bool assigned, TrainingAdvance? training, Guid currentUserId, Guid customerId, Guid? availableId, string? calendarEventId, CancellationToken clt)
    {
        if (assigned && await _userSettingService.TrainingToCalendar(customerId, planUserId))
        {
            var type = await _trainingTypesService.GetById(training?.RoosterTrainingTypeId ?? Guid.Empty, customerId, clt);
            if (string.IsNullOrEmpty(calendarEventId))
            {
                if (!string.IsNullOrEmpty(type?.TrainingType?.Name))
                {
                    var text = $"{type.TrainingType.Name} - {training?.Name}";
                    _graphService.InitializeGraph();
                    var eventResult = await _graphService.AddToCalendar(planUserId, text, training!.DateStart, training.DateEnd);
                    await _scheduleService.PatchEventIdForUserAvailible(planUserId, customerId, availableId, eventResult.Id, clt);
                }
            }
            else
            {
                await _auditService.Log(currentUserId, AuditType.PatchTraining, customerId, $"Preventing duplicate event '{type?.TrainingType?.Name}' on '{training?.DateStart.ToString("o")}' : '{training?.DateEnd.ToString("o")}'");
            }
        }
        else if (!string.IsNullOrEmpty(calendarEventId))
        {
            _graphService.InitializeGraph();
            await _graphService.DeleteCalendarEvent(planUserId, calendarEventId, clt);
            await _scheduleService.PatchEventIdForUserAvailible(planUserId, customerId, availableId, null, clt);
        }
    }
}
