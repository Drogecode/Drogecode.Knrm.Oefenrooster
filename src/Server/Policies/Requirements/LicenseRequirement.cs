using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Drogecode.Knrm.Oefenrooster.Server.Policies.Requirements;

// https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-9.0
public class LicenseRequirement : IAuthorizationRequirement
{ 
    public Licenses Requires { get; }
    public LicenseRequirement(Licenses requires)
    {
        Requires = requires;
    }
    
}