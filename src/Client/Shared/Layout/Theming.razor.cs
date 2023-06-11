using Blazored.LocalStorage;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;

public sealed partial class Theming : IDisposable
{
    [Inject] private IStringLocalizer<Theming> L { get; set; } = default!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
    [Parameter, EditorRequired] public DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter, EditorRequired] public MudThemeProvider MudThemeProvider { get; set; } = default!;
    [Parameter] public EventCallback<bool> IsDarkModeChanged { get; set; }
    [Parameter]
    public bool IsDarkMode
    {
        get
        {
            return _isDarkMode;
        }
        set
        {
            if (_isDarkMode == value) return;
            _isDarkMode = value;
            if (IsDarkModeChanged.HasDelegate)
            {
                IsDarkModeChanged.InvokeAsync(value);
            }
        }
    }
    private bool _isDarkMode;
    private DarkLightMode _darkModeToggle;
    private LocalUserSettings? _localUserSettings;
    private bool _watchStarted;
    private int counter = 0;
    protected override async Task OnInitializedAsync()
    {
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
                    IsDarkMode = await MudThemeProvider.GetSystemPreference();
                    await StartWatch();
                    break;
                case DarkLightMode.Light:
                    IsDarkMode = false;
                    break;
                case DarkLightMode.Dark:
                    IsDarkMode = true;
                    break;
            }
            await Global.CallDarkLightChangedAsync(IsDarkMode);
            await Global.CallRequestRefreshAsync();
        }
    }

    private DarkLightMode DarkModeToggle
    {
        get { return _darkModeToggle; }
        set
        {
            if (_darkModeToggle == value) return;
            _darkModeToggle = value;
            counter++;
            RefreshMe();
            if (_localUserSettings == null)
            {
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
                IsDarkMode = false;
                break;
            case DarkLightMode.Light:
                DarkModeToggle = DarkLightMode.Dark;
                IsDarkMode = true;
                break;
            case DarkLightMode.Dark:
                DarkModeToggle = DarkLightMode.System;
                IsDarkMode = await MudThemeProvider.GetSystemPreference();
                await StartWatch();
                break;
        }
        await Global.CallDarkLightChangedAsync(IsDarkMode);
    }

    private async Task StartWatch()
    {
        if (!_watchStarted)
        {
            _watchStarted = true;
            await MudThemeProvider.WatchSystemPreference(OnSystemPreferenceChanged);
        }
    }

    public async Task OnSystemPreferenceChanged(bool newValue)
    {
        if (DarkModeToggle == DarkLightMode.System)
        {
            IsDarkMode = newValue;
            RefreshMe();
            await Global.CallDarkLightChangedAsync(IsDarkMode);
        }
    }

    private void RefreshMe()
    {
        StateHasChanged();
    }

    public void Dispose()
    {
    }
}
