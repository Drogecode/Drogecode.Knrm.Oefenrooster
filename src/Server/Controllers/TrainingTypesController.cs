using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
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

    [HttpGet]
    public async Task<ActionResult<MultiplePlannerTrainingTypesResponse>> GetTrainingTypes(CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var result = await _trainingTypesService.GetTrainingTypes(customerId, token);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetTrainingTypes");
            return BadRequest();
        }
    }
}
