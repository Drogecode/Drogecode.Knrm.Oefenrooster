using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;

public sealed partial class User : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<User>? L { get; set; }
    [Inject, NotNull] private UserRepository? UserRepository { get; set; }
    [Inject, NotNull] private FunctionRepository? FunctionRepository { get; set; }
    [Inject, NotNull] private NavigationManager? Navigation { get; set; }
    private readonly CancellationTokenSource _cls = new();
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    protected override async Task OnParametersSetAsync()
    {
        _users = await UserRepository.GetAllUsersAsync(false, false, false, _cls.Token);
        _functions = await FunctionRepository.GetAllFunctionsAsync(false, _cls.Token);
        StateHasChanged();
    }

    private void ClickUser(DrogeUser user)
    {
        Navigation.NavigateTo($"/planner/user/{user.Id}");
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
