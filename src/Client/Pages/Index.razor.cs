using Drogecode.Knrm.Oefenrooster.Client.Helpers;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages;
public sealed partial class Index : IDisposable
{
    [Inject] private IStringLocalizer<Index> L { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<Training>? _futureTrainings;
    private List<Training>? _pinnedTrainings;
    protected override async Task OnParametersSetAsync()
    {
        var dbUser = await _userRepository.GetCurrentUserAsync();//Force creation of user.
        _futureTrainings = (await _scheduleRepository.GetScheduledTrainingsForUser(_cls.Token))?.Trainings;
        _pinnedTrainings = (await _scheduleRepository.GetPinnedTrainingsForUser(_cls.Token))?.Trainings;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
