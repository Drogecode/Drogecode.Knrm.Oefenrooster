using System.Diagnostics;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportActionShared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "ReportActionShared")]
public class ReportActionSharedController : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger;
    private readonly IReportActionSharedService _reportActionSharedService;
    private readonly IReportActionService _reportActionService;

    public ReportActionSharedController(
        ILogger<ScheduleController> logger,
        IReportActionSharedService reportActionSharedService,
        IReportActionService reportActionService)
    {
        _logger = logger;
        _reportActionSharedService = reportActionSharedService;
        _reportActionService = reportActionService;
    }

    [HttpPut]
    [Route("")]
    [Authorize(Roles = AccessesNames.AUTH_action_share)]
    public async Task<ActionResult<PutReportActionSharedResponse>> PutReportActionShared([FromBody] ReportActionSharedConfiguration body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _reportActionSharedService.PutReportActionShared(body, customerId, userId, clt);
            return result;
        }
        catch (OperationCanceledException)
        {
            return Ok();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PutNewReportActionShared");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("all")]
    [Authorize(Roles = AccessesNames.AUTH_action_share)]
    public async Task<ActionResult<MultipleReportActionShareConfigurationResponse>> GetAllReportActionShared(CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _reportActionSharedService.GetAllReportActionSharedConfiguration(customerId, userId, clt);
            return result;
        }
        catch (OperationCanceledException)
        {
            return Ok();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetAllReportActionShared");
            return BadRequest();
        }
    }
    
    [HttpDelete]
    [Route("{itemId:guid}")]
    [Authorize(Roles = AccessesNames.AUTH_action_share)]
    public async Task<ActionResult<DeleteResponse>> DeleteReportActionShared(Guid itemId, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _reportActionSharedService.DeleteReportActionSharedConfiguration(itemId, customerId, userId, clt);
            return result;
        }
        catch (OperationCanceledException)
        {
            return Ok();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in DeleteReportActionShared");
            return BadRequest();
        }
    }
    

    [HttpGet]
    [Route("actions/{id:guid}/{count:int}/{skip:int}")]
    [Authorize]
    public async Task<ActionResult<MultipleReportActionsResponse>> GetActions(Guid id, int count, int skip, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));

            var config = await _reportActionSharedService.GetReportActionSharedConfiguration(customerId, id, clt);
            if (config is null)
            {
                _logger.LogWarning("GetReportActionSharedConfiguration requested but not found for customer `{customerId}` with id `{id}`", customerId, id);
                return new MultipleReportActionsResponse();
            }

            if (config.ValidUntil < DateTime.UtcNow)
            {
                _logger.LogWarning("GetReportActionSharedConfiguration requested for expired report `{customerId}` with id `{id}`", customerId, id);
                return new MultipleReportActionsResponse();
            }
            var result = await _reportActionService.GetListActionsUser(config.SelectedUsers, config.Types, config.Search, count, skip, customerId, true, config.StartDate, config.EndDate, clt);
            _logger.LogInformation("External requested loading actions {count} skipping {skip} for user {users}", count, skip, config.SelectedUsers);
            return result;
        }
        catch (OperationCanceledException)
        {
            return new MultipleReportActionsResponse();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetLastActions");
            return BadRequest();
        }
    }
}