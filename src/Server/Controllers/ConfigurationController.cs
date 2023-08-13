using Azure;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Nager.Holiday;
using System.Diagnostics;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
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

    [HttpPatch]
    [Route("upgrade-database")]
    [Authorize(Roles = AccessesNames.AUTH_Taco)]
    public async Task<ActionResult<UpgradeDatabaseResponse>> UpgradeDatabase(CancellationToken token = default)
    {
        try
        {
            var sw = Stopwatch.StartNew();
            UpgradeDatabaseResponse result = new UpgradeDatabaseResponse { Success = false };
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("customerId not found"));
            result.Success = await _configurationService.UpgradeDatabase();
            await _auditService.Log(userId, AuditType.DataBaseUpgrade, customerId);
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return Ok(result);
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in UpgradeDatabase");
            return BadRequest();
        }
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("new-version-available/{clientVersion}")]
    public async Task<ActionResult<VersionDetailResponse>> NewVersionAvailable(string clientVersion, CancellationToken token = default)
    {
        try
        {
            var sw = Stopwatch.StartNew();
            var response = new VersionDetailResponse
            {
                NewVersionAvailable = string.Compare(DefaultSettingsHelper.CURRENT_VERSION, clientVersion, StringComparison.OrdinalIgnoreCase) != 0
            };
            sw.Stop();
            response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in NewVersionAvailable");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("installing-active")]
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

    [HttpPatch]
    [Route("update-special-dates")]
    [Authorize(Roles = AccessesNames.AUTH_Taco)]
    public async Task<ActionResult<UpdateSpecialDatesResponse>> UpdateSpecialDates(CancellationToken token = default)
    {
        try
        {
            var sw = Stopwatch.StartNew();
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
            sw.Stop();
            return Ok(new UpdateSpecialDatesResponse { Success = true, ElapsedMilliseconds = sw.ElapsedMilliseconds });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in InstallingActive");
            return new UpdateSpecialDatesResponse { Success = false };
        }
    }
}
