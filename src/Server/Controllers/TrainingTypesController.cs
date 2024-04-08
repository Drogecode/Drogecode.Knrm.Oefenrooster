using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
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
[ApiExplorerSettings(GroupName = "TrainingTypes")]
public class TrainingTypesController : ControllerBase
{
    private readonly ILogger<TrainingTypesController> _logger;
    private readonly ITrainingTypesService _trainingTypesService;
    private readonly IAuditService _auditService;
    private readonly RefreshHub _refreshHub;

    public TrainingTypesController(
        ILogger<TrainingTypesController> logger,
        ITrainingTypesService trainingTypesService,
        IAuditService auditService, 
        RefreshHub refreshHub)
    {
        _logger = logger;
        _trainingTypesService = trainingTypesService;
        _auditService = auditService;
        _refreshHub = refreshHub;
    }

    [HttpPost]
    [Authorize(Roles = AccessesNames.AUTH_configure_training_types)]
    [Route("")]
    public async Task<ActionResult<PutTrainingTypeResponse>> PostNewTrainingType([FromBody] PlannerTrainingType plannerTrainingType, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _trainingTypesService.PostTrainingType(userId, customerId, plannerTrainingType, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetTrainingTypes");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{callHub:bool}")]
    public async Task<ActionResult<MultiplePlannerTrainingTypesResponse>> GetTrainingTypes(bool callHub = false, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _trainingTypesService.GetTrainingTypes(customerId, clt);
            if (callHub)
            {
                _logger.LogInformation("Calling hub AllTrainingTypes");
                await _refreshHub.SendMessage(userId, ItemUpdated.AllTrainingTypes);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetTrainingTypes");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<GetTraininTypeByIdResponse>> GetById(Guid id, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            GetTraininTypeByIdResponse result = await _trainingTypesService.GetById(id, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetById");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Authorize(Roles = AccessesNames.AUTH_configure_training_types)]
    [Route("")]
    public async Task<ActionResult<PatchTrainingTypeResponse>> PatchTrainingType([FromBody] PlannerTrainingType plannerTrainingType, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            PatchTrainingTypeResponse result = await _trainingTypesService.PatchTrainingType(userId, customerId, plannerTrainingType, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PatchTrainingType");
            return BadRequest();
        }
    }
}
