using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
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
    private readonly IConfiguration _configuration;
    private readonly IConfigurationService _configurationService;
    private readonly IAuditService _auditService;

    public ConfigurationController(
        ILogger<ConfigurationController> logger,
        IConfiguration configuration,
        IConfigurationService configurationService,
        IAuditService auditService)
    {
        _logger = logger;
        _configuration = configuration;
        _configurationService = configurationService;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<ActionResult> UpgradeDatabase()
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var installing = _configuration.GetValue<bool>("Drogecode:Installing");
            if (installing || userId == DefaultSettingsHelper.IdTaco)
            {
                await _configurationService.UpgradeDatabase();
                await _auditService.Log(userId, AuditType.DataBaseUpgrade, customerId);
            }
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpgradeDatabase");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<UpdateDetails>> NewVersionAvailable(string clientVersion)
    {
        try
        {
            var response = new UpdateDetails
            {
                NewVersionAvailable = string.Compare(DefaultSettingsHelper.CURRENT_VERSION, clientVersion, StringComparison.OrdinalIgnoreCase) != 0
            };
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in NewVersionAvailable");
            return BadRequest();
        }
    }

    [HttpGet]
    public ActionResult<bool> InstallingActive()
    {
        try
        {
            return _configuration.GetValue<bool>("Drogecode:Installing");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in InstallingActive");
            return false;
        }
    }
}
