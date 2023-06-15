using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[ApiController]
[Route("api/[controller]/[action]")]
[ApiExplorerSettings(GroupName = "PreCom")]
public class PreComController : ControllerBase
{
    private readonly ILogger<PreComController> _logger;
    private readonly IPreComService _preComService;
    private readonly IAuditService _auditService;
    private readonly PreComHub _preComHub;
    public PreComController(
        ILogger<PreComController> logger,
        IPreComService preComService,
        IAuditService auditService,
        PreComHub preComHub)
    {
        _logger = logger;
        _preComService = preComService;
        _auditService = auditService;
        _preComHub = preComHub;
    }

    [HttpPost]
    public async Task<IActionResult> WebHook([FromBody] object body)
    {
        try
        {
            _logger.LogInformation("Resived PreCom message");
            var data = body as NotificationData;
            _logger.LogInformation($"Message is '{data?.Alert}'");
            var customerId = DefaultSettingsHelper.KnrmHuizenId;
            var alert = data?.Alert ?? "No alert found by hui.nu webhook";
            await _preComService.WriteAlertToDb(customerId, data?.NotificationId, data?.Data?.ActionData?.Timestamp, alert, JsonSerializer.Serialize(body));
            await _preComHub.SendMessage("PreCom", alert);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in PreCom WebHook");
            return BadRequest("Exception in PreCom WebHook");
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<MultiplePreComAlertsResponse>> AllAlerts(CancellationToken clt)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            if (userId != DefaultSettingsHelper.IdTaco) return Unauthorized("User not me");
            MultiplePreComAlertsResponse result = await _preComService.GetAllAlerts(customerId);
            await _preComHub.SendMessage("PreCom", "bliep blop hello");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in AllAlerts");
            return BadRequest();
        }
    }
}
