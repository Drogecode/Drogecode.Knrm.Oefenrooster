﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class VehicleController : ControllerBase
{
    private readonly ILogger<VehicleController> _logger;
    private readonly IVehicleService _vehicleService;
    private readonly IAuditService _auditService;

    public VehicleController(ILogger<VehicleController> logger, IVehicleService vehicleService, IAuditService auditService)
    {
        _logger = logger;
        _vehicleService = vehicleService;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DrogeVehicle>>> GetAll()
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _vehicleService.GetAllVehicles(customerId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAll");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<DrogeLinkVehicleTraining>>> GetForTraining(Guid trainingId)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _vehicleService.GetForTraining(customerId, trainingId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetForTraining");
            return BadRequest();
        }
    }
}
