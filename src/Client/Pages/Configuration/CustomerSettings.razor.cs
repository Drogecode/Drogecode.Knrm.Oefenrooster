using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class CustomerSettings
{
    [Inject, NotNull] private IStringLocalizer<CustomerSettings>? L { get; set; }
    [Inject, NotNull] private UserRepository? UserRepository { get; set; }
    [Inject, NotNull] private ICustomerClient? CustomerClient { get; set; }
    [Inject, NotNull] private ILinkedCustomerClient? LinkedCustomerClient { get; set; }
    [Parameter] public Guid? Id { get; set; }

    private CancellationTokenSource _cls = new();
    private GetCustomerResponse? Customer { get; set; }
    private List<DrogeUser>? UsersDifferentCustomer { get; set; }
    private List<DrogeUser>? UsersThisCustomer { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Id is null) return;
            Customer = await CustomerClient.GetCustomerByIdAsync(Id.Value);
            UsersDifferentCustomer = await UserRepository.GetAllDifferentCustomerAsync(Id.Value, true, _cls.Token);
            UsersThisCustomer = await UserRepository.GetAllUsersAsync(true, false, false, _cls.Token);
            var linkedUsers = await LinkedCustomerClient.GetAllUsersWithLinkToCustomerAsync(Id.Value, _cls.Token);
            if (UsersDifferentCustomer is not null && UsersDifferentCustomer.Count != 0)
            {
                for(int i = 0; i < UsersDifferentCustomer.Count; i++)
                {
                    if (linkedUsers.LinkInfo?.Any(x => x.DrogeUserOther != null && x.DrogeUserOther.Id == UsersDifferentCustomer[i].Id) == true)
                    {
                        UsersDifferentCustomer.RemoveAt(i);
                    }
                }
            }
            if (UsersThisCustomer is not null && UsersThisCustomer.Count != 0)
            {
                for(int i = 0; i < UsersThisCustomer.Count; i++)
                {
                    if (linkedUsers.LinkInfo?.Any(x => x.DrogeUserCurrent != null && x.DrogeUserCurrent.Id == UsersThisCustomer[i].Id) == true)
                    {
                        UsersThisCustomer.RemoveAt(i);
                    }
                }
            }

            StateHasChanged();
        }
    }
}