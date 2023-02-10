using Drogecode.Knrm.Oefenrooster.Client.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;
public sealed partial class MainLayout :IDisposable
{
    [Inject] SignOutSessionStateManager SignOutManager { get; set; }
    [Inject] NavigationManager Navigation { get; set; }
    private DrogeCodeGlobal _global { get; set; } = new();

    protected override void OnParametersSet()
    {
        _global.RefreshRequested += RefreshMe;
    }

    private async Task BeginLogout(MouseEventArgs args)
    {
        await SignOutManager.SetSignOutState();
        Navigation.NavigateTo("authentication/logout");
    }
    private void RefreshMe()
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        _global.RefreshRequested -= RefreshMe;
    }
}
