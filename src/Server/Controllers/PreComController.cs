using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PreComController : ControllerBase
{
    private readonly ILogger<PreComController> _logger;
    private readonly IPreComService _preComService;
    private readonly IAuditService _auditService;
    public PreComController(
        ILogger<PreComController> logger,
        IPreComService preComService,
        IAuditService auditService)
    {
        _logger = logger;
        _preComService = preComService;
        _auditService = auditService;
    }

    [HttpPost]
    [Route("WebHook")]
    public async Task<IActionResult> WebHook([FromBody] object body)
    {
        try
        {
            _logger.LogInformation("Resived PreCom message");
            var data = body as NotificationData;
            _logger.LogInformation($"Message is '{data?._alert}'");
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            await _preComService.WriteAlertToDb(customerId, data?._notificationId, data?._alert ?? "No alert found by hui.nu webhook", JsonSerializer.Serialize(body));
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in PreCom WebHook");
            return BadRequest("Exception in PreCom WebHook");
        }
    }
}
