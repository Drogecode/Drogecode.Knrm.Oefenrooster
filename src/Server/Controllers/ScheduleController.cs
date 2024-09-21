using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "Schedule")]
public class ScheduleController : ControllerBase
{
    private readonly RefreshHub _refreshHub;
    private readonly ILogger<ScheduleController> _logger;
    private readonly IScheduleService _scheduleService;
    private readonly IAuditService _auditService;
    private readonly IGraphService _graphService;
    private readonly ITrainingTypesService _trainingTypesService;
    private readonly IUserSettingService _userSettingService;
    private readonly IDateTimeService _dateTimeService;
    private readonly IUserService _userService;
    private readonly IFunctionService _functionService;

    public ScheduleController(
        RefreshHub refreshHub,
        ILogger<ScheduleController> logger,
        IScheduleService scheduleService,
        IAuditService auditService,
        IGraphService graphService,
        ITrainingTypesService trainingTypesService,
        IUserSettingService userSettingService,
        IDateTimeService dateTimeService,
        IUserService userService,
        IFunctionService functionService)
    {
        _refreshHub = refreshHub;
        _logger = logger;
        _scheduleService = scheduleService;
        _auditService = auditService;
        _graphService = graphService;
        _trainingTypesService = trainingTypesService;
        _userSettingService = userSettingService;
        _dateTimeService = dateTimeService;
        _userService = userService;
        _functionService = functionService;
    }

    [HttpGet]
    [Route("me/period/{yearStart:int}/{monthStart:int}/{dayStart:int}/{yearEnd:int}/{monthEnd:int}/{dayEnd:int}")]
    public async Task<ActionResult<MultipleTrainingsResponse>> ForUser(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _scheduleService.ScheduleForUserAsync(userId, customerId, yearStart, monthStart, dayStart, yearEnd, monthEnd, dayEnd, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForUser");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("all/period/{forMonth:int}/{yearStart:int}/{monthStart:int}/{dayStart:int}/{yearEnd:int}/{monthEnd:int}/{dayEnd:int}/{includeUnAssigned:bool}")]
    public async Task<ActionResult<ScheduleForAllResponse>> ForAll(int forMonth, int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, bool includeUnAssigned,
        CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var countPerUser = User.IsInRole(AccessesNames.AUTH_users_counter);
            var result = await _scheduleService.ScheduleForAllAsync(userId, customerId, forMonth, yearStart, monthStart, dayStart, yearEnd, monthEnd, dayEnd, countPerUser, includeUnAssigned, clt);
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
    public async Task<ActionResult<GetTrainingByIdResponse>> GetTrainingById(Guid id, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _scheduleService.GetTrainingById(userId, customerId, id, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetTrainingById");
            return BadRequest();
        }
    }

    [HttpGet]
    [Authorize(Roles = AccessesNames.AUTH_scheduler_description_read)]
    [Route("{id:guid}/description")]
    public async Task<ActionResult<GetDescriptionByTrainingIdResponse>> GetDescriptionByTrainingId(Guid id, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            GetDescriptionByTrainingIdResponse result = await _scheduleService.GetDescriptionByTrainingId(userId, customerId, id, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetDescriptionByTrainingId");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("planned/{id:guid}")]
    public async Task<ActionResult<GetPlannedTrainingResponse>> GetPlannedTrainingById(Guid id, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _scheduleService.GetPlannedTrainingById(customerId, id, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetPlannedTrainingById");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("default/{date}")]
    public async Task<ActionResult<GetPlannedTrainingResponse>> GetPlannedTrainingForDefaultDate(DateTime date, Guid defaultId, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _scheduleService.GetPlannedTrainingForDefaultDate(customerId, date, defaultId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetPlannedTrainingForDefaultDate");
            return BadRequest();
        }
    }


    [HttpPatch]
    [Route("training")]
    public async Task<ActionResult<PatchTrainingResponse>> PatchTraining([FromBody] PlannedTraining patchedTraining, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var inRoleEditPast = User.IsInRole(AccessesNames.AUTH_scheduler_edit_past);
            var result = await _scheduleService.PatchTraining(customerId, patchedTraining, inRoleEditPast, clt);
            if (result.Success)
                await _auditService.Log(userId, AuditType.PatchTraining, customerId, objectKey: patchedTraining.TrainingId, objectName: patchedTraining.Name);
            else
            {
                await _auditService.Log(userId, AuditType.PatchTraining, customerId, "Failed", patchedTraining.TrainingId, patchedTraining.Name);
                return BadRequest();
            }

            await _userService.PatchLastOnline(userId, clt);
            clt = CancellationToken.None;
            await PatchTrainingCalenderUsers(patchedTraining.TrainingId!.Value, customerId, clt);
            await _userService.PatchLastOnline(userId, clt);

            return result;
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddTraining");
            return BadRequest();
        }
    }

    private async Task PatchTrainingCalenderUsers(Guid trainingId, Guid customerId, CancellationToken clt)
    {
        var training = await _scheduleService.GetPlannedTrainingById(customerId, trainingId, clt);
        clt.ThrowIfCancellationRequested();

        _graphService.InitializeGraph();
        if (training.Training?.PlanUsers.Count > 0)
        {
            foreach (var user in training.Training.PlanUsers)
            {
                await _refreshHub.SendMessage(user.UserId, ItemUpdated.FutureTrainings);
                if (!string.IsNullOrEmpty(user.CalendarEventId))
                {
                    DrogeFunction? function = null;
                    if (user.PlannedFunctionId is not null && user.UserFunctionId is not null && user.UserFunctionId != user.PlannedFunctionId)
                        function = await _functionService.GetById(customerId, user.PlannedFunctionId.Value, clt);
                    var text = GetTrainingCalenderText(training.Training.TrainingTypeName, training.Training.Name, function?.Name);
                    await _graphService.PatchCalender(user.UserId, user.CalendarEventId, text, training.Training.DateStart, training.Training.DateEnd, !training.Training.ShowTime);
                }
            }
        }
    }

    private static string GetTrainingCalenderText(string? trainingTypeName, string? trainingName, string? functionName)
    {
        var text = new StringBuilder();
        if (!string.IsNullOrEmpty(trainingTypeName))
            text.Append(trainingTypeName);
        if (!string.IsNullOrEmpty(trainingTypeName) && !string.IsNullOrEmpty(trainingName))
            text.Append(" - ");
        if (!string.IsNullOrEmpty(trainingName))
            text.Append(trainingName);
        if (!string.IsNullOrEmpty(functionName))
            text.Append(" - ").Append(functionName);
        return text.ToString();
    }

    [HttpPost]
    [Route("training")]
    public async Task<ActionResult<AddTrainingResponse>> AddTraining([FromBody] PlannedTraining newTraining, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var trainingId = Guid.NewGuid();
            var result = await _scheduleService.AddTrainingAsync(customerId, newTraining, trainingId, clt);
            await _userService.PatchLastOnline(userId, clt);
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
            var userId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _scheduleService.PatchScheduleForUserAsync(userId, customerId, training, clt);
            if (result is { Success: true, PatchedTraining.Assigned: true })
            {
                await _auditService.Log(userId, AuditType.PatchAssignedUser, customerId,
                    JsonSerializer.Serialize(new AuditAssignedUser
                    {
                        UserId = userId, Assigned = result.PatchedTraining.Assigned, Availability = result.PatchedTraining.Availability, SetBy = result.PatchedTraining.SetBy,
                        AuditReason = AuditReason.ChangeAvailability
                    }),
                    training.TrainingId);
                await _refreshHub.SendMessage(userId, ItemUpdated.FutureTrainings);
            }

            await _userService.PatchLastOnline(userId, clt);
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
            var userId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var inRoleEditOther = User.IsInRole(AccessesNames.AUTH_scheduler_other_user);
            if (!inRoleEditOther && !userId.Equals(body.User?.UserId))
                return Unauthorized();
            var result = await _scheduleService.PatchAssignedUserAsync(userId, customerId, body, clt);
            await _auditService.Log(userId, AuditType.PatchAssignedUser, customerId,
                JsonSerializer.Serialize(new AuditAssignedUser
                {
                    UserId = body.User?.UserId, Assigned = body.User?.Assigned, Availability = result?.Availability, SetBy = result?.SetBy,
                    AuditReason = body.AuditReason ?? AuditReason.Assigned, // body.AuditReason was added in v0.3.81
                    VehicleId = body.AuditReason == AuditReason.ChangeVehicle ? body.User?.VehicleId : null,
                    FunctionId = body.AuditReason == AuditReason.ChangedFunction ? body.User?.PlannedFunctionId : null
                }),
                body.TrainingId);
            await _userService.PatchLastOnline(userId, clt);
            if (result?.Success == true && body.User?.UserId is not null)
            {
                clt = CancellationToken.None;
                DrogeFunction? function = null;
                if (body.User.PlannedFunctionId is not null && body.User.UserFunctionId is not null && body.User.UserFunctionId != body.User.PlannedFunctionId)
                    function = await _functionService.GetById(customerId, body.User.PlannedFunctionId.Value, clt);
                await ToOutlookCalendar(body.User.UserId, body.TrainingId, body.User.Assigned, body.Training, userId, customerId, result.AvailableId, result.CalendarEventId, function?.Name, clt);
                await _refreshHub.SendMessage(body.User.UserId, ItemUpdated.FutureTrainings);
            }

            if (result is not null)
                return result;
            return BadRequest();
        }
        catch (OperationCanceledException)
        {
            return Ok();
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
            var userId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var inRoleEditOther = User.IsInRole(AccessesNames.AUTH_scheduler_other_user);
            if (!inRoleEditOther && !userId.Equals(body.UserId))
                return Unauthorized();
            var result = await _scheduleService.PutAssignedUserAsync(userId, customerId, body, clt);
            await _auditService.Log(userId, AuditType.PatchAssignedUser, customerId,
                JsonSerializer.Serialize(new AuditAssignedUser
                {
                    UserId = body.UserId, Assigned = body.Assigned, Availability = result.Availability, SetBy = result.SetBy,
                    AuditReason = AuditReason.Assigned
                }),
                body.TrainingId);
            await _userService.PatchLastOnline(userId, clt);
            if (result.Success && body.UserId is not null)
            {
                clt = CancellationToken.None;
                var user = await _userService.GetUserById(body.UserId.Value, clt);
                DrogeFunction? function = null;
                if (body.Training?.PlannedFunctionId is not null && user?.UserFunctionId is not null && user.UserFunctionId != body.Training.PlannedFunctionId)
                    function = await _functionService.GetById(customerId, body.Training.PlannedFunctionId.Value, clt);
                await ToOutlookCalendar(body.UserId.Value, body.TrainingId, body.Assigned, body.Training, userId, customerId, result.AvailableId, result.CalendarEventId, function?.Name, clt);
                await _refreshHub.SendMessage(body.UserId.Value, ItemUpdated.FutureTrainings);
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PutAssignedUser");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("me/scheduled-trainings/{callHub:bool}/{take:int}/{skip:int}", Order = 0)]
    [Route("me/scheduled-trainings/{callHub:bool}", Order = 1)] //ToDo Remove when all users on v0.4.5 or above
    public async Task<ActionResult<GetScheduledTrainingsForUserResponse>> GetScheduledTrainingsForUser(bool callHub = false, int take = 10, int skip = 0, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var fromDate = _dateTimeService.Today().ToUniversalTime();
            var result = await _scheduleService.GetScheduledTrainingsForUser(userId, customerId, fromDate, take, skip, OrderAscDesc.Asc, clt);
            if (callHub)
            {
                _logger.LogTrace("Calling hub futureTrainings");
                await _refreshHub.SendMessage(userId, ItemUpdated.FutureTrainings);
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetScheduledTrainingsForUser");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("training/user/all/{id:guid}/{take:int}/{skip:int}", Order = 0)]
    [Route("training/user/all/{id:guid}", Order = 1)] //ToDo Remove when all users on v0.4.5 or above
    [Authorize(Roles = AccessesNames.AUTH_users_details)]
    public async Task<ActionResult<GetScheduledTrainingsForUserResponse>> AllTrainingsForUser(Guid id, int take = 10, int skip = 0, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _scheduleService.GetScheduledTrainingsForUser(id, customerId, null, take, skip, OrderAscDesc.Desc, clt);
            return result;
        }
        catch (OperationCanceledException)
        {
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AllTrainingsForUser");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("me/pinned/{callHub:bool}", Order = 0)]
    [Route("me/pinned", Order = 1)]
    public async Task<ActionResult<GetPinnedTrainingsForUserResponse>> GetPinnedTrainingsForUser(bool callHub = false, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var fromDate = DateTime.UtcNow;
            var result = await _scheduleService.GetPinnedTrainingsForUser(userId, customerId, fromDate, clt);
            if (callHub)
            {
                _logger.LogTrace("Calling hub PinnedDashboard");
                await _refreshHub.SendMessage(userId, ItemUpdated.PinnedDashboard);
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            return Ok();
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
            var userId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var training = await _scheduleService.GetPlannedTrainingById(customerId, id, clt);
            await _userService.PatchLastOnline(userId, clt);
            if (training.Training is null)
                return BadRequest("training not found");
            clt.ThrowIfCancellationRequested();
            clt = CancellationToken.None;
            var canEditPast = User.IsInRole(AccessesNames.AUTH_scheduler_edit_past);
            if (!canEditPast && training.Training.DateEnd < DateTime.UtcNow.AddDays(AccessesSettings.AUTH_scheduler_edit_past_days))
                return Unauthorized();
            foreach (var user in training.Training.PlanUsers)
            {
                if (!user.Assigned)
                    continue;
                user.Assigned = false;
                var body = new PatchAssignedUserRequest
                {
                    TrainingId = id,
                    User = user,
                    Training = training.Training
                };
                await PatchAssignedUser(body, clt);
                await _refreshHub.SendMessage(user.UserId, ItemUpdated.FutureTrainings);
            }

            var result = await _scheduleService.DeleteTraining(userId, customerId, id, clt);
            await _auditService.Log(userId, AuditType.DeleteTraining, customerId, null, id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteTraining");
            return BadRequest();
        }
    }

    private async Task ToOutlookCalendar(Guid planUserId, Guid? trainingId, bool assigned, TrainingAdvance? training, Guid currentUserId, Guid customerId, Guid? availableId, string? calendarEventId,
        string? functionName, CancellationToken clt)
    {
        try
        {
            if (assigned && await _userSettingService.TrainingToCalendar(customerId, planUserId))
            {
#if DEBUG
                // Be carefully when debugging.
                Debugger.Break();
#endif
                if (training is null && trainingId is not null)
                    training = (await _scheduleService.GetTrainingById(planUserId, customerId, trainingId.Value, clt)).Training;
                var type = await _trainingTypesService.GetById(training?.RoosterTrainingTypeId ?? Guid.Empty, customerId, clt);
                var text = GetTrainingCalenderText(type.TrainingType?.Name, training?.Name, functionName);
                if (training is null)
                {
                    _logger.LogWarning("Failed to set a training for trainingId {trainingId}", trainingId);
                    return;
                }

                _graphService.InitializeGraph();
                if (string.IsNullOrEmpty(calendarEventId))
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        var eventResult = await _graphService.AddToCalendar(planUserId, text, training.DateStart, training.DateEnd, !training.ShowTime);
                        if (eventResult is not null)
                            await _scheduleService.PatchEventIdForUserAvailible(planUserId, customerId, availableId, eventResult.Id, clt);
                    }
                    else
                        _logger.LogWarning(
                            "text is null or empty for {customerId} {planUserId} {trainingId} {assigned} {currentUserId} {availableId} {calendarEventId}",
                            customerId, planUserId, trainingId, assigned, currentUserId, availableId, calendarEventId);
                }
                else
                {
                    await _graphService.PatchCalender(planUserId, calendarEventId, text, training.DateStart, training.DateEnd, !training.ShowTime);
                }
            }
            else if (!string.IsNullOrEmpty(calendarEventId))
            {
                _graphService.InitializeGraph();
                await _graphService.DeleteCalendarEvent(planUserId, calendarEventId, clt);
                await _scheduleService.PatchEventIdForUserAvailible(planUserId, customerId, availableId, null, clt);
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in ToOutlookCalendar {customerId} {planUserId} {trainingId} {assigned} {currentUserId} {availableId} {calendarEventId} and training is {trainingNullOrNot}",
                customerId, planUserId, trainingId, assigned, currentUserId, availableId, calendarEventId, training is null ? "null" : "not null");
        }
    }
}