using Drogecode.Knrm.Oefenrooster.Shared.Authorization;

namespace Drogecode.Knrm.Oefenrooster.Client.Helpers;

public static class UserHelper
{
    public static async Task<bool> InRole(Task<AuthenticationState>? authenticationState, string role)
    {
        if (authenticationState is null) return false;
        var authState = await authenticationState;
        return InRole(authState, role);
    }

    public static bool InRole(AuthenticationState? authState, string role)
    {
        var user = authState?.User;
        return user is not null && user.IsInRole(role);
    }
}