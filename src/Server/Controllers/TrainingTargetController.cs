using System.Diagnostics;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Server.Controllers.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
[Authorize(Roles = $"{AccessesNames.AUTH_target_read},{AccessesNames.AUTH_scheduler_target_set},{AccessesNames.AUTH_target_edit},{AccessesNames.AUTH_target_user_read}")]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "TrainingTarget")]
public class TrainingTargetController : DrogeController
{
    private readonly ITrainingTargetService _trainingTargetService;

    public TrainingTargetController(ILogger<TrainingTargetController> logger, ITrainingTargetService trainingTargetService) : base(logger)
    {
        _trainingTargetService = trainingTargetService;
    }

    [HttpGet]
    [Authorize(Roles = AccessesNames.AUTH_scheduler_target_set)]
    [Route("all/{count:int}/{skip:int}")]
    [Route("all/{subjectId:guid}/{count:int}/{skip:int}")]
    public async Task<ActionResult<AllTrainingTargetSubjectsResponse>> AllTrainingTargets(int count, int skip, Guid? subjectId = null, CancellationToken clt = default)
    {
        try
        {
            if (count > 50)
            {
                Logger.LogWarning("AllTrainingTargets count to big {0}", count);
                return BadRequest("Count to big");
            }

            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var response = await _trainingTargetService.AllTrainingTargets(count, skip, subjectId, customerId, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in AllTrainingTargets");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{trainingId:guid}")]
    public async Task<ActionResult<AllTrainingTargetsResponse>> GetTargetsLinkedToTraining(Guid trainingId, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var response = await _trainingTargetService.GetTargetsLinkedToTraining(trainingId, customerId, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in GetTargetsLinkedToTraining");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("set/{trainingId:guid}")]
    public async Task<ActionResult<GetSingleTargetSetResponse>> GetSetLinkedToTraining(Guid trainingId, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var response = await _trainingTargetService.GetSetLinkedToTraining(trainingId, customerId, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in GetSetLinkedToTraining");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("set/all/reusable/{count:int}/{skip:int}")]
    public async Task<ActionResult<GetAllTargetSetResponse>> GetAllReusableSets(int count, int skip, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var response = await _trainingTargetService.GetAllReusableSets(customerId, count, skip, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in GetAllReusableSets");
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("set")]
    public async Task<ActionResult<PutResponse>> PutNewTemplateSet([FromBody] TrainingTargetSet body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var response = await _trainingTargetService.PutNewTemplateSet(body, userId, customerId, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in PutNewTemplateSet");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("set")]
    public async Task<ActionResult<PatchResponse>> PatchTemplateSet([FromBody] TrainingTargetSet body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var response = await _trainingTargetService.PatchTemplateSet(body, userId, customerId, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in PatchNewTemplateSet");
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("result")]
    public async Task<ActionResult<PutResponse>> PutUserResponse(TrainingTargetResult body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var response = await _trainingTargetService.PutUserResponse(body, userId, customerId, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in PutUserResponse");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("result")]
    public async Task<ActionResult<PatchResponse>> PatchUserResponse([FromBody] TrainingTargetResult body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var response = await _trainingTargetService.PatchUserResponse(body, userId, customerId, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in PatchUserResponse");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("result/training/{trainingId:guid}/{forType}/{result:int}")]
    public async Task<ActionResult<PatchResponse>> PatchUserResponseForTraining(Guid trainingId, TrainingTargetType forType, int result, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var response = await _trainingTargetService.PatchUserResponseForTraining(trainingId, forType, result, userId, customerId, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in PatchUserResponseForTraining");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("results/all/user/{userIdResult:guid}/{period}")]
    public async Task<ActionResult<AllResultForUserResponse>> GetAllResultForUser(Guid userIdResult, RatingPeriod period, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var response = await _trainingTargetService.GetAllResultForUser(userIdResult, userId, period, customerId, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in GetAllResultForUser");
            return BadRequest();
        }
    }
}