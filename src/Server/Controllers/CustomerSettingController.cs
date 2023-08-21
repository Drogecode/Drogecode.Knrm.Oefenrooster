using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
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
    private readonly IFunctionService _functionService;
    private readonly ICustomerSettingService _customerSettingService;

    public CustomerSettingController(ILogger<CustomerSettingController> logger, IFunctionService functionService, ICustomerSettingService customerSettingService)
    {
        _logger = logger;
        _functionService = functionService;
        _customerSettingService = customerSettingService;
    }

    [HttpGet]
    [Route("TrainingToCalendar")]
    public async Task<ActionResult<bool>> GetTrainingToCalendar(CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            bool result = await _customerSettingService.TrainingToCalendar(customerId);

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
    [Route("TrainingToCalendar")]
    [Authorize(Roles = AccessesNames.AUTH_Taco)]
    public async Task<ActionResult> PatchTrainingToCalendar(bool newValue, CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
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
