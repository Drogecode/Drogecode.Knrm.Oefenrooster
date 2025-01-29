using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Setting;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
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
    [Obsolete("Use GetStringSetting(SettingName.TimeZone)")] // ToDo Remove when all users on v0.4.32 or above
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
    [Route("training-string/{name}")]
    public async Task<ActionResult<SettingStringResponse>> GetStringSetting(SettingName name, CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            SettingStringResponse result;
            switch (name)
            {
                case SettingName.TimeZone:
                    result = new SettingStringResponse()
                    {
                        Value = await _customerSettingService.GetTimeZone(customerId),
                        Success = true
                    };
                    break;
                case SettingName.CalendarPrefix:
                    result = await _customerSettingService.GetStringCustomerSetting(customerId, name, string.Empty);
                    break;
                default:
                    return BadRequest("Not bool");
            }

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
    [Route("training-bool/{name}")]
    public async Task<ActionResult<SettingBoolResponse>> GetBoolSetting(SettingName name, CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            SettingBoolResponse result;
            switch (name)
            {
                case SettingName.TrainingToCalendar:
                    result = await _customerSettingService.GetBoolCustomerSetting(customerId, name);
                    break;
                default:
                    return BadRequest("Not bool");
            }

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
    [Route("training-string")]
    [Authorize(Roles = AccessesNames.AUTH_configure_global_all)]
    public async Task<ActionResult> PatchStringSetting([FromBody] PatchSettingStringRequest body, CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            await _customerSettingService.PatchStringSetting(customerId, body.Name, body.Value);
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

    [HttpPatch]
    [Route("training-bool")]
    [Authorize(Roles = AccessesNames.AUTH_configure_global_all)]
    public async Task<ActionResult> PatchBoolSetting([FromBody] PatchSettingBoolRequest body, CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            await _customerSettingService.PatchBoolSetting(customerId, body.Name, body.Value);
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