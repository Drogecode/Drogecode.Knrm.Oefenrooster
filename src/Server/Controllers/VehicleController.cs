using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "Vehicle")]
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
    [Route("")]
    public async Task<ActionResult<MultipleVehicleResponse>> GetAll(CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _vehicleService.GetAllVehicles(customerId);

            return Ok(new MultipleVehicleResponse { DrogeVehicles = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetAll");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("training/{trainingId:guid}")]
    public async Task<ActionResult<MultipleVehicleTrainingLinkResponse>> GetForTraining(Guid trainingId, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _vehicleService.GetForTraining(customerId, trainingId);

            return Ok(new MultipleVehicleTrainingLinkResponse { DrogeLinkVehicleTrainingLinks = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetForTraining");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("link-vehicle-training")]
    public async Task<ActionResult<DrogeLinkVehicleTrainingResponse>> UpdateLinkVehicleTraining([FromBody] DrogeLinkVehicleTraining link, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _vehicleService.UpdateLinkVehicleTraining(customerId, link);

            return Ok(new DrogeLinkVehicleTrainingResponse { DrogeLinkVehicleTraining = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpdateLinkVehicleTraining");
            return BadRequest();
        }
    }
}
