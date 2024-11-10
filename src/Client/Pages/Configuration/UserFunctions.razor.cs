using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Microsoft.AspNetCore.Components;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public partial class UserFunctions : IDisposable
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
            _functions = await FunctionClient.GetAll2Async(false, _cls.Token);
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}