using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class ConfigurationController : ControllerBase
{
    private readonly ILogger<ConfigurationController> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly IAuditService _auditService;

    public ConfigurationController(
        ILogger<ConfigurationController> logger,
        IConfigurationService configurationService,
        IAuditService auditService)
    {
        _logger = logger;
        _configurationService = configurationService;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task UpgradeDatabase()
    {
        var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
        var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
        await _configurationService.UpgradeDatabase();
        await _auditService.Log(userId, Shared.Enums.AuditType.DataBaseUpgrade, customerId);
    }

    [HttpGet]
    public bool NewVersionAvailable(string clientVersion)
    {
        return string.Compare(DefaultSettingsHelper.CURRENT_VERSION, clientVersion, StringComparison.OrdinalIgnoreCase) != 0;
    }
}
