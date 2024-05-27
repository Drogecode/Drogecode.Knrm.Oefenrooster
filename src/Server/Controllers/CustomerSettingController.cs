using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "CustomerSettings")]
public class CustomerSettingController : ControllerBase
{
    private readonly ILogger<CustomerSettingController> _logger;
    private readonly ICustomerSettingService _customerSettingService;

    public CustomerSettingController(ILogger<CustomerSettingController> logger, ICustomerSettingService customerSettingService)
    {
        _logger = logger;
        _customerSettingService = customerSettingService;
    }

    [HttpGet]
    [Route("ios-dark-light-check")]
    public async Task<ActionResult<bool>> GetIosDarkLightCheck(CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _customerSettingService.IosDarkLightCheck(customerId);

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetTrainingToCalendar");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("time-zone")]
    public async Task<ActionResult<string>> GetTimeZone(CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = System.Text.Json.JsonSerializer.Serialize(await _customerSettingService.GetTimeZone(customerId));

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetTrainingToCalendar");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("training-to-calendar")]
    public async Task<ActionResult<bool>> GetTrainingToCalendar(CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _customerSettingService.TrainingToCalendar(customerId);

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetTrainingToCalendar");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("training-to-calendar")]
    [Authorize(Roles = AccessesNames.AUTH_Taco)]
    public async Task<ActionResult> PatchTrainingToCalendar(bool newValue, CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            await _customerSettingService.Patch_TrainingToCalendar(customerId, newValue);
            return Ok();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PatchTrainingToCalendar");
            return BadRequest();
        }
    }
}