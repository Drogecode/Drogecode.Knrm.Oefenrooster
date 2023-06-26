using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "Function")]

public class FunctionController : ControllerBase
{
    private readonly ILogger<FunctionController> _logger;
    private readonly IFunctionService _functionService;

    public FunctionController(ILogger<FunctionController> logger, IFunctionService functionService)
    {
        _logger = logger;
        _functionService = functionService;
    }

    [HttpPut]
    [Route("")]
    public async Task<ActionResult<AddFunctionResponse>> AddFunction(DrogeFunction function, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            AddFunctionResponse result = await _functionService.AddFunction(function, customerId, clt);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in AddFunction");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult<MultipleFunctionsResponse>> GetAll(CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _functionService.GetAllFunctions(customerId, clt);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAll functions");
            return BadRequest();
        }
    }
}
