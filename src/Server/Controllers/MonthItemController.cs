using Drogecode.Knrm.Oefenrooster.Server.Controllers.Obsolite;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;
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
[ApiExplorerSettings(GroupName = "MonthItem")]
public class MonthItemController : ControllerBase
{
    private readonly ILogger<MonthItemController> _logger;
    private readonly IMonthItemService _monthItemService;

    public MonthItemController(
        ILogger<MonthItemController> logger,
        IConfiguration configuration,
        IMonthItemService monthItemService,
        IAuditService auditService,
        IUserSettingService userSettingService,
        IGraphService graphService)
    {
        _logger = logger;
        _monthItemService = monthItemService;
    }

    [HttpGet]
    [Route("{year:int}/{month:int}")]
    public async Task<ActionResult<GetMultipleMonthItemResponse>> GetItems(int year, int month, CancellationToken clt = default)
    {
        try
        {
            var result = new GetMultipleMonthItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            result = await _monthItemService.GetItems(year, month, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in month GetItems");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("all/{take:int}/{skip:int}/{includeExpired:bool}")]
    public async Task<ActionResult<GetMultipleMonthItemResponse>> GetAllItems(int take, int skip, bool includeExpired, CancellationToken clt = default)
    {
        try
        {
            var result = new GetMultipleMonthItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            result = await _monthItemService.GetAllItems(take, skip, includeExpired, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in month GetAllItems");
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("")]
    public async Task<ActionResult<PutMonthItemResponse>> PutItem([FromBody] RoosterItemMonth roosterItemMonth, CancellationToken clt = default)
    {
        try
        {
            var result = new PutMonthItemResponse();
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            result = await _monthItemService.PutItem(roosterItemMonth, customerId, userId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in month PutItem");
            return BadRequest();
        }
    }
}
