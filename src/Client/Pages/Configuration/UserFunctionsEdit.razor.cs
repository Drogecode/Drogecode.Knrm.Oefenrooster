using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;
using Microsoft.AspNetCore.Components;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public partial class UserFunctionsEdit : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<UserRolesEdit>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private IFunctionClient? FunctionClient { get; set; }
    [Parameter] public Guid? Id { get; set; }
    private readonly CancellationTokenSource _cls = new();
    private GetUserRoleResponse? _userRole;
    private GetLinkedUsersByIdResponse? _linkedUsers;
    private List<DrogeUser>? _users;
    private bool? _saved = null;
    private bool _editName = false;
    private bool _isNew;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Id is not null)
            {
                DebugHelper.WriteLine($"Loading user role: {Id}");
            }
            else
            {
                DebugHelper.WriteLine("Creating new role");
                _editName = true;
                _isNew = true;
                _userRole = new GetUserRoleResponse
                {
                    Role = new DrogeUserRole()
                };
            }

            StateHasChanged();
        }
    }

    private async Task Submit()
    {
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}