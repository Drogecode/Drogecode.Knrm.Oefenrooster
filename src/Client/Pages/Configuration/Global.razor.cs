using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class Global : IDisposable
{
    [Inject] private IStringLocalizer<Global> L { get; set; } = default!;
    [Inject] private CustomStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    [Inject] private ConfigurationRepository _configurationRepository { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private FunctionRepository _functionRepository { get; set; } = default!;
    private ClaimsPrincipal _user;
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private RefreshModel _refreshModel = new();
    private bool _isAuthenticated;
    private string _name = string.Empty;

    private bool? _clickedUpdate;
    private bool? _usersSynced;
    private bool? _specialDatesUpdated;


    protected override void OnInitialized()
    {
        _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
    }
    protected override async Task OnParametersSetAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        _user = authState.User;
        _isAuthenticated = authState.User?.Identity?.IsAuthenticated ?? false;
        if (_isAuthenticated)
        {
            var dbUser = await _userRepository.GetCurrentUserAsync();
            _name = authState!.User!.Identity!.Name ?? string.Empty;
        }
        _users = await _userRepository.GetAllUsersAsync(true);
        _functions = await _functionRepository.GetAllFunctionsAsync();
    }

    private async Task UpdateDatabase()
    {
        _clickedUpdate = await _configurationRepository.UpgradeDatabaseAsync();
        StateHasChanged();
    }

    private async Task SyncUsers()
    {
        _usersSynced = null;
        _usersSynced = await _userRepository.SyncAllUsersAsync();
        if (_usersSynced == true)
        {
            _users = await _userRepository.GetAllUsersAsync(true);
            await RefreshMeAsync();
        }
    }
    private async Task UpdateSpecialDates()
    {
        _specialDatesUpdated = await _configurationRepository.UpdateSpecialDates();
        StateHasChanged();
    }

    private void AddUser()
    {

        var options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        var parameters = new DialogParameters { { "Functions", _functions }, { "Refresh", _refreshModel } };
        _dialogProvider.Show<AddUserDialog>(L["Add user"], parameters, options);
    }

    private void ChangeUser(DrogeUser user)
    {
        var options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        var parameters = new DialogParameters { { "User", user }, { "Functions", _functions }, { "Refresh", _refreshModel } };
        _dialogProvider.Show<EditUserDialog>(L["Edit user"], parameters, options);
    }
    private async Task RefreshMeAsync()
    {
        _users = await _userRepository.GetAllUsersAsync(true);
        StateHasChanged();
    }

    public void Dispose()
    {
        _refreshModel.RefreshRequestedAsync -= RefreshMeAsync;
    }
}
