using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Client.Services;

public class CustomStateProvider : AuthenticationStateProvider
{
    private CurrentUser? _currentUser;
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
            var userInfo = await GetCurrentUser();
            if (!userInfo.IsAuthenticated)
                return new AuthenticationState(new ClaimsPrincipal(identity));
            var parsedFrom = DateTime.TryParse(userInfo.Claims.FirstOrDefault(x => x.Key.Equals("ValidFrom")).Value, out var validFrom);
            var parsedTo = DateTime.TryParse(userInfo.Claims.FirstOrDefault(x => x.Key.Equals("ValidTo")).Value, out var validTo);
            if (!parsedFrom || !parsedTo)
                return new AuthenticationState(new ClaimsPrincipal(identity));
            var now = DateTime.Now;
            if (now.CompareTo(validFrom.AddMinutes(-15)) < 0 || now.CompareTo(validTo) > 0)
                return new AuthenticationState(new ClaimsPrincipal(identity));
            var claims = new[] { new Claim(ClaimTypes.Name, _currentUser.UserName) }.Concat(_currentUser.Claims.Select(c => new Claim(c.Key, c.Value)));
            identity = new ClaimsIdentity(claims, "Server authentication");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Request failed:" + ex.ToString());
        }
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
    private async Task<CurrentUser> GetCurrentUser()
    {
        if (_currentUser != null && _currentUser.IsAuthenticated) return _currentUser;
        _currentUser = await _authenticationClient.CurrentUserInfoAsync();
        return _currentUser;
    }

    public async Task loginCallback()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task Logout()
    {
        await _authenticationClient.LogoutAsync();
        _currentUser = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
