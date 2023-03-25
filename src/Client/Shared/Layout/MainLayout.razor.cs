using Blazored.LocalStorage;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
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
    [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;

    private DrogeCodeGlobal _global { get; set; } = new();
    private MudThemeProvider _mudThemeProvider = new();
    private IDictionary<NotificationMessage, bool> _messages = null;
    private bool _isDarkMode;
    private bool _drawerOpen = true;
    private bool _settingsOpen = true;
    private bool _newNotificationsAvailable = false;
    private bool _isAuthenticated;
    private bool _watchStarted;
    private DarkLightMode _darkModeToggle;
    private string _name = string.Empty;
    private LocalUserSettings? _localUserSettings;

    protected override async Task OnParametersSetAsync()
    {
        _global.RefreshRequested += RefreshMe;
        _localUserSettings = (await LocalStorage.GetItemAsync<LocalUserSettings>("localUserSettings")) ?? new LocalUserSettings();
        DarkModeToggle = _localUserSettings.DarkLightMode;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            switch (DarkModeToggle)
            {
                case DarkLightMode.System:
                    _isDarkMode = await _mudThemeProvider.GetSystemPreference();
                    await StartWatch();
                    break;
                case DarkLightMode.Light:
                    _isDarkMode = false;
                    break;
                case DarkLightMode.Dark:
                    _isDarkMode = true;
                    break;
            }
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

    private DarkLightMode DarkModeToggle
    {
        get { return _darkModeToggle; }
        set
        {
            if (_darkModeToggle == value) return;
            _darkModeToggle = value;
            if (_localUserSettings == null)
            {
                Console.WriteLine("_localUserSettings is null");
                return;
            }
            _localUserSettings.DarkLightMode = _darkModeToggle;
            LocalStorage.SetItemAsync("localUserSettings", _localUserSettings);
        }
    }

    public async Task ToggleDarkLight()
    {
        switch (DarkModeToggle)
        {
            case DarkLightMode.System:
                DarkModeToggle = DarkLightMode.Light;
                _isDarkMode = false;
                break;
            case DarkLightMode.Light:
                DarkModeToggle = DarkLightMode.Dark;
                _isDarkMode = true;
                break;
            case DarkLightMode.Dark:
                DarkModeToggle = DarkLightMode.System;
                _isDarkMode = await _mudThemeProvider.GetSystemPreference();
                await StartWatch();
                break;
        }
        Console.WriteLine($"New setting: {DarkModeToggle} {_isDarkMode}");
        RefreshMe();
    }

    private async Task StartWatch()
    {
        if (!_watchStarted)
        {
            Console.WriteLine($"Start watch: {DarkModeToggle} {_isDarkMode}");
            _watchStarted = true;
            await _mudThemeProvider.WatchSystemPreference(OnSystemPreferenceChanged);
        }
    }

    public async Task OnSystemPreferenceChanged(bool newValue)
    {
        if (_darkModeToggle == DarkLightMode.System)
        {
            Console.WriteLine($"System dark light changed: {newValue}");
            _isDarkMode = newValue;
            RefreshMe();
        }
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
