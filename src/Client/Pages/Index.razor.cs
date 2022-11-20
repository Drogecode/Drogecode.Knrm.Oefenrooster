
namespace Drogecode.Knrm.Oefenrooster.Client.Pages;
public sealed partial class Index
{
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    private bool _isAuthenticated;
    private string _name = string.Empty;
    protected override async Task OnParametersSetAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        _isAuthenticated = authState.User?.Identity?.IsAuthenticated ?? false;
        if (_isAuthenticated)
            _name = authState!.User!.Identity!.Name ?? string.Empty;
    }
}
