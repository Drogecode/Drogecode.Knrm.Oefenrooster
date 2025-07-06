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

    public Task<AuthenticationState> SwitchUser(SwitchUserRequest body)
    {
        _authenticationClient.SwitchUserAsync(body);
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
            var authState = GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(authState);
        }
    }

    public async Task Logout()
    {
        await _authenticationClient.LogoutAsync();
        _currentUser = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
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
        var parsedFrom = DateTime.TryParse(identity.Claims.FirstOrDefault(x => x.Type.Equals("ValidFrom"))?.Value, out var validFrom);
        var parsedTo = DateTime.TryParse(identity.Claims.FirstOrDefault(x => x.Type.Equals("ValidTo"))?.Value, out var validTo);
        if (!parsedFrom || !parsedTo)
        {
            identity = await RefreshInternal();
            return identity;
        }

        var now = DateTime.Now;
#if DEBUG
        //now = now.AddMinutes(59).AddSeconds(30);
#endif
        if (now.CompareTo(validFrom.AddMinutes(-5)) < 0 || now.CompareTo(validTo) > 0)
        {
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