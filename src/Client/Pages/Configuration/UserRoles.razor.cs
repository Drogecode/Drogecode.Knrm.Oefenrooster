using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class UserRoles : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<UserRoles>? L { get; set; }
    [Inject, NotNull] private IUserRoleClient? UserRoleClient { get; set; }
    [Inject, NotNull] private NavigationManager? Navigation { get; set; }
    private readonly CancellationTokenSource _cls = new();
    private MultipleDrogeUserRolesBasicResponse? _userRoles;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _userRoles = await UserRoleClient.GetAllBasicAsync(_cls.Token);
            StateHasChanged();
        }
    }

    private void ClickUserRole(DrogeUserRoleBasic userRole)
    {
        Navigation.NavigateTo($"/configuration/user-roles/edit/{userRole.Id}");
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}