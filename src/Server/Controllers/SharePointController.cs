﻿using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "SharePoint")]
public class SharePointController : ControllerBase
{
    private readonly ILogger<SharePointController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IAuditService _auditService;
    private readonly IGraphService _graphService;

    public SharePointController(
        ILogger<SharePointController> logger,
        IConfiguration configuration,
        IAuditService auditService,
        IGraphService graphService)
    {
        _logger = logger;
        _configuration = configuration;
        _auditService = auditService;
        _graphService = graphService;
    }

    [Obsolete("Should not be used anymore and will be deleted in a future version")]
    [HttpGet]
    [Route("training/user/{count:int}/{skip:int}")]
    public async Task<ActionResult<MultipleSharePointTrainingsResponse>> GetLastTrainingsForCurrentUser(int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 30) return Forbid();
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var users = new List<Guid?>() { userId };

            _graphService.InitializeGraph();
            var result = await _graphService.GetListTrainingUser(users, userId, count, skip, customerId, clt);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastTrainingsForCurrentUser");
            return BadRequest();
        }
    }

    [Obsolete("Should not be used anymore and will be deleted in a future version")]
    [HttpGet]
    [Route("training/{users}/{count:int}/{skip:int}")]
    public async Task<ActionResult<MultipleSharePointTrainingsResponse>> GetLastTrainings(string users, int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 30) return Forbid();
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var usersAsList = System.Text.Json.JsonSerializer.Deserialize<List<Guid?>>(users);
            if (usersAsList is null)
                return BadRequest("users is null");

            _graphService.InitializeGraph();
            var result = await _graphService.GetListTrainingUser(usersAsList, userId, count, skip, customerId, clt);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastTrainings");
            return BadRequest();
        }
    }

    [Obsolete("Should not be used anymore and will be deleted in a future version")]
    [HttpGet]
    [Route("action/user/{count:int}/{skip:int}")]
    public async Task<ActionResult<MultipleSharePointActionsResponse>> GetLastActionsForCurrentUser(int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 30) return Forbid();
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var users = new List<Guid?>() { userId };

            _graphService.InitializeGraph();
            var result = await _graphService.GetListActionsUser(users, userId, count, skip, customerId, clt);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastActionsForCurrentUser");
            return BadRequest();
        }
    }

    [Obsolete("Should not be used anymore and will be deleted in a future version")]
    [HttpGet]
    [Route("action/{users}/{count:int}/{skip:int}")]
    public async Task<ActionResult<MultipleSharePointActionsResponse>> GetLastActions(string users, int count, int skip, CancellationToken clt = default)
    {
        try
        {
            if (count > 30) return Forbid();
            var userId = new Guid(User?.FindFirstValue("ExternalUserId") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var usersAsList = System.Text.Json.JsonSerializer.Deserialize<List<Guid?>>(users);
            if (usersAsList is null)
                return BadRequest("users is null");

            _graphService.InitializeGraph();
            var result = await _graphService.GetListActionsUser(usersAsList, userId, count, skip, customerId, clt);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetLastActions");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("historical")]
    [Authorize(Roles = AccessesNames.AUTH_super_user)]
    public async Task<ActionResult<GetHistoricalResponse>> SyncHistorical(CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));

            _graphService.InitializeGraph();
            var result = await _graphService.SyncHistorical(customerId, clt);
            return Ok(result);
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in SyncHistorical");
            return BadRequest();
        }
    }
}