using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "DefaultSchedule")]
public class DefaultScheduleController : ControllerBase
{
    private readonly ILogger<DefaultScheduleController> _logger;
    private readonly IDefaultScheduleService _defaultScheduleService;
    private readonly IAuditService _auditService;

    public DefaultScheduleController(
        ILogger<DefaultScheduleController> logger,
        IDefaultScheduleService defaultscheduleService,
        IAuditService auditService)
    {
        _logger = logger;
        _defaultScheduleService = defaultscheduleService;
        _auditService = auditService;
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
    public async Task<ActionResult<PatchDefaultScheduleForUserResponse>> PatchDefaultScheduleForUser([FromBody] PatchDefaultUserSchedule body)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _defaultScheduleService.PatchDefaultScheduleForUser(body, customerId, userId);

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
