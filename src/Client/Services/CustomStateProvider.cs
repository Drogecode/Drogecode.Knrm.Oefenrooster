using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Client.Services;

public class CustomStateProvider : AuthenticationStateProvider
{
    private ClaimsIdentity? _currentUser;
    private IAuthenticationClient _authenticationClient;
    private DateTime _lastModified = DateTime.MinValue;

    public CustomStateProvider(IAuthenticationClient authenticationClient)
    {
        _authenticationClient = authenticationClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsIdentity identity;
        try
        {
            identity = await GetCurrentUser();
            if (!identity.IsAuthenticated)
            {
                _currentUser = null;
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }

            return new AuthenticationState(new ClaimsPrincipal(await RefreshIfRequired(identity)));
        }
        catch (HttpRequestException ex)
        {
            identity = new ClaimsIdentity();
            Console.WriteLine("Request failed:" + ex);
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task LoginCallback()
    {
        _currentUser = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task SwitchUser(SwitchUserRequest body)
    {
        await _authenticationClient.SwitchUserAsync(body);
        await UpdateAuthState();
    }

    private Task<AuthenticationState> UpdateAuthState()
    {
        var authState = GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(authState);
        return authState;
    }

    public async Task Refresh()
    {
        var oldLastModified = _lastModified;

        var identity = await GetCurrentUser();
        await RefreshIfRequired(identity);
        if (oldLastModified != _lastModified)
        {
            await UpdateAuthState();
        }
    }

    public async Task Logout()
    {
        await _authenticationClient.LogoutAsync();
        _currentUser = null;
        await UpdateAuthState();
    }

    private async Task<ClaimsIdentity> GetCurrentUser()
    {
        if (_currentUser is { IsAuthenticated: true }) return _currentUser;
        var user = await _authenticationClient.CurrentUserInfoAsync();
        _lastModified = DateTime.UtcNow;
        if (!user.IsAuthenticated)
        {
            _currentUser = new ClaimsIdentity();
            return new ClaimsIdentity();
        }

        var claims = new[] { new Claim(ClaimTypes.Name, user.UserName ?? "No name") }.Concat(user.Claims.Select(c => new Claim(c.Key, c.Value)));
        _currentUser = new ClaimsIdentity(claims, "Server authentication");
        return _currentUser;
    }

    private async Task<ClaimsIdentity> RefreshIfRequired(ClaimsIdentity identity)
    {
        if (!identity.IsAuthenticated)
        {
            DebugHelper.WriteLine("❌ Identity not authenticated.");
            return await RefreshInternal();
        }

        var validFromClaim = identity.Claims.FirstOrDefault(x => x.Type.Equals("ValidFrom"))?.Value;
        var validToClaim = identity.Claims.FirstOrDefault(x => x.Type.Equals("ValidTo"))?.Value;

        var parsedFrom = DateTime.TryParse(validFromClaim, out var validFrom);
        var parsedTo = DateTime.TryParse(validToClaim, out var validTo);

        // If parsing fails or the dates are invalid (e.g., DateTime.MinValue), refresh.
        if (!parsedFrom || !parsedTo || validFrom == default || validTo == default)
        {
            DebugHelper.WriteLine($"⚠️ Failed to parse ValidFrom/ValidTo. From='{validFromClaim}', To='{validToClaim}'");
            identity = await RefreshInternal();
            return identity;
        }

        var now = DateTime.Now;
#if DEBUG
        //now = now.AddMinutes(59).AddSeconds(30);
#endif

        // Check for underflow/overflow when adding minutes
        try
        {
            if (now < validFrom.AddMinutes(-5) || now > validTo)
            {
                DebugHelper.WriteLine($"⏰ Token expired or not yet valid. Now={now}, From={validFrom}, To={validTo}");
                identity = await RefreshInternal();
            }
        }
        catch (ArgumentOutOfRangeException ex)
        {
            DebugHelper.WriteLine($"❌ DateTime overflow: {ex.Message}");
            identity = await RefreshInternal();
        }

        return identity;
    }

    private async Task<ClaimsIdentity> RefreshInternal()
    {
        var refreshResponse = await _authenticationClient.RefreshUserAsync();
        if (!refreshResponse.Success || refreshResponse.State != RefreshState.AuthenticationRefreshed)
        {
            DebugHelper.WriteLine("Logged out after failed refresh");
            _currentUser = null;
            return new ClaimsIdentity();
        }

        if (refreshResponse.ForceRefresh) // ToDo: add logic to compare claims
        {
            DebugHelper.WriteLine("Should force refresh");
            _lastModified = DateTime.UtcNow;
        }
        else
        {
            DebugHelper.WriteLine("State changed but hold refresh");
        }

        _currentUser = null;
        return await GetCurrentUser();
    }
}