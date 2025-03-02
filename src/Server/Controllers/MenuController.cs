using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Menu;
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
[ApiExplorerSettings(GroupName = "Menu")]
public class MenuController : ControllerBase
{
    private readonly ILogger<HolidayController> _logger;
    private readonly IMenuService _menuService;

    public MenuController(
        ILogger<HolidayController> logger,
        IMenuService menuService)
    {
        _logger = logger;
        _menuService = menuService;
    }

    [HttpGet]
    [Route("all")]
    public async Task<ActionResult<MultipleMenuResponse>> GetAll(CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _menuService.GetAllMenus(customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in menu GetAll");
            return BadRequest();
        }
    }
}