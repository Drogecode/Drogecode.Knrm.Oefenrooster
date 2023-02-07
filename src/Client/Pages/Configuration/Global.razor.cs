using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Security.Claims;
using System.Text;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class Global : IDisposable
{
    [Inject] private IStringLocalizer<Global> L { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    [Inject] private ConfigurationRepository configurationRepository { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private FunctionRepository _functionRepository { get; set; } = default!;
    private ClaimsPrincipal _user;
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private RefreshModel _refreshModel = new();
    private bool _isAuthenticated;
    private string _name = string.Empty;

    private bool _clickedUpdate;


    protected override void OnInitialized()
    {
        _refreshModel.RefreshRequested += RefreshMe;
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
        _users = await _userRepository.GetAllUsersAsync();
        _functions = await _functionRepository.GetAllFunctionsAsync();
    }

    private async Task UpdateDatabase()
    {
        _clickedUpdate = true;
        await configurationRepository.UpgradeDatabaseAsync();
    }

    private void AddUser()
    {

        DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        _dialogProvider.Show<AddUserDialog>(L["Add user"], new DialogParameters { { "Functions", _functions }, { "Refresh", _refreshModel } }, options);
    }

    private async void ChangeUser(DrogeUser user)
    {
        DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        _dialogProvider.Show<EditUserDialog>(L["Edit user"], new DialogParameters { { "User", user }, { "Functions", _functions }, { "Refresh", _refreshModel } }, options);
    }
    private void RefreshMe()
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        _refreshModel.RefreshRequested -= RefreshMe;
    }
}
