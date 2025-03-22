using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public partial class Customers : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<Customers>? L { get; set; }
    [Inject, NotNull] private ICustomerClient? CustomerClient { get; set; }
    [Inject, NotNull] private IDialogService? DialogProvider { get; set; }
    [Inject, NotNull] private NavigationManager? Navigation { get; set; }

    private GetAllCustomersResponse? _customers;
    private RefreshModel _refreshModel = new();
    private CancellationTokenSource _cls = new();
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
            _customers = await CustomerClient.GetAllCustomersAsync(500, 0);
            StateHasChanged();
        }
    }
    
    private Task OpenCustomerDialog(Customer? customer, bool isNew)
    {
        var parameters = new DialogParameters<AddCustomerDialog>
        {
            { x => x.Customer, customer },
            { x => x.IsNew, isNew },
            { x => x.Refresh, _refreshModel },
        };
        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Large,
            CloseButton = true,
            FullWidth = true
        };
        return DialogProvider.ShowAsync<AddCustomerDialog>(isNew ? L["Put customer"] : L["Edit customer"], parameters, options);
    }

    private async Task RefreshMeAsync()
    {
        _customers = await CustomerClient.GetAllCustomersAsync(500, 0);
        StateHasChanged();
    }

    public void Dispose()
    {
        _refreshModel.RefreshRequestedAsync -= RefreshMeAsync;
        _cls.Cancel();
    }
}