using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;

public sealed partial class AddCustomerDialog : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<Customers>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private ICustomerClient? CustomerClient { get; set; }
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public Customer? Customer { get; set; }
    [Parameter] public bool? IsNew { get; set; }
    [AllowNull] private MudForm _form;
    private CancellationTokenSource _cls = new();
    private bool _success;
    private string[] _errors = [];
    void Cancel() => MudDialog.Cancel();

    protected override void OnParametersSet()
    {
        if (IsNew == true && Customer is null)
        {
            Customer = new Customer();
        }
    }

    private async Task Submit()
    {
        if (Customer is null)
        {
            DebugHelper.WriteLine("Customer is null");
            return;
        }

        if (IsNew == true)
        {
            var putResult = await CustomerClient.PutNewCustomerAsync(Customer, _cls.Token);
            if (putResult?.NewId is not null)
            {
                Customer.Id = putResult.NewId.Value;
                IsNew = false;
            }

            if (Refresh is not null)
                await Refresh.CallRequestRefreshAsync();
            MudDialog.Close();
        }
        else
        {
            var patchResult = await CustomerClient.PatchCustomerAsync(Customer, _cls.Token);
            if (patchResult?.Success == true)
                MudDialog.Close();
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}