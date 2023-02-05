using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;
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

    public ScheduleController(
        ILogger<ScheduleController> logger,
        IScheduleService scheduleService)
    {
        _logger = logger;
        _scheduleService = scheduleService;
    }

    [HttpGet]
    public async Task<ActionResult<ScheduleForUserResponse>> ForUser(int relativeWeek, CancellationToken token)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            ScheduleForUserResponse result = await _scheduleService.ScheduleForUserAsync(userId, customerId, relativeWeek, token);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForUser");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<ScheduleForAllResponse>> ForAll(int relativeWeek, CancellationToken token)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            ScheduleForAllResponse result = await _scheduleService.ScheduleForAllAsync(userId, customerId, relativeWeek, token);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForAll");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult<Training>> Patch(Training training, CancellationToken token)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _scheduleService.PatchTrainingAsync(userId, customerId, training, token);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Patch");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult> PatchScheduleUser(PatchScheduleUserRequest body, CancellationToken token)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            await _scheduleService.PatchScheduleUserAsync(userId, customerId, body, token);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Patch");
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
            _logger.LogError(ex, "Error in Patch");
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
            var fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-7));
            var result = await _scheduleService.GetScheduledTrainingsForUser(userId, customerId, fromDate, token);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Patch");
            return BadRequest();
        }
    }
}
