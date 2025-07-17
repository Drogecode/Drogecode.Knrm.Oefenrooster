using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_mail_invite_external)]
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "UserLinkedMails")]
public class UserLinkMailsController : ControllerBase
{
    private readonly ILogger<UserLinkMailsController> _logger;
    private readonly IUserLinkedMailsService _userLinkedMailsService;
    private readonly IAuditService _auditService;
    private readonly IGraphService _graphService;

    public UserLinkMailsController(ILogger<UserLinkMailsController> logger, IUserLinkedMailsService userLinkedMailsService, IAuditService auditService, IGraphService graphService)
    {
        _logger = logger;
        _userLinkedMailsService = userLinkedMailsService;
        _auditService = auditService;
        _graphService = graphService;
    }

    [HttpPut]
    [Route("")]
    public async Task<ActionResult<PutUserLinkedMailResponse>> PutUserLinkedMail([FromBody] PutUserLinkedMailRequest body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            if (body.UserLinkedMail?.Email is null)
                return BadRequest();
            body.UserLinkedMail.Email = body.UserLinkedMail.Email.ToLower();
            var result = await _userLinkedMailsService.PutUserLinkedMail(body.UserLinkedMail, customerId, userId, clt);

            if (result.Success && body.SendMail)
            {
                clt = CancellationToken.None;
                _graphService.InitializeGraph();
                var mailBody = $"""
                                Om kalender updates te ontvangen voor je oefeningen moet je onderstaande code invullen op hui.nu

                                {result.ActivateKey}
                                """;
                await _graphService.SendMail(userId, body.UserLinkedMail!.Email!, "Bevestig linking met hui.nu", mailBody, clt);
            }

            result.ActivateKey = null;
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
    [Route("validate")]
    public async Task<ActionResult<ValidateUserLinkedActivateKeyResponse>> ValidateUserLinkedActivateKey([FromBody] ValidateUserLinkedActivateKeyRequest body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            ValidateUserLinkedActivateKeyResponse result = await _userLinkedMailsService.ValidateUserLinkedActivateKey(body, customerId, userId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in ValidateUserLinkedActivateKey");
            return BadRequest();
        }
    }
    
    [HttpPatch]
    [Route("is-enabled")]
    public async Task<ActionResult<IsEnabledChangedResponse>> IsEnabledChanged([FromBody] IsEnabledChangedRequest body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _userLinkedMailsService.IsEnabledChanged(body, customerId, userId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in IsEnabledChanged");
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
            var result = await _userLinkedMailsService.PatchUserLinkedMail(userLinkedMail, customerId, userId, clt);
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
            var result = await _userLinkedMailsService.AllUserLinkedMail(take, skip, userId, customerId, false, clt);
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

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<ActionResult<DeleteResponse>> DeleteUserLinkMail(Guid id, CancellationToken clt = default)
    {

        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var response = await _userLinkedMailsService.DeleteUserLinkMail(userId, customerId, id, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in DeleteUserLinkMail");
            return BadRequest();
        }
    }
}