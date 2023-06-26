using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "TrainingTypes")]
public class TrainingTypesController : ControllerBase
{
    private readonly ILogger<TrainingTypesController> _logger;
    private readonly ITrainingTypesService _trainingTypesService;
    private readonly IAuditService _auditService;

    public TrainingTypesController(
        ILogger<TrainingTypesController> logger,
        ITrainingTypesService trainingTypesService,
        IAuditService auditService)
    {
        _logger = logger;
        _trainingTypesService = trainingTypesService;
        _auditService = auditService;
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<PutTrainingTypeResponse>> PostNewTrainingType([FromBody] PlannerTrainingType PlannerTrainingType, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _trainingTypesService.PostTrainingType(PlannerTrainingType, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetTrainingTypes");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult<MultiplePlannerTrainingTypesResponse>> GetTrainingTypes(CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _trainingTypesService.GetTrainingTypes(customerId, clt);
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
}
