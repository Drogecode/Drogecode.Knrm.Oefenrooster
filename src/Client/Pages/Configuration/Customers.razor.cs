using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public partial class Customers : ComponentBase
{
    [Inject] private IStringLocalizer<Customers> L { get; set; } = default!;
    [Inject] private ICustomerClient CustomerClient { get; set; } = default!;

    private GetAllCustomersResponse? _customers;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _customers = await CustomerClient.GetAllCustomersAsync();
            StateHasChanged();
        }
    }
}