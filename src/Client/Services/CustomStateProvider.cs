using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;
using System.Security.Claims;
namespace Drogecode.Knrm.Oefenrooster.Client.Services;

public class CustomStateProvider : AuthenticationStateProvider
{
    private ClaimsIdentity? _currentUser;
    private IAuthenticationClient _authenticationClient;
    private DateTime _lastModified = DateTime.MinValue;
    
    public string? LoginHint { get; private set; }

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
        _currentUser = null;
        await _authenticationClient.LogoutAsync();
        await UpdateLogOutState();
    }
    private Task<AuthenticationState> UpdateLogOutState()
    {
        var authState = LogOutState();
        NotifyAuthenticationStateChanged(authState);
        return authState;
    }

    private async Task<AuthenticationState> LogOutState()
    {
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
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
        LoginHint = _currentUser.Claims.FirstOrDefault(x => x.Type.Equals("login_hint"))?.Value;
        return _currentUser;
    }

    private async Task<ClaimsIdentity> RefreshIfRequired(ClaimsIdentity identity)
    {
        if (!identity.IsAuthenticated)
        {
            return await RefreshInternal();
        }

        var validFromClaim = identity.Claims.FirstOrDefault(x => x.Type.Equals("ValidFrom"))?.Value;
        var validToClaim = identity.Claims.FirstOrDefault(x => x.Type.Equals("ValidTo"))?.Value;

        var parsedFrom = DateTime.TryParse(validFromClaim, out var validFrom);
        var parsedTo = DateTime.TryParse(validToClaim, out var validTo);

        // If parsing fails or the dates are invalid (e.g., DateTime.MinValue), refresh.
        if (!parsedFrom || !parsedTo || validFrom == default || validTo == default)
        {
            //Failed to parse ValidFrom/ValidTo.
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
                identity = await RefreshInternal();
            }
        }
        catch (ArgumentOutOfRangeException ex)
        {
            DebugHelper.WriteLine($"DateTime overflow: {ex.Message}");
            identity = await RefreshInternal();
        }

        return identity;
    }

    private async Task<ClaimsIdentity> RefreshInternal()
    {
        var refreshResponse = await _authenticationClient.RefreshUserAsync();
        if (!refreshResponse.Success || refreshResponse.State != RefreshState.AuthenticationRefreshed)
        {
            //Logged out after a failed refresh
            _currentUser = null;
            return new ClaimsIdentity();
        }
        var oldUser = _currentUser;

        _currentUser = null;
        var newUser = await GetCurrentUser();
        
        if (refreshResponse.ForceRefresh || oldUser is null || oldUser.Claims.Count() != newUser.Claims.Count())
        {
            // Should force refresh
            _lastModified = DateTime.UtcNow;
            return newUser;
        }

        if (newUser.Claims.Where(x=>x.Type.Equals(ClaimTypes.Role)).Any(newClaim => !oldUser.Claims.Any(x => x.Type.Equals(ClaimTypes.Role) && x.Value.Equals(newClaim.Value))))
        {
            // Should force refresh because a role has changed
            _lastModified = DateTime.UtcNow;
            return newUser;
        }
        
        // State changed but hold refresh
        return newUser;
    }
}