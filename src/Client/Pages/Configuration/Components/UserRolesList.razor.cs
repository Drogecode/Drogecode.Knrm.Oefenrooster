using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;

public sealed partial class UserRolesList : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<UserRolesList>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private IUserRoleClient? UserRoleClient { get; set; }
    [Inject, NotNull] private UserRepository? UserRepository { get; set; }
    [Parameter] public Guid? Id { get; set; }
    private readonly CancellationTokenSource _cls = new();
    private GetUserRoleResponse? _userRole;
    private List<DrogeUser>? _users;
    private bool? _saved ;
    private bool _editName ;
    private bool _isNew;
    private bool _isSaving;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Id is not null)
            {
                DebugHelper.WriteLine($"Loading user role: {Id}");
                _userRole = await UserRoleClient.GetByIdAsync(Id.Value, _cls.Token);
                _users = await UserRepository.GetAllUsersAsync(false, true, false, _cls.Token);
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
        if (_isSaving) return;
        _isSaving = true;
        StateHasChanged();
        try
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
        }
        finally
        {
            _isSaving = false;
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}