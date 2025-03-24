using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class CustomerSettings : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<CustomerSettings>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private UserRepository? UserRepository { get; set; }
    [Inject, NotNull] private ICustomerClient? CustomerClient { get; set; }
    [Inject, NotNull] private ILinkedCustomerClient? LinkedCustomerClient { get; set; }

    [Parameter] public Guid? Id { get; set; }

    private IEnumerable<DrogeUser> _selectedUsersCurrent = new List<DrogeUser>();
    private IEnumerable<DrogeUser> _selectedUsersOther = new List<DrogeUser>();
    private CancellationTokenSource _cls = new();
    private GetCustomerResponse? _customer;
    private List<DrogeUser>? _usersDifferentCustomer;
    private List<DrogeUser>? _usersThisCustomer;
    private GetAllUsersWithLinkToCustomerResponse? _linkedUsers;

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
        _customer = await CustomerClient.GetCustomerByIdAsync(Id.Value);
        _usersDifferentCustomer = await UserRepository.GetAllDifferentCustomerAsync(Id.Value, false, _cls.Token);
        _usersThisCustomer = await UserRepository.GetAllUsersAsync(false, false, false, _cls.Token);
        _linkedUsers = await LinkedCustomerClient.GetAllUsersWithLinkToCustomerAsync(Id.Value, _cls.Token);
        if (_usersDifferentCustomer is not null && _usersDifferentCustomer.Count != 0)
        {
            var linkedDifferent = new List<DrogeUser>();
            foreach (var t in _usersDifferentCustomer)
            {
                if (_linkedUsers.LinkInfo?.Any(x => x.DrogeUserOther != null && x.DrogeUserOther.Id == t.Id) == true)
                {
                    linkedDifferent.Add(t);
                }
            }

            foreach (var linked in linkedDifferent)
            {
                _usersDifferentCustomer.Remove(linked);
            }
        }

        if (_usersThisCustomer is not null && _usersThisCustomer.Count != 0)
        {
            var linkedCurrent = new List<DrogeUser>();
            foreach (var t in _usersThisCustomer)
            {
                if (_linkedUsers.LinkInfo?.Any(x => x.DrogeUserCurrent != null && x.DrogeUserCurrent.Id == t.Id) == true)
                {
                    linkedCurrent.Add(t);
                }
            }

            foreach (var linked in linkedCurrent)
            {
                _usersThisCustomer.Remove(linked);
            }
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }

    private async Task Save()
    {
        if (Id is null || !_selectedUsersCurrent.Any() || !_selectedUsersOther.Any())
            return;
        _allowSave = false;
        await LinkedCustomerClient.LinkUserToCustomerAsync(new LinkUserToCustomerRequest()
        {
            CustomerId = Id.Value,
            UserId = _selectedUsersCurrent.FirstOrDefault()!.Id,
            GlobalUserId = _selectedUsersOther.FirstOrDefault()!.Id,
            IsActive = true,
            CreateNew = false
        });
        _selectedUsersCurrent = [];
        _selectedUsersOther = [];
        await RefreshMe();
        _allowSave = true;
    }

    private void SelectionCurrentCustomer(IEnumerable<DrogeUser> selection)
    {
        _selectedUsersCurrent = selection;
        if (_selectedUsersCurrent.Any() && _selectedUsersOther.Any())
            _allowSave = true;
        else
            _allowSave = false;
    }

    private void SelectionOtherCustomer(IEnumerable<DrogeUser> selection)
    {
        _selectedUsersOther = selection;
        if (_selectedUsersCurrent.Any() && _selectedUsersOther.Any())
            _allowSave = true;
        else
            _allowSave = false;
    }
}