using Drogecode.Knrm.Oefenrooster.Client.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Drogecode.Knrm.Oefenrooster.Client.Policies.Handlers;

public class LicenseHandler : AuthorizationHandler<LicenseRequirement>
{
    private readonly LicenseRepository _licenseRepository;

    public LicenseHandler(LicenseRepository licenseRepository)
    {
        _licenseRepository = licenseRepository;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, LicenseRequirement requirement)
    {
        var licenses = await _licenseRepository.GetAllAsync(false, false, CancellationToken.None);
        if (licenses?.Licenses?.Count is null or 0 || licenses.Licenses.All(x => x.License != requirement.Requires))
        {
            return;
        }

        context.Succeed(requirement);
    }
}