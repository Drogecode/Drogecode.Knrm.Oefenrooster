using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages;
public sealed partial class Index : IDisposable
{
    [Inject] private IStringLocalizer<Index> L { get; set; } = default!;
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] UserRepository UserRepository { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private ClaimsPrincipal _user;
    private List<Training>? _trainings;
    private bool _isAuthenticated;
    private string _name = string.Empty;
    protected override async Task OnParametersSetAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        _user = authState.User;
        _isAuthenticated = authState.User?.Identity?.IsAuthenticated ?? false;
        if (_isAuthenticated)
        {
            var dbUser = await UserRepository.GetCurrentUserAsync();
            _name = authState!.User!.Identity!.Name ?? string.Empty;
            
        }
        _trainings = (await _scheduleRepository.GetScheduledTrainingsForUser(_cls.Token))?.Trainings;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
