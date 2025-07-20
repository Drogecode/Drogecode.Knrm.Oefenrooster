using System.Diagnostics;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Server.Controllers.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.License;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "License")]
public class LicenseController : DrogeController
{
    private readonly ILicenseService _licenseService;

    public LicenseController(ILogger<LicenseController> logger,
        ILicenseService licenseService) : base(logger)
    {
        _licenseService = licenseService;
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult<GetAllLicensesResponse>> GetLicenseForCurrentCustomer(CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var licenses = await _licenseService.GetAllLicensesForCustomer(customerId, clt);
            return licenses;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in GetLicenseForCurrentCustomer");
            return BadRequest();
        }
    }
}