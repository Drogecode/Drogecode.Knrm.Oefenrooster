using System.Diagnostics;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "UserLinkedMails")]
public class UserLinkedMailsController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserLinkedMailsService _userLinkedMailsService;
    private readonly IAuditService _auditService;

    public UserLinkedMailsController(ILogger<UserController> logger, IUserLinkedMailsService userLinkedMailsService, IAuditService auditService)
    {
        _logger = logger;
        _userLinkedMailsService = userLinkedMailsService;
        _auditService = auditService;
    }

    [HttpPut]
    [Route("")]
    public async Task<ActionResult<PutUserLinkedMailResponse>> PutUserLinkedMail([FromBody] UserLinkedMail userLinkedMail, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            PutUserLinkedMailResponse result = await _userLinkedMailsService.PutUserLinkedMail(userLinkedMail, customerId, userId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PutUserLinkedMail");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("")]
    public async Task<ActionResult<PatchUserLinkedMailResponse>> PatchUserLinkedMail([FromBody] UserLinkedMail userLinkedMail, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            PatchUserLinkedMailResponse result = await _userLinkedMailsService.PatchUserLinkedMail(userLinkedMail, customerId, userId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PatchUserLinkedMail");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{take:int}/{skip:int}")]
    public async Task<ActionResult<AllUserLinkedMailResponse>> AllUserLinkedMail(int take, int skip, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            AllUserLinkedMailResponse result = await _userLinkedMailsService.AllUserLinkedMail(take, skip, userId, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in AllUserLinkedMail");
            return BadRequest();
        }
    }
}