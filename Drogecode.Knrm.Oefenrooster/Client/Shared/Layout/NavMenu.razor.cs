using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Xml.Linq;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;
public sealed partial class NavMenu
{
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] SignOutSessionStateManager SignOutManager { get; set; }
    [Inject] NavigationManager Navigation { get; set; }

    private AuthenticationState? _authState;
    protected override async Task OnParametersSetAsync()
    {
        _authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
    }

    private async Task BeginLogout(MouseEventArgs args)
    {
        await SignOutManager.SetSignOutState();
        Navigation.NavigateTo("authentication/logout");
    }
}
