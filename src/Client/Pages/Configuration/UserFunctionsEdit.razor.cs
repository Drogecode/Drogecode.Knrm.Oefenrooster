using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class UserFunctionsEdit : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<UserFunctions>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private IFunctionClient? FunctionClient { get; set; }
    [Parameter] public Guid? Id { get; set; }
    private readonly CancellationTokenSource _cls = new();
    private GetFunctionResponse? _function;
    private bool? _saved = null;
    private bool _editName = false;
    private bool _isNew;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Id is not null)
            {
                _function = await FunctionClient.GetByIdAsync(Id.Value, _cls.Token);
                DebugHelper.WriteLine($"Loading function: {Id}");
            }
            else
            {
                DebugHelper.WriteLine("Creating new role");
                _editName = true;
                _isNew = true;
                _function = new GetFunctionResponse
                {
                    Function = new DrogeFunction()
                };
            }

            StateHasChanged();
        }
    }

    private async Task Submit()
    {
        if (_function?.Function is null)
        {
            _saved = false;
            return;
        }

        if (_isNew)
        {
            var putResponse = await FunctionClient.AddFunctionAsync(_function.Function, _cls.Token);
            if (putResponse.Success && putResponse.NewId is not null)
            {
                _function = await FunctionClient.GetByIdAsync(putResponse.NewId.Value, _cls.Token);
                _saved = true;
                _isNew = false;
            }
        }
        else
        {
            var patchResponse = await FunctionClient.PatchFunctionAsync(_function.Function, _cls.Token);
            _saved = patchResponse?.Success ?? false;
        }

        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}