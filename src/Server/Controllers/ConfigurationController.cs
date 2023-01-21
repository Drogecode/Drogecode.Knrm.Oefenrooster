using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[Authorize]
[ApiController]
[Route("[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class ConfigurationController : ControllerBase
{
    private readonly ILogger<ConfigurationController> _logger;
    private readonly IConfigurationService _configurationService;

    public ConfigurationController(ILogger<ConfigurationController> logger, IConfigurationService configurationService)
    {
        _logger = logger;
        _configurationService = configurationService;
    }

    [HttpPatch]
    public void UpgradeDatabase()
    {
        _configurationService.UpgradeDatabase();
    }
}
