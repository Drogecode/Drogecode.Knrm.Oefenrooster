using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Server.Controllers.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Setting;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "UserSettings")]
public class UserSettingController : DrogeController
{
    private readonly IUserSettingService _userSettingService;

    public UserSettingController(ILogger<CustomerSettingController> logger, IUserSettingService userSettingService) : base(logger)
    {
        _userSettingService = userSettingService;
    }

    [HttpGet]
    [Route("string/{name}")]
    public async Task<ActionResult<SettingStringResponse>> GetStringSetting(SettingName name, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            SettingStringResponse result;
            switch (name)
            {
                case SettingName.CalendarPrefix:
                case SettingName.PreComAvailableText:
                    result = await _userSettingService.GetStringUserSetting(customerId, userId, name);
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
            Logger.LogError(ex, "Exception in GetStringSetting");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("bool/{name}")]
    public async Task<ActionResult<SettingBoolResponse>> GetBoolSetting(SettingName name, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            SettingBoolResponse result;
            switch (name)
            {
                case SettingName.TrainingToCalendar:
                case SettingName.SyncPreComWithCalendar:
                    result = await _userSettingService.GetBoolUserSetting(customerId, userId, name);
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

    [HttpPatch]
    [Route("string")]
    public async Task<ActionResult> PatchStringSetting([FromBody] PatchSettingStringRequest body, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            await _userSettingService.PatchStringSetting(customerId, userId, body.Name, body.Value);
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
    public async Task<ActionResult> PatchBoolSetting([FromBody] PatchSettingBoolRequest body, CancellationToken token = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            await _userSettingService.PatchBoolSetting(customerId, userId, body.Name, body.Value);
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
}
