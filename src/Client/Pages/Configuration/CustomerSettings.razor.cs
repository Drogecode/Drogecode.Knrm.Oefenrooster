using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserGlobal;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class CustomerSettings : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<CustomerSettings>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private UserRepository? UserRepository { get; set; }
    [Inject, NotNull] private ICustomerClient? CustomerClient { get; set; }
    [Inject, NotNull] private ILinkedCustomerClient? LinkedCustomerClient { get; set; }
    [Inject, NotNull] private IUserGlobalClient? UserGlobalClient { get; set; }

    [Parameter] public Guid? Id { get; set; }

    private IEnumerable<DrogeUser> _selectedUsersOther = [];
    private Guid? _selectedUserGlobal;
    private CancellationTokenSource _cls = new();
    private GetCustomerResponse? _customer;
    private List<DrogeUser>? _usersDifferentCustomer;
    private GetAllUsersWithLinkToCustomerResponse? _linkedUsers;
    private AllDrogeUserGlobalResponse? _usersGlobal;

    private bool _allowSave = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Id is null) return;
            await RefreshMe();
            StateHasChanged();
        }
    }

    private async Task RefreshMe()
    {
        if (Id is null) return;
        _customer = await CustomerClient.GetCustomerByIdAsync(Id.Value);
        _usersDifferentCustomer = await UserRepository.GetAllDifferentCustomerAsync(Id.Value, false, _cls.Token);
        _linkedUsers = await LinkedCustomerClient.GetAllUsersWithLinkToCustomerAsync(Id.Value, _cls.Token);
        _usersGlobal = await UserGlobalClient.GetAllAsync(_cls.Token);
        
        if (_usersDifferentCustomer is not null && _usersDifferentCustomer.Count != 0)
        {
            var linkedDifferent = new List<DrogeUser>();
            foreach (var t in _usersDifferentCustomer)
            {
                if (_linkedUsers.LinkInfo?.Any(x => x.DrogeUser != null && x.DrogeUser.Id == t.Id) == true)
                {
                    linkedDifferent.Add(t);
                }
            }

            foreach (var linked in linkedDifferent)
            {
                _usersDifferentCustomer.Remove(linked);
            }
        }
        if (_usersGlobal?.GlobalUsers is not null && _usersGlobal.GlobalUsers.Count != 0)
        {
            var linkedDifferent = new List<DrogeUserGlobal>();
            foreach (var t in _usersGlobal.GlobalUsers)
            {
                if (_linkedUsers.LinkInfo?.Any(x => x.UserGlobal != null && x.UserGlobal.Id == t.Id) == true)
                {
                    linkedDifferent.Add(t);
                }
            }

            foreach (var linked in linkedDifferent)
            {
                _usersGlobal.GlobalUsers.Remove(linked);
            }
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }

    private async Task Save()
    {
        if (Id is null || !_selectedUsersOther.Any() || _selectedUserGlobal is null)
            return;
        _allowSave = false;
        await LinkedCustomerClient.LinkUserToCustomerAsync(new LinkUserToCustomerRequest()
        {
            CustomerId = Id.Value,
            UserId = _selectedUsersOther.FirstOrDefault()!.Id,
            GlobalUserId = _selectedUserGlobal.Value,
            IsActive = true,
            CreateNew = false
        });
        _selectedUserGlobal = null;
        _selectedUsersOther = [];
        await RefreshMe();
        _allowSave = true;
    }

    private void SelectionOtherCustomer(IEnumerable<DrogeUser> selection)
    {
        _selectedUsersOther = selection;
        if (_selectedUserGlobal is not null && _selectedUsersOther.Any())
            _allowSave = true;
        else
            _allowSave = false;
    }

    private void SelectionGlobalUsers(Guid? selection)
    {
        DebugHelper.WriteLine($"SelectionGlobalUsers: {selection}");
        if (selection is null)
        {
            _selectedUserGlobal = Guid.Empty;
            return;
        }
        _selectedUserGlobal = selection;
        if (_selectedUserGlobal is not null && _selectedUsersOther.Any())
            _allowSave = true;
        else
            _allowSave = false;
    }
}