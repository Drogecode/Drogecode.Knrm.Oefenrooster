using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]

public class FunctionController : ControllerBase
{
    private readonly ILogger<FunctionController> _logger;
    private readonly IFunctionService _functionService;

    public FunctionController(ILogger<FunctionController> logger, IFunctionService functionService)
    {
        _logger = logger;
        _functionService = functionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DrogeFunction>>> GetAll(CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _functionService.GetAllFunctions(customerId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpgradeDatabase");
            return BadRequest();
        }
    }
}
