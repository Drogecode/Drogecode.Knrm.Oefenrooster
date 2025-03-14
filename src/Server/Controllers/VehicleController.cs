﻿using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
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
[ApiExplorerSettings(GroupName = "Vehicle")]
public class VehicleController : ControllerBase
{
    private readonly ILogger<VehicleController> _logger;
    private readonly IVehicleService _vehicleService;
    private readonly RefreshHub _refreshHub;

    public VehicleController(ILogger<VehicleController> logger, IVehicleService vehicleService, RefreshHub refreshHub)
    {
        _logger = logger;
        _vehicleService = vehicleService;
        _refreshHub = refreshHub;
    }

    [HttpGet]
    [Route("all/{callHub:bool}")]
    public async Task<ActionResult<MultipleVehicleResponse>> GetAll(bool callHub = false, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _vehicleService.GetAllVehicles(customerId);

            if (callHub)
            {
                _logger.LogTrace("Calling hub AllVehicles");
                await _refreshHub.SendMessage(userId, ItemUpdated.AllVehicles);
            }
            return  result ;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAll vehicles");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("training/{trainingId:guid}")]
    public async Task<ActionResult<MultipleVehicleTrainingLinkResponse>> GetForTraining(Guid trainingId, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _vehicleService.GetForTraining(customerId, trainingId, clt);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetForTraining");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("default/{defaultId:guid}")]
    public async Task<ActionResult<MultipleVehicleTrainingLinkResponse>> GetForDefault(Guid defaultId, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _vehicleService.GetForDefault(customerId, defaultId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetForTraining");
            return BadRequest();
        }
    }

    [HttpPut]
    [Authorize(Roles = AccessesNames.AUTH_configure_vehicles)]
    [Route("")]
    public async Task<ActionResult<PutResponse>> PutVehicle(DrogeVehicle vehicle, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _vehicleService.PutVehicle(vehicle, customerId, userId, clt);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in PutVehicle");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Authorize(Roles = AccessesNames.AUTH_configure_vehicles)]
    [Route("")]
    public async Task<ActionResult<PatchResponse>> PatchVehicle([FromBody] DrogeVehicle vehicle, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _vehicleService.PatchVehicle(vehicle, customerId, userId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PatchVehicle");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("link-vehicle-training")]
    public async Task<ActionResult<DrogeLinkVehicleTrainingResponse>> UpdateLinkVehicleTraining([FromBody] DrogeLinkVehicleTraining link, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _vehicleService.UpdateLinkVehicleTraining(customerId, link);

            return  result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpdateLinkVehicleTraining");
            return BadRequest();
        }
    }
}
