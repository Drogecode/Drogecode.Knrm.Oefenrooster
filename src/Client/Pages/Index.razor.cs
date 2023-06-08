using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages;
public sealed partial class Index : IDisposable
{
    [Inject] private IStringLocalizer<Index> L { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private SharePointRepository _sharePointRepository{ get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<Training>? _futureTrainings;
    private List<Training>? _pinnedTrainings;
    private List<SharePointTraining>? _sharePointTrainings;
    private List<SharePointAction>? _sharePointActions;
    protected override async Task OnParametersSetAsync()
    {
        var dbUser = await _userRepository.GetCurrentUserAsync();//Force creation of user.
        _futureTrainings = (await _scheduleRepository.GetScheduledTrainingsForUser(_cls.Token))?.Trainings;
        StateHasChanged();
        _pinnedTrainings = (await _scheduleRepository.GetPinnedTrainingsForUser(_cls.Token))?.Trainings;
        StateHasChanged();
        _sharePointActions = await _sharePointRepository.GetLastActionsForCurrentUser(_cls.Token);
        StateHasChanged();
        _sharePointTrainings = await _sharePointRepository.GetLastTrainingsForCurrentUser(_cls.Token);
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
