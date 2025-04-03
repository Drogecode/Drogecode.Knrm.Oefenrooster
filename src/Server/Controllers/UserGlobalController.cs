using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Server.Controllers.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserGlobal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "UserGlobal")]
public class UserGlobalController : DrogeController
{
    private readonly IUserGlobalService _userGlobalService;

    public UserGlobalController(ILogger<CustomerSettingController> logger, IUserGlobalService userGlobalService) : base(logger)
    {
        _userGlobalService = userGlobalService;
    }

    [HttpGet]
    [Route("all")]
    public async Task<ActionResult<AllDrogeUserGlobalResponse>> GetAll(CancellationToken clt = default)
    {
        try
        {
            var result = await _userGlobalService.GetAllUserGlobals(clt);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in GetAll");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<GetGlobalUserByIdResponse>> GetGlobalUserById(Guid id, CancellationToken clt = default)
    {
        try
        {
            var result = await _userGlobalService.GetGlobalUserById(id, clt);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in GetGlobalUserById");
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("")]
    public async Task<ActionResult<PutResponse>> PutGlobalUser([FromBody] DrogeUserGlobal globalUser, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _userGlobalService.PutGlobalUser(userId, globalUser, clt);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in PutGlobalUser");
            return BadRequest();
        }
    }
    
    [HttpPatch]
    [Route("")]
    public async Task<ActionResult<PatchResponse>> PatchGlobalUser([FromBody] DrogeUserGlobal globalUser, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _userGlobalService.PatchGlobalUser(userId, globalUser, clt);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in PatchGlobalUser");
            return BadRequest();
        }
    }
}