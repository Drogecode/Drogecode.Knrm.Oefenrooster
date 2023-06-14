using Microsoft.AspNetCore.Mvc;

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
    public IActionResult Post([FromBody] string value, string catchAll)
    {
        try
        {
            _auditService.Log(Guid.Empty, AuditType.CatchAll, Guid.Empty, $"{value} : {catchAll}", objectName: "POST catch all");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CatchallController CatchAll Post");
            return BadRequest();
        }
    }
}
