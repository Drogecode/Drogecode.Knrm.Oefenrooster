using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CatchallController : ControllerBase
{
    private readonly ILogger<CatchallController> _logger;
    private readonly IAuditService _auditService;
    public CatchallController(
        ILogger<CatchallController> logger,
        IAuditService auditService)
    {
        _logger = logger;
        _auditService = auditService;
    }

    [Route("{**catchAll}")]
    [HttpPost("post", Order = int.MaxValue)]
    public IActionResult Post([FromBody] object value, string? catchAll)
    {
        try
        {
            _auditService.Log(Guid.Empty, AuditType.CatchAll, Guid.Empty, $"{JsonSerializer.Serialize(value)} : {catchAll}", objectName: "POST catch all");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CatchallController CatchAll Post");
            return BadRequest();
        }
    }
}
