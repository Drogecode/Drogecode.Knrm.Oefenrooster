using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages;
public sealed partial class Index
{
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] HttpClient Http { get; set; } = default!;
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
            var dbUser = await Http.GetFromJsonAsync<DrogeUser>("User");
            _name = authState!.User!.Identity!.Name ?? string.Empty;
            
        }
    }
}
