using Drogecode.Knrm.Oefenrooster.Server.Helpers.JsonConverters;
using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
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
    [AllowAnonymous]
    [Route("web-hook/{id}")]
    public async Task<IActionResult> WebHook(Guid id, [FromBody] object body, bool sendToHub = true)
    {
        try
        {
            _logger.LogInformation("Resived PreCom message");
            var customerId = DefaultSettingsHelper.KnrmHuizenId;
            try
            {
                var jsonSerializerOptions = new JsonSerializerOptions()
                {
                    Converters = { new PreComConverter() }
                };
                var ser = JsonSerializer.Serialize(body);
                var data = JsonSerializer.Deserialize<NotificationDataBase>(ser, jsonSerializerOptions);
                _logger.LogInformation($"Message is '{data?._alert}'");
                var alert = data?._alert ?? "No alert found by hui.nu webhook";
                var timestamp = DateTime.SpecifyKind(data?._data?.actionData?.Timestamp ?? DateTime.MinValue, DateTimeKind.Utc);
                var prioParsed = int.TryParse(data?._data?.priority, out int priority);
                await _preComService.WriteAlertToDb(id, customerId, data?._notificationId, timestamp, alert, prioParsed ? priority : null, JsonSerializer.Serialize(body));
                if (sendToHub)
                    await _preComHub.SendMessage(id, "PreCom", alert);
            }
            catch (Exception ex)
            {
                await _preComService.WriteAlertToDb(id, customerId, Guid.Empty, DateTime.UtcNow, ex.Message, -1, body is null ? "body is null" : JsonSerializer.Serialize(body));
                if (sendToHub)
                    await _preComHub.SendMessage(id, "PreCom", "piep piep");
            }
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in PreCom WebHook");
            return BadRequest("Exception in PreCom WebHook");
        }
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult<MultiplePreComAlertsResponse>> AllAlerts(CancellationToken clt)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            if (userId != DefaultSettingsHelper.IdTaco) return Unauthorized("User not me");
            MultiplePreComAlertsResponse result = await _preComService.GetAllAlerts(customerId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in AllAlerts");
            return BadRequest();
        }
    }

    /*[Route("{**catchAll}")]
    [AllowAnonymous]
    [HttpPost("post", Order = int.MaxValue)]
    public IActionResult Post([FromBody] object value, string? catchAll)
    {
        try
        {
            _auditService.Log(DefaultSettingsHelper.IdTaco, AuditType.CatchAll, DefaultSettingsHelper.KnrmHuizenId, $"{JsonSerializer.Serialize(value)} : {catchAll}", objectName: "POST catch all");
            return Ok($"Got it {JsonSerializer.Serialize(value)} : {catchAll}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CatchallController CatchAll POST");
            return BadRequest("Exception in CatchallController CatchAll POST");
        }
    }*/
}
