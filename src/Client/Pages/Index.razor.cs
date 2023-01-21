using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages;
public sealed partial class Index
{
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] UserRepository UserRepository { get; set; } = default!;
    private bool _isAuthenticated;
    private string _name = string.Empty;
    private ClaimsPrincipal _user;
    protected override async Task OnParametersSetAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        _user = authState.User;
        _isAuthenticated = authState.User?.Identity?.IsAuthenticated ?? false;
        if (_isAuthenticated)
        {
            var dbUser = await UserRepository.GetCurrentUserAsync();
            _name = authState!.User!.Identity!.Name ?? string.Empty;
            
        }
    }
}
