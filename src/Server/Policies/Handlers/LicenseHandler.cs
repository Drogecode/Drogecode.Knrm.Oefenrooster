using Drogecode.Knrm.Oefenrooster.Server.Policies.Requirements;
using Drogecode.Knrm.Oefenrooster.Shared.Models.License;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Policies.Handlers;

public class LicenseHandler : AuthorizationHandler<LicenseRequirement>
{
    private IMemoryCache _memoryCache { get; set; }
    private readonly ILicenseService _licenseService;

    public LicenseHandler(IMemoryCache memoryCache, ILicenseService licenseService)
    {
        _memoryCache = memoryCache;
        _licenseService = licenseService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, LicenseRequirement requirement)
    {
        var customerId = context.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.microsoft.com/identity/claims/tenantid");
        if (customerId is null)
        {
            return;
        }

        var cacheKey = $"licenses_{customerId}";
        var licenses = _memoryCache.Get<ICollection<DrogeLicense>>(cacheKey);
        if (licenses is null)
        {
            if (!Guid.TryParse(customerId.Value, out var customerIdGuid))
                return;

            var licensesForCustomer = await _licenseService.GetAllLicensesForCustomer(customerIdGuid, CancellationToken.None);
            if (licensesForCustomer.Licenses is null)
                return;

            licenses = licensesForCustomer.Licenses;
            if (licenses is not null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromHours(3));
                cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(10));
                _memoryCache.Set(cacheKey, licenses, cacheEntryOptions);
            }
        }

        if (licenses is not null && licenses.Any(x => x.License == requirement.Requires))
        {
            context.Succeed(requirement);
        }
    }
}