using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;

namespace Drogecode.Knrm.Oefenrooster.Client.Services;

public class CustomStateProvider : AuthenticationStateProvider
{
    private ClaimsIdentity? _currentUser;
    private IAuthenticationClient _authenticationClient;

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
                return new AuthenticationState(new ClaimsPrincipal(identity));
            var parsedFrom = DateTime.TryParse(identity.Claims.FirstOrDefault(x => x.Type.Equals("ValidFrom"))?.Value, out var validFrom);
            var parsedTo = DateTime.TryParse(identity.Claims.FirstOrDefault(x => x.Type.Equals("ValidTo"))?.Value, out var validTo);
            if (!parsedFrom || !parsedTo)
            {
                identity = await RefreshInternal();
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }

            var now = DateTime.Now;
#if DEBUG
            //now = now.AddMinutes(59).AddSeconds(30);
#endif
            if (now.CompareTo(validFrom.AddMinutes(-5)) < 0 || now.CompareTo(validTo) > 0)
            {
                identity = await RefreshInternal();
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
        }
        catch (HttpRequestException ex)
        {
            identity = new ClaimsIdentity();
            Console.WriteLine("Request failed:" + ex);
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task loginCallback()
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

    public Task<AuthenticationState> Refresh()
    {
        var authState = GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(authState);
        return authState;
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
        if (!user.IsAuthenticated)
        {
            _currentUser = new ClaimsIdentity();
            return new ClaimsIdentity();
        }

        var claims = new[] { new Claim(ClaimTypes.Name, user.UserName ?? "No name") }.Concat(user.Claims.Select(c => new Claim(c.Key, c.Value)));
        _currentUser = new ClaimsIdentity(claims, "Server authentication");
        return _currentUser;
    }

    private async Task<ClaimsIdentity> RefreshInternal()
    {
        if (await _authenticationClient.RefreshAsync())
        {
            _currentUser = null;
            return await GetCurrentUser();
        }

        return new ClaimsIdentity();
    }
}