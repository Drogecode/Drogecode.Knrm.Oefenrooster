using Drogecode.Knrm.Oefenrooster.Server.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Drogecode.Knrm.Oefenrooster.Server.Policies;

internal static class ConfigurePolicys
{
    public static void Configure(AuthorizationOptions options)
    {
        options.AddPolicy("something", policy =>
            policy.Requirements.Add(new LicenseRequirement("something")));
        options.AddPolicy("yustademo", policy =>
            policy.Requirements.Add(new LicenseRequirement("yustademo")));
    }
}
