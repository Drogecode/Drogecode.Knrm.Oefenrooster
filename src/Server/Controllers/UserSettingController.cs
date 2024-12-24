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
[ApiExplorerSettings(GroupName = "UserSettings")]
public class UserSettingController : ControllerBase
{
    private readonly ILogger<CustomerSettingController> _logger;
    private readonly IUserSettingService _userSettingService;

    public UserSettingController(ILogger<CustomerSettingController> logger, IUserSettingService userSettingService)
    {
        _logger = logger;
        _userSettingService = userSettingService;
    }

    [HttpGet]
    [Route("training-to-calendar")]
    public async Task<ActionResult<bool>> GetTrainingToCalendar(CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            bool result = await _userSettingService.TrainingToCalendar(customerId, userId);

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
    [Authorize(Roles = AccessesNames.AUTH_super_user)]
    public async Task<ActionResult> PatchTrainingToCalendar(bool newValue, CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            await _userSettingService.Patch_TrainingToCalendar(customerId, userId, newValue);
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
