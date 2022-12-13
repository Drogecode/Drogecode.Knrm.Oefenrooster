
using Drogecode.Knrm.Oefenrooster.Shared;
using Drogecode.Knrm.Oefenrooster.Shared.Models;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages;
public sealed partial class Index
{
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] HttpClient Http { get; set; } = default!;
    private bool _isAuthenticated;
    private string _name = string.Empty;
    protected override async Task OnParametersSetAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        _isAuthenticated = authState.User?.Identity?.IsAuthenticated ?? false;
        if (_isAuthenticated)
        {
            var dbUser = await Http.GetFromJsonAsync<User>("User");
            _name = authState!.User!.Identity!.Name ?? string.Empty;
        }
    }
}
