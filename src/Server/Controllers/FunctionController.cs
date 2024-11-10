using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;

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
    [Authorize(Roles = AccessesNames.AUTH_configure_user_functions)]
    public async Task<ActionResult<AddFunctionResponse>> AddFunction(DrogeFunction function, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _functionService.AddFunction(function, customerId, clt);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in AddFunction");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Authorize(Roles = AccessesNames.AUTH_configure_user_functions)]
    [Route("")]
    public async Task<ActionResult<PatchResponse>> PatchFunction([FromBody] DrogeFunction function, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _functionService.PatchFunction(customerId, function, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PatchFunction");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("all/{callHub:bool}", Order = 0)]
    [Route("", Order = 1)] // from version v0.4.17 and older
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

    [HttpGet]
    [Authorize(Roles = AccessesNames.AUTH_configure_user_roles)]
    [Route("{id:guid}")]
    public async Task<ActionResult<GetFunctionResponse>> GetById(Guid id, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var sw = Stopwatch.StartNew();
            var result = new GetFunctionResponse
            {
                Function = await _functionService.GetById(customerId, id, clt),
                Success = true
            };
            sw.Stop();
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAll");
            return BadRequest();
        }
    }
}