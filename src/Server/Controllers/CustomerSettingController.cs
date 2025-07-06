using Drogecode.Knrm.Oefenrooster.Server.Controllers.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "CustomerSettings")]
public class CustomerSettingController : DrogeController
{
    private readonly ICustomerSettingService _customerSettingService;

    public CustomerSettingController(ILogger<CustomerSettingController> logger, ICustomerSettingService customerSettingService) : base(logger)
    {
        _customerSettingService = customerSettingService;
    }

    [HttpGet]
    [Route("string/{name}", Order = 0)]
    [Route("training-string/{name}", Order = 1)] // ToDo Remove when all users on v0.5.12 or above
    public async Task<ActionResult<SettingStringResponse>> GetStringSetting(SettingName name, CancellationToken clt = default)
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
                case SettingName.PreComAvailableText:
                    result = await _customerSettingService.GetStringCustomerSetting(customerId, name, string.Empty, clt);
                    break;
                default:
                    return BadRequest("Not string");
            }

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in GetStringSetting");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("bool/{name}", Order = 0)]
    [Route("training-bool/{name}", Order = 1)] // ToDo Remove when all users on v0.5.12 or above
    public async Task<ActionResult<SettingBoolResponse>> GetBoolSetting(SettingName name, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            SettingBoolResponse result;
            switch (name)
            {
                case SettingName.TrainingToCalendar:
                case SettingName.SyncPreComWithCalendar:
                case SettingName.SyncPreComDeleteOld:
                case SettingName.DelaySyncingTrainingToOutlook:
                    result = await _customerSettingService.GetBoolCustomerSetting(customerId, name, false, clt);
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
            Logger.LogError(ex, "Exception in GetBoolSetting");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("int/{name}", Order = 0)]
    public async Task<ActionResult<SettingIntResponse>> GetIntSetting(SettingName name, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            SettingIntResponse result;
            switch (name)
            {
                case SettingName.PreComDaysInFuture:
                    result = await _customerSettingService.GetIntCustomerSetting(customerId, name, -2, clt);
                    break;
                default:
                    return BadRequest("Not int");
            }

            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in GetIntSetting");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("string")]
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
            Logger.LogError(ex, "Exception in PatchStringSetting");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("bool")]
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
            Logger.LogError(ex, "Exception in PatchBoolSetting");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("int")]
    [Authorize(Roles = AccessesNames.AUTH_configure_global_all)]
    public async Task<ActionResult> PatchIntSetting([FromBody] PatchSettingIntRequest body, CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            await _customerSettingService.PatchIntSetting(customerId, body.Name, body.Value);
            return Ok();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in PatchIntSetting");
            return BadRequest();
        }
    }
}