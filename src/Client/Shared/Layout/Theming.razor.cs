using System.Diagnostics.CodeAnalysis;
using Blazored.LocalStorage;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;

public sealed partial class Theming : IDisposable
{
    [Inject] private IStringLocalizer<Theming> L { get; set; } = default!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
    [Inject, NotNull] private ICustomerSettingsClient? CustomerSettingsClient { get; set; }
    [Inject, NotNull] private IJSRuntime? JsRuntime { get; set; }
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter, EditorRequired] public DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter, EditorRequired] public MudThemeProvider MudThemeProvider { get; set; } = default!;
    [Parameter] public EventCallback<bool> IsDarkModeChanged { get; set; }
    [Parameter] public EventCallback<DarkLightMode> DarkModeToggleChanged { get; set; }
    [Inject] ISnackbar Snackbar { get; set; } = default!;


    private DarkLightMode _darkModeToggle;
    private LocalUserSettings? _localUserSettings;
    private DotNetObjectReference<Theming>? _dotNetHelper;
    private CancellationTokenSource _cls = new();
    private DateTime _lastVisibilityChange = DateTime.UtcNow;
    private bool _isDarkMode;
    private bool _watchStarted;
    private bool _isTaco;
    private int _counter = 0;

    [Parameter]
    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            if (_isDarkMode == value) return;
            _isDarkMode = value;
            Global.DarkMode = value;
            if (IsDarkModeChanged.HasDelegate)
            {
                IsDarkModeChanged.InvokeAsync(value);
            }
        }
    }

    [Parameter]
    public DarkLightMode DarkModeToggle
    {
        get => _darkModeToggle;
        set
        {
            if (_darkModeToggle == value) return;
            _darkModeToggle = value;
            _counter++;
            RefreshMe();
            if (_localUserSettings == null)
            {
                return;
            }

            _localUserSettings.DarkLightMode = _darkModeToggle;
            LocalStorage.SetItemAsync("localUserSettings", _localUserSettings);
            if (DarkModeToggleChanged.HasDelegate)
            {
                DarkModeToggleChanged.InvokeAsync(value);
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _localUserSettings = (await LocalStorage.GetItemAsync<LocalUserSettings>("localUserSettings")) ?? new LocalUserSettings();
            DarkModeToggle = _localUserSettings.DarkLightMode;
            _isTaco = await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_Taco);
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
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await Global.CallDarkLightChangedAsync(IsDarkMode);
            await Global.CallRequestRefreshAsync();
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
            _dotNetHelper = DotNetObjectReference.Create(this);
            if (_cls.IsCancellationRequested) return;
            await JsRuntime.InvokeVoidAsync("AddVisibilityWatcher", _dotNetHelper);
        }
    }

    [JSInvokable]
    public async Task VisibilityChange(string newState, bool isIos)
    {
        if (string.Compare(newState, "visible", StringComparison.InvariantCulture) != 0)
            return;
        await Task.Delay(50);
        await OnSystemPreferenceChanged(await MudThemeProvider.GetSystemPreference()); // always check dark/light on reopen
        if (_lastVisibilityChange.AddMinutes(3).CompareTo(DateTime.UtcNow) > 0)
            return;
        _lastVisibilityChange = DateTime.UtcNow;
        if (false && isIos && await CustomerSettingsClient.GetIosDarkLightCheckAsync(_cls.Token) && DarkModeToggle == DarkLightMode.System)
        {
            // Disable, se if it works without on the latest ios version.
            // https://forums.developer.apple.com/forums/thread/739154
            await JsRuntime.InvokeVoidAsync("ColorschemeFix");
        }

        await Global.CallVisibilityChangeAsync();
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
        _cls.Cancel();
    }
}