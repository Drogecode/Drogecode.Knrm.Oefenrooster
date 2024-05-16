﻿using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;
public sealed partial class MainLayout : IDisposable
{
    [Inject] private IStringLocalizer<MainLayout> L { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private TrainingTypesRepository TrainingTypesRepository { get; set; } = default!;
    [Inject] private UserRepository UserRepository { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private readonly DrogeCodeGlobal _global = new();
    private MudThemeProvider _mudThemeProvider = new();
    private IDictionary<NotificationMessage, bool> _messages = null;
    private List<PlannerTrainingType>? _trainingTypes;
    private HubConnection? _hubConnection;
    private readonly CancellationTokenSource _cls = new();
    private bool _isDarkMode;
    private bool _isAuthenticated;
    private bool _isOffline;
    private bool _drawerOpen = true;
    private bool _settingsOpen = true;
    private bool _newNotificationsAvailable = false;

    private MudTheme _myCustomTheme = new()
    {
        Palette = new PaletteLight(),
        PaletteDark = new PaletteDark(),
    };
    private Action<SnackbarOptions> _snackbarConfig = (SnackbarOptions options) =>
    {
        options.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
        options.RequireInteraction = false;
        options.ShowCloseIcon = true;
        options.VisibleStateDuration = 20000;
    };

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            _isAuthenticated = authState.User?.Identity?.IsAuthenticated ?? false;
            if (!_isAuthenticated)
            {
                if (!Navigation.Uri.Contains("/authentication/login-callback") && !Navigation.Uri.Contains("/landing_page"))
                    Navigation.NavigateTo("/authentication/login");
                return;
            }

            var dbUser = await UserRepository.GetCurrentUserAsync();//Force creation of user.
            if (dbUser?.Id != null && dbUser.Id != Guid.Empty)
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(Navigation.ToAbsoluteUri("/hub/precomhub"))
                .Build();
                _hubConnection.On<string, string>($"ReceivePrecomAlert_{dbUser.Id}", (user, message) =>
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
        }
        catch (HttpRequestException ex)
        {
            DebugHelper.WriteLine("Faild to start hubconnection.", ex);
        }
    }

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
                _trainingTypes = await TrainingTypesRepository.GetTrainingTypes(false, false);
            RefreshMe();
        }

    }

    private async Task Login(MouseEventArgs args)
    {
        Navigation.NavigateTo("/landing_page");
    }

    public void ShowSnackbarAssignmentChanged(PlanUser user, PlannedTraining training)
    {
        var key = $"table_{user.UserId}_{training.TrainingId}";
        Snackbar.RemoveByKey(key);
        Snackbar.Add(L["{0} {1} {2} {3} {4}", user.Assigned ? L["Assigned"] : L["Removed"], user.Name, user.Assigned ? L["to"] : L["from"], training.DateStart.ToShortDateString(), training.Name ?? ""], (user.Availability == Availability.NotAvailable || user.Availability == Availability.Maybe) && user.Assigned ? Severity.Warning : user.Assigned ? Severity.Normal : Severity.Info, configure: _snackbarConfig, key: key);
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
        _cls.Cancel();
    }
}
