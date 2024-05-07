using System.Diagnostics.CodeAnalysis;
using Blazored.LocalStorage;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;

public sealed partial class Theming : IDisposable
{
    [Inject] private IStringLocalizer<Theming> L { get; set; } = default!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
    [Inject, NotNull] private ICustomerSettingsClient? CustomerSettingsClient { get; set; }
    [Inject, NotNull] private IJSRuntime? JsRuntime { get; set; }
    [Parameter, EditorRequired] public DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter, EditorRequired] public MudThemeProvider MudThemeProvider { get; set; } = default!;
    [Parameter] public EventCallback<bool> IsDarkModeChanged { get; set; }
    [Inject] ISnackbar Snackbar { get; set; } = default!;

    [Parameter]
    public bool IsDarkMode
    {
        get { return _isDarkMode; }
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

    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }

    private DarkLightMode _darkModeToggle;
    private LocalUserSettings? _localUserSettings;
    private DotNetObjectReference<Theming>? _dotNetHelper;
    private CancellationTokenSource _cls = new();
    private DateTime _lastVisibilityChange = DateTime.UtcNow;
    private bool _isDarkMode;
    private bool _watchStarted;
    private bool _isTaco;
    private int counter = 0;

    protected override async Task OnInitializedAsync()
    {
        _localUserSettings = (await LocalStorage.GetItemAsync<LocalUserSettings>("localUserSettings")) ?? new LocalUserSettings();
        DarkModeToggle = _localUserSettings.DarkLightMode;
        if (AuthenticationState is not null)
        {
            var authState = await AuthenticationState;
            var user = authState?.User;
            if (user != null)
            {
                _isTaco = user.IsInRole(AccessesNames.AUTH_Taco);
            }
        }
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
            _dotNetHelper = DotNetObjectReference.Create(this);
            if (_cls.IsCancellationRequested) return;
            await JsRuntime.InvokeVoidAsync("AddVisibilityWatcher", _dotNetHelper);
        }
    }

    [JSInvokable]
    public async Task VisibilityChange(string newState, bool isIos, string userAgent)
    {
        if (string.Compare(newState, "visible", StringComparison.InvariantCulture) != 0)
            return;
        if (DarkModeToggle != DarkLightMode.System) return;
        if ((_isTaco && _lastVisibilityChange.AddSeconds(5).CompareTo(DateTime.UtcNow) > 0) ||
            (!_isTaco && _lastVisibilityChange.AddMinutes(3).CompareTo(DateTime.UtcNow) > 0))
            return;
        _lastVisibilityChange = DateTime.UtcNow;
        if (isIos && await CustomerSettingsClient.GetIosDarkLightCheckAsync(_cls.Token))
        {
            //https://forums.developer.apple.com/forums/thread/739154
            await JsRuntime.InvokeVoidAsync("ColorschemeFix");
        }

        if (_isTaco)
        {
            Snackbar.Add(L["Visibility change"] + $": {isIos} {userAgent}", Severity.Warning, configure: Config, key: "VisibilityChange");

            void Config(SnackbarOptions options)
            {
                options.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
                options.ActionColor = Color.Default;
            }
        }

        await Global.CallVisibilityChangeAsync();
        await OnSystemPreferenceChanged(await MudThemeProvider.GetSystemPreference());
    }

    public async Task OnSystemPreferenceChanged(bool newValue)
    {
        if (DarkModeToggle == DarkLightMode.System && IsDarkMode != newValue)
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