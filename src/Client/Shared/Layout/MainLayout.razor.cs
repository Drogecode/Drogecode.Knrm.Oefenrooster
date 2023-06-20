using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;
public sealed partial class MainLayout : IDisposable
{
    [Inject] private IStringLocalizer<MainLayout> L { get; set; } = default!;
    [Inject] private SignOutSessionStateManager SignOutManager { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private IOfflineService _offlineService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private DrogeCodeGlobal _global { get; set; } = new();
    private MudThemeProvider _mudThemeProvider = new();
    private IDictionary<NotificationMessage, bool> _messages = null;
    private List<PlannerTrainingType>? _trainingTypes;
    private HubConnection? _hubConnection;
    private CancellationTokenSource _cls = new();
    private bool _isDarkMode;
    private bool _isAuthenticated;
    private bool _isOffline;
    private bool _drawerOpen = true;
    private bool _settingsOpen = true;
    private bool _newNotificationsAvailable = false;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/hub/precomhub"))
                .Build();

            _hubConnection.On<string, string>("ReceivePrecomAlert", (user, message) =>
            {
                var config = (SnackbarOptions options) =>
                {
                    options.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow;
                    options.RequireInteraction = true;
                    options.ShowCloseIcon = true;
                };
                Snackbar.Add($"PreCom: {message}", Severity.Error, configure: config, key: "precom");
            });

            await _hubConnection.StartAsync(_cls.Token);
        }
        catch (HttpRequestException ex)
        {
            DebugHelper.WriteLine("Faild to start hubconnection.", ex);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        _global.RefreshRequested += RefreshMe;
        _offlineService.OfflineStatusChanged += OfflineStatusChanged;
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

    private void OfflineStatusChanged()
    {
        _isOffline = _offlineService.Offline;
        RefreshMe();
    }

    private void RefreshMe()
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        _global.RefreshRequested -= RefreshMe;
        _cls.Cancel();
    }
}
