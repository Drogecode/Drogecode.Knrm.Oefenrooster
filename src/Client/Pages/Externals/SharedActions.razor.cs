using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Externals;

public partial class SharedActions : IDisposable
{
    [Inject, NotNull] private UserRepository? UserRepository { get; set; }
    [Inject, NotNull] private FunctionRepository? FunctionRepository { get; set; }
    [Parameter] public Guid? Id { get; set; }
    private readonly CancellationTokenSource _cls = new();
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _users = await UserRepository.GetAllUsersAsync(false, false, false, _cls.Token);
        _functions = await FunctionRepository.GetAllFunctionsAsync(true, _cls.Token);
    }
    
    public void Dispose()
    {
        _cls.Cancel();
    }
}