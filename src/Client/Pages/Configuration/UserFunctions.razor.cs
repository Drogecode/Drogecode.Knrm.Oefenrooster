using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class UserFunctions : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<UserFunctions>? L { get; set; }
    [Inject, NotNull] private IFunctionClient? FunctionClient { get; set; }
    [Inject, NotNull] private NavigationManager? Navigation { get; set; }
    private readonly CancellationTokenSource _cls = new();
    private MultipleFunctionsResponse? _functions;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _functions = await FunctionClient.GetAllAsync(false, _cls.Token);
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}