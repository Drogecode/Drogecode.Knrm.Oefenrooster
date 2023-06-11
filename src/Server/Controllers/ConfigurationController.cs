using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Nager.Holiday;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "Configuration")]
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
    public async Task<ActionResult<UpgradeDatabaseResponse>> UpgradeDatabase(CancellationToken token = default)
    {
        try
        {
            UpgradeDatabaseResponse result = new UpgradeDatabaseResponse { Success = false };
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            var installing = _configuration.GetValue<bool>("Drogecode:Installing");
            if (installing || userId == DefaultSettingsHelper.IdTaco)
            {
                result.Success = await _configurationService.UpgradeDatabase();
                await _auditService.Log(userId, AuditType.DataBaseUpgrade, customerId);
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in UpgradeDatabase");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<VersionDetailResponse>> NewVersionAvailable(string clientVersion, CancellationToken token = default)
    {
        try
        {
            var response = new VersionDetailResponse
            {
                NewVersionAvailable = string.Compare(DefaultSettingsHelper.CURRENT_VERSION, clientVersion, StringComparison.OrdinalIgnoreCase) != 0
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in NewVersionAvailable");
            return BadRequest();
        }
    }

    [HttpGet]
    public ActionResult<InstallingActiveResponse> InstallingActive(CancellationToken token = default)
    {
        try
        {
            return Ok(new InstallingActiveResponse { Success = _configuration.GetValue<bool>("Drogecode:Installing") });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in InstallingActive");
            return new InstallingActiveResponse { Success = false };
        }
    }

    [HttpGet]
    public async Task<ActionResult<UpdateSpecialDatesResponse>> UpdateSpecialDates(CancellationToken token = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            using var holidayClient = new HolidayClient();
            var currentYear = DateTime.Now.Year;
            for (int i = currentYear; i < currentYear + 10; i++)
            {
                var holidays = await holidayClient.GetHolidaysAsync(i, "nl");
                if (holidays == null) continue;
                foreach (var holiday in holidays)
                {
                    await _configurationService.AddSpecialDay(customerId, holiday, token);
                }
            }
            return Ok(new UpdateSpecialDatesResponse { Success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in InstallingActive");
            return new UpdateSpecialDatesResponse { Success = false };
        }
    }
}
