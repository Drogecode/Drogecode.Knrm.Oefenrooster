using Blazored.LocalStorage;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;
public sealed partial class MainLayout : IDisposable
{
    [Inject] private IStringLocalizer<MainLayout> L { get; set; } = default!;
    [Inject] private SignOutSessionStateManager SignOutManager { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;

    private DrogeCodeGlobal _global { get; set; } = new();
    private MudThemeProvider _mudThemeProvider = new();
    private IDictionary<NotificationMessage, bool> _messages = null;
    private List<PlannerTrainingType>? _trainingTypes;
    private bool _isDarkMode;
    private bool _drawerOpen = true;
    private bool _settingsOpen = true;
    private bool _newNotificationsAvailable = false;
    private bool _isAuthenticated;

    protected override async Task OnParametersSetAsync()
    {
        _global.RefreshRequested += RefreshMe;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetTrainingTypes();
            RefreshMe();
        }
    }
    private async Task GetTrainingTypes()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        _isAuthenticated = authState?.User?.Identity?.IsAuthenticated ?? false;
        if (_isAuthenticated)
        {
            if (_trainingTypes == null)
                _trainingTypes = await _scheduleRepository.GetTrainingTypes();
            RefreshMe();
        }

    }

    private async Task BeginLogout(MouseEventArgs args)
    {
        await SignOutManager.SetSignOutState();
        Navigation.NavigateTo("authentication/logout");
    }
    protected async Task NotAuthorized()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        _isAuthenticated = authState.User?.Identity?.IsAuthenticated ?? false;
        if (!_isAuthenticated)
            Navigation.NavigateTo("landing_page");
    }

    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    public void ToggleOpen()
    {
        _settingsOpen = !_settingsOpen;
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
