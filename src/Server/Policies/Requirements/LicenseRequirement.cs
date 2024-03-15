using Microsoft.AspNetCore.Authorization;

//https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-8.0


namespace Drogecode.Knrm.Oefenrooster.Server.Policies.Requirements;

public class LicenseRequirement : IAuthorizationRequirement
{
    public string Requires { get; }
    public LicenseRequirement(string requires)
    {
        Requires = requires;
    }
}
