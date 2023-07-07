using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "Report")]
public class ReportController : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger;
    private readonly IReportService _reportService;
    private readonly IAuditService _auditService;

    public ReportController(
        ILogger<ScheduleController> logger,
        IReportService reportService,
        IAuditService auditService)
    {
        _logger = logger;
        _reportService = reportService;
        _auditService = auditService;
    }
}
