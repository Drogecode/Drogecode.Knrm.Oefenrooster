using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared;
public sealed partial class NavMenu
{
    [Inject] SignOutSessionStateManager SignOutManager { get; set; }
    [Inject] NavigationManager Navigation { get; set; }

    private async Task BeginLogout(MouseEventArgs args)
    {
        await SignOutManager.SetSignOutState();
        Navigation.NavigateTo("authentication/logout");
    }
}
