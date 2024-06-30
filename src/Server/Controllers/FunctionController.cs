using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Server.Hubs;

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
    private readonly RefreshHub _refreshHub;

    public FunctionController(ILogger<FunctionController> logger, IFunctionService functionService, RefreshHub refreshHub)
    {
        _logger = logger;
        _functionService = functionService;
        _refreshHub = refreshHub;
    }

    [HttpPut]
    [Route("")]
    public async Task<ActionResult<AddFunctionResponse>> AddFunction(DrogeFunction function, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
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
    [Route("all/{callHub:bool}", Order = 0)]
    [Route("", Order = 1)]// from version v0.3.41 and older
    public async Task<ActionResult<MultipleFunctionsResponse>> GetAll(bool callHub = false, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _functionService.GetAllFunctions(customerId, clt);
            if (callHub)
            {
                _logger.LogTrace("Calling hub AllFunctions");
                await _refreshHub.SendMessage(userId, ItemUpdated.AllFunctions);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAll functions");
            return BadRequest();
        }
    }
}
