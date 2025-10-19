using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "DefaultSchedule")]
public class DefaultScheduleController : ControllerBase
{
    private readonly ILogger<DefaultScheduleController> _logger;
    private readonly IDefaultScheduleService _defaultScheduleService;
    private readonly IAuditService _auditService;
    private readonly IScheduleService _scheduleService;

    public DefaultScheduleController(
        ILogger<DefaultScheduleController> logger,
        IDefaultScheduleService defaultscheduleService,
        IAuditService auditService,
        IScheduleService scheduleService)
    {
        _logger = logger;
        _defaultScheduleService = defaultscheduleService;
        _auditService = auditService;
        _scheduleService = scheduleService;
    }

    [HttpDelete]
    [Route("config")]
    public async Task<ActionResult<DeleteResponse>> DeleteDefaultSchedule(Guid id, CancellationToken token = default)
    {
        return new DeleteResponse();
    }

    [HttpGet]
    [Route("groups")]
    public async Task<ActionResult<GetAllDefaultGroupsResponse>> GetAllGroups(CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _defaultScheduleService.GetAllDefaultGroupsForUser(customerId, userId);

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetAllGroups default schedules");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("group/{id:guid}")]
    public async Task<ActionResult<MultipleDefaultSchedulesResponse>> GetAllByGroupId(Guid id, CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _defaultScheduleService.GetAllDefaultsForUser(customerId, userId, id);

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetAllByGroupId default schedules");
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("group")]
    public async Task<ActionResult<PutGroupResponse>> PutGroup([FromBody] DefaultGroup body)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            PutGroupResponse result = await _defaultScheduleService.PutGroup(body, customerId, userId);

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PutGroup");
            return BadRequest();
        }
    }


    [HttpPut]
    [Route("schedule")]
    public async Task<ActionResult<PutDefaultScheduleResponse>> PutDefaultSchedule([FromBody] DefaultSchedule body)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _defaultScheduleService.PutDefaultSchedule(body, customerId, userId);

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PutDefaultSchedule");
            return BadRequest();
        }
    }


    [HttpPatch]
    [Route("schedule")]
    public async Task<ActionResult<PatchDefaultScheduleResponse>> PatchDefaultSchedule([FromBody] DefaultSchedule body)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _defaultScheduleService.PatchDefaultSchedule(body, customerId, userId);

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PutDefaultSchedule");
            return BadRequest();
        }
    }


    [HttpGet]
    [Authorize(Roles = AccessesNames.AUTH_configure_default_schedule)]
    [Route("schedule/all")]
    public async Task<ActionResult<GetAllDefaultScheduleResponse>> GetAllDefaultSchedule(CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _defaultScheduleService.GetAllDefaultSchedule(customerId, userId, clt);

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PutDefaultSchedule");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("user")]
    public async Task<ActionResult<PatchDefaultScheduleForUserResponse>> PatchDefaultScheduleForUser([FromBody] PatchDefaultUserSchedule body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));

            var oldDefault = await _defaultScheduleService.GetDefaultScheduleById(customerId, userId, body.UserDefaultAvailableId, clt);
            var validFrom = DateTime.Compare(oldDefault?.ValidFromUser ?? DateTime.MaxValue, body.ValidFromUser ?? DateTime.MaxValue) > 0 ? oldDefault?.ValidFromUser : body.ValidFromUser;
            var validUntil = DateTime.Compare(oldDefault?.ValidUntilUser ?? DateTime.MaxValue, body.ValidUntilUser ?? DateTime.MaxValue) > 0 ? oldDefault?.ValidUntilUser : body.ValidUntilUser;
            GetScheduledTrainingsForUserResponse? trainingsForUserBefore = null;
            if (oldDefault?.DefaultId is not null)
            {
                trainingsForUserBefore = await _scheduleService.GetScheduledTrainingsForUser(userId, customerId, oldDefault.DefaultId, validFrom, 1000, 0, OrderAscDesc.Asc, clt);
            }

            var result = await _defaultScheduleService.PatchDefaultScheduleForUser(body, customerId, userId);
            GetScheduledTrainingsForUserResponse? trainingsForUserAfter = null;
            if (result.Patched?.DefaultId is not null)
            {
                 trainingsForUserAfter = await _scheduleService.GetScheduledTrainingsForUser(userId, customerId, result.Patched.DefaultId, validFrom, 1000, 0, OrderAscDesc.Asc, clt);
            }

            if (!result.Success || trainingsForUserAfter is null || result.Patched is null) return result;
            
            foreach (var training in trainingsForUserAfter.Trainings.Where(x=>x.DateStart >= validFrom && x.DateEnd <= validUntil))
            {
                var userInTraining = training.PlanUsers.FirstOrDefault(x => x.UserId == userId && x.Assigned);
                if (userInTraining is null || userInTraining.SetBy == AvailabilitySetBy.User || userInTraining.SetBy == AvailabilitySetBy.Holiday)
                {
                    continue;
                }
                var oldUserInTraining = trainingsForUserBefore?.Trainings
                    .FirstOrDefault(x => x.TrainingId == training.TrainingId)
                    ?.PlanUsers.FirstOrDefault(x => x.UserId == userId && x.Assigned);
                Availability? availability;
                AvailabilitySetBy? setBy;

                if (training.DateStart >= result.Patched.ValidFromUser && training.DateEnd <= result.Patched.ValidUntilUser)
                {
                    if (oldUserInTraining?.Availability == body.Availability)
                        continue;
                    availability = body.Availability;
                    setBy = AvailabilitySetBy.DefaultAvailable;
                }
                else
                {
                    if (userInTraining.Availability == oldUserInTraining?.Availability)
                        continue;
                    availability = userInTraining.Availability;
                    setBy = userInTraining.SetBy;
                }
            
                await _auditService.Log(userId,
                    AuditType.PatchAssignedUser,
                    customerId,
                    JsonSerializer.Serialize(new AuditAssignedUser
                    {
                        UserId = userId,
                        Assigned = true,
                        Availability = availability,
                        SetBy = setBy,
                        AuditReason = AuditReason.ChangeDefaultAvailable,
                    }),
                    training.TrainingId);
            }

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PatchDefaultScheduleForUser");
            return BadRequest();
        }
    }
}
