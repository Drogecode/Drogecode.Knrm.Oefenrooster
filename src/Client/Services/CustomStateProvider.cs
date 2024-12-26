using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using System.Linq;
using System.Security.Claims;

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
        var identity = new ClaimsIdentity();
        try
        {
            identity = await GetCurrentUser();
            if (!identity.IsAuthenticated)
                return new AuthenticationState(new ClaimsPrincipal(identity));
            var parsedFrom = DateTime.TryParse(identity.Claims.FirstOrDefault(x => x.Type.Equals("ValidFrom"))?.Value, out var validFrom);
            var parsedTo = DateTime.TryParse(identity.Claims.FirstOrDefault(x => x.Type.Equals("ValidTo"))?.Value, out var validTo);
            if (!parsedFrom || !parsedTo)
            {
                identity = await Refresh();
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            var now = DateTime.Now;
#if DEBUG
            //now = now.AddMinutes(59).AddSeconds(30);
#endif
            if (now.CompareTo(validFrom.AddMinutes(-15)) < 0 || now.CompareTo(validTo) > 0)
            {
                identity = await Refresh();
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
        }
        catch (HttpRequestException ex)
        {
            identity = new ClaimsIdentity();
            Console.WriteLine("Request failed:" + ex.ToString());
        }
        return new AuthenticationState(new ClaimsPrincipal(identity));
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
        var claims = new[] { new Claim(ClaimTypes.Name, user.UserName) }.Concat(user.Claims.Select(c => new Claim(c.Key, c.Value)));
        _currentUser = new ClaimsIdentity(claims, "Server authentication");
        return _currentUser;
    }

    public async Task loginCallback()
    {
        _currentUser = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task Logout()
    {
        await _authenticationClient.LogoutAsync();
        _currentUser = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private async Task<ClaimsIdentity> Refresh()
    {
        if (await _authenticationClient.RefreshAsync())
        {
            _currentUser = null;
            return await GetCurrentUser();
        }
        return new ClaimsIdentity();
    }
}
