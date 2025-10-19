using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "Holiday")]
public class HolidayController : ControllerBase
{
    private readonly ILogger<HolidayController> _logger;
    private readonly IHolidayService _holidayService;
    private readonly IAuditService _auditService;
    private readonly IScheduleService _scheduleService;
    private readonly RefreshHub _refreshHub;

    public HolidayController(
        ILogger<HolidayController> logger,
        IHolidayService holidayService,
        IAuditService auditService,
        IScheduleService scheduleService,
        RefreshHub refreshHub)
    {
        _logger = logger;
        _holidayService = holidayService;
        _auditService = auditService;
        _scheduleService = scheduleService;
        _refreshHub = refreshHub;
    }

    [HttpGet]
    [Route("all/user")]
    public async Task<ActionResult<MultipleHolidaysResponse>> GetAll(CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _holidayService.GetAllHolidaysForUser(customerId, userId, clt);

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetAll Holidays");
            return BadRequest();
        }
    }

    [HttpGet]
    [Authorize(Roles = AccessesNames.AUTH_dashboard_holidays)]
    [Route("all/future/{days:int}")]
    public async Task<ActionResult<MultipleHolidaysResponse>> GetAllFuture(int days, bool callHub, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            MultipleHolidaysResponse result = await _holidayService.GetAllHolidaysForFuture(customerId, userId, days, clt);

            if (callHub)
            {
                _logger.LogTrace("Calling hub GetAllFuture holidays");
                await _refreshHub.SendMessage(userId, ItemUpdated.FutureHolidays);
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetAll Holidays");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<GetHolidayResponse>> Get(Guid id, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _holidayService.Get(id, customerId, userId, clt);

            return result;
        }
        catch (OperationCanceledException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetAll Holidays");
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("")]
    public async Task<ActionResult<PutHolidaysForUserResponse>> PutHolidayForUser([FromBody] Holiday body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var trainingsForUserBefore = await _scheduleService.GetScheduledTrainingsForUser(userId, customerId, body.ValidFrom, 1000, 0, OrderAscDesc.Asc, clt);
            var result = await _holidayService.PutHolidaysForUser(body, customerId, userId, clt);
            var trainingsForUserAfter = await _scheduleService.GetScheduledTrainingsForUser(userId, customerId, body.ValidFrom, 1000, 0, OrderAscDesc.Asc, clt);
            var holiday = await _holidayService.Get(body.Id, customerId, userId, clt);

            if (result.Success)
            {
                await WriteAuditToTrainingIfRequired(result.Put ?? body, trainingsForUserBefore, trainingsForUserAfter, holiday, userId, customerId, false);
            }

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PutHolidaysForUser");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("")]
    public async Task<ActionResult<PatchHolidaysForUserResponse>> PatchHolidayForUser([FromBody] Holiday body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var holiday = await _holidayService.Get(body.Id, customerId, userId, clt);
            if (holiday.Holiday is null)
            {
                _logger.LogError("Holiday `{id}` not found to patch", body.Id);
                return BadRequest();
            }

            var validFrom = DateTime.Compare(holiday.Holiday.ValidFrom ?? DateTime.MaxValue, body.ValidFrom ?? DateTime.MaxValue) > 0 ? holiday.Holiday.ValidFrom : body.ValidFrom;
            var trainingsForUserBefore = await _scheduleService.GetScheduledTrainingsForUser(userId, customerId, validFrom, 1000, 0, OrderAscDesc.Asc, clt);
            var result = await _holidayService.PatchHolidaysForUser(body, customerId, userId, clt);
            var trainingsForUserAfter = await _scheduleService.GetScheduledTrainingsForUser(userId, customerId, validFrom, 1000, 0, OrderAscDesc.Asc, clt);
            if (result.Success)
            {
                await WriteAuditToTrainingIfRequired(result.Patched ?? body, trainingsForUserBefore, trainingsForUserAfter, holiday, userId, customerId, false);
            }

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PatchHolidayForUser");
            return BadRequest();
        }
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<ActionResult<DeleteResponse>> Delete(Guid id, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var holiday = await _holidayService.Get(id, customerId, userId, clt);
            if (holiday.Holiday is null)
            {
                _logger.LogError("Holiday `{id}` not found to delete", id);
                return BadRequest();
            }

            var trainingsForUserBefore = await _scheduleService.GetScheduledTrainingsForUser(userId, customerId, holiday.Holiday.ValidFrom, 1000, 0, OrderAscDesc.Asc, clt);
            var result = await _holidayService.Delete(id, customerId, userId, clt);
            var trainingsForUserAfter = await _scheduleService.GetScheduledTrainingsForUser(userId, customerId, holiday.Holiday.ValidFrom, 1000, 0, OrderAscDesc.Asc, clt);

            holiday.Holiday.Availability = Availability.None;
            await WriteAuditToTrainingIfRequired(holiday.Holiday, trainingsForUserBefore, trainingsForUserAfter, holiday, userId, customerId, true);

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in Delete holiday");
            return BadRequest();
        }
    }

    private async Task WriteAuditToTrainingIfRequired(Holiday patchedHoliday, GetScheduledTrainingsForUserResponse trainingsForUserBefore, GetScheduledTrainingsForUserResponse trainingsForUserAfter,
        GetHolidayResponse oldVersionHoliday, Guid userId, Guid customerId, bool isDelete)
    {
        if (trainingsForUserBefore.Trainings.Count == 0) return;
        var validFrom = DateTime.Compare(oldVersionHoliday.Holiday?.ValidFrom ?? DateTime.MaxValue, patchedHoliday.ValidFrom ?? DateTime.MaxValue) < 0
            ? oldVersionHoliday.Holiday?.ValidFrom
            : patchedHoliday.ValidFrom;
        var validUntil = DateTime.Compare(oldVersionHoliday.Holiday?.ValidUntil ?? DateTime.MinValue, patchedHoliday.ValidUntil ?? DateTime.MinValue) > 0
            ? oldVersionHoliday.Holiday?.ValidUntil
            : patchedHoliday.ValidUntil;
        foreach (var training in trainingsForUserBefore.Trainings.Where(x => x.DateStart >= validFrom && x.DateEnd <= validUntil))
        {
            var userInTraining = training.PlanUsers.FirstOrDefault(x => x.UserId == userId && x.Assigned);
            if (userInTraining is null || userInTraining.SetBy == AvailabilitySetBy.User)
            {
                continue;
            }

            var newUserInTraining = trainingsForUserAfter.Trainings
                .FirstOrDefault(x => x.TrainingId == training.TrainingId)
                ?.PlanUsers.FirstOrDefault(x => x.UserId == userId && x.Assigned);
            Availability? availability;
            AvailabilitySetBy? setBy;

            if (!isDelete && training.DateStart >= patchedHoliday.ValidFrom && training.DateEnd <= patchedHoliday.ValidUntil)
            {
                availability = patchedHoliday.Availability;
                setBy = AvailabilitySetBy.Holiday;
            }
            else if (newUserInTraining is not null)
            {
                availability = newUserInTraining.Availability;
                setBy = newUserInTraining.SetBy;
            }
            else
            {
                _logger.LogWarning("No user found in training for user `{userId}`", userId);
                continue;
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
                    AuditReason = AuditReason.ChangeHoliday,
                }),
                training.TrainingId);
        }
    }
}