using Drogecode.Knrm.Oefenrooster.Server.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Web;

//https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-8.0

namespace Drogecode.Knrm.Oefenrooster.Server.Policies.Handlers;

public class LicenseHandler : AuthorizationHandler<LicenseRequirement>
{
    private IMemoryCache _memoryCache { get; set; }
    public LicenseHandler(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, LicenseRequirement requirement)
    {
        try
        {
            var tenantId = context.User.Claims.FirstOrDefault(x => x.Type == "tenantId");
            if (tenantId is null)
            {
                var displayname = context.User.GetDisplayName();
                return;
            }
            var cacheKey = $"licenses_{tenantId}";
            ICollection<string>? licenses = _memoryCache.Get<ICollection<string>>(cacheKey);
            if (licenses is null)
            {
                if (tenantId is null)
                    return;
                if (!Guid.TryParse(tenantId.Value, out Guid tenantGuid))
                    return;
                licenses = new List<string> {"something", "yustademo" };
                if (licenses is not null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions();
                    cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromHours(2));
                    cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                    _memoryCache.Set(cacheKey, licenses, cacheEntryOptions);
                }
            }
            if (licenses is not null && licenses.Contains(requirement.Requires))
            {
                context.Succeed(requirement);
            }
        }
        catch (Exception ex)
        {
        }

        return;
    }
}
