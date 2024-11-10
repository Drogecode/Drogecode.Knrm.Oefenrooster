using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public partial class UserRolesEdit : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<UserRolesEdit>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private IUserRoleClient? UserRoleClient { get; set; }
    [Inject, NotNull] private UserRepository? UserRepository { get; set; }
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
                _userRole = await UserRoleClient.GetByIdAsync(Id.Value, _cls.Token);
                _linkedUsers = await UserRoleClient.GetLinkedUsersByIdAsync(Id.Value, _cls.Token);
                _users = await UserRepository.GetAllUsersAsync(false, false, false, _cls.Token);
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
        if (string.IsNullOrWhiteSpace(_userRole?.Role?.Name))
        {
            _saved = false;
            StateHasChanged();
            return;
        }

        _editName = false;
        StateHasChanged();
        if (_isNew)
        {
           var newResult = await UserRoleClient.NewUserRoleAsync(_userRole?.Role, _cls.Token);
           if (newResult.NewId is not null)
           {
               _saved = newResult.Success;
               _userRole!.Role!.Id = newResult.NewId.Value;
           }
           else
           {
               _saved = false;
           }
        }
        else
        {
            _saved = (await UserRoleClient.PatchUserRoleAsync(_userRole?.Role, _cls.Token)).Success;
        }

        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}