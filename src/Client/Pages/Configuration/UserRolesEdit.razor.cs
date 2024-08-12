using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public partial class UserRolesEdit : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<UserRolesEdit>? L { get; set; }
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject, NotNull] private IUserRoleClient? UserRoleClient { get; set; }
    [Parameter] public Guid? Id { get; set; }
    private readonly CancellationTokenSource _cls = new();
    private GetUserRoleResponse? _userRole;
    private bool? saved = null;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Id is not null)
        {
            _userRole = await UserRoleClient.GetByIdAsync(Id.Value, _cls.Token);
            StateHasChanged();
        }
    }

    private async Task Submit()
    {
       saved = (await UserRoleClient.PatchUserRoleAsync(_userRole?.Role, _cls.Token)).Success;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}