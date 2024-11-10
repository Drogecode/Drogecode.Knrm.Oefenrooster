using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class UserRoles : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<UserRoles>? L { get; set; }
    [Inject, NotNull] private IUserRoleClient? UserRoleClient { get; set; }
    [Inject, NotNull] private NavigationManager? Navigation { get; set; }
    private readonly CancellationTokenSource _cls = new();
    private MultipleDrogeUserRolesResponse? _userRoles;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _userRoles = await UserRoleClient.GetAllAsync(_cls.Token);
            StateHasChanged();
        }
    }

    private void ClickUserRole(DrogeUserRole userRole)
    {
        Navigation.NavigateTo($"/configuration/user-roles/edit/{userRole.Id}");
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}