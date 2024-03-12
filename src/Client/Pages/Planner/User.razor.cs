using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;

public sealed partial class User : IDisposable
{
    [Inject] private IStringLocalizer<User> L { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private FunctionRepository _functionRepository { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    protected override async Task OnParametersSetAsync()
    {
        _users = await _userRepository.GetAllUsersAsync(false, false, _cls.Token);
        _functions = await _functionRepository.GetAllFunctionsAsync();
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
