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
    private List<Training>? _trainings;
    private List<PlannerTrainingType>? _trainingTypes;
    protected override async Task OnParametersSetAsync()
    {
        var dbUser = await _userRepository.GetCurrentUserAsync();//Force creation of user.
        _trainings = (await _scheduleRepository.GetScheduledTrainingsForUser(_cls.Token))?.Trainings;
        _trainingTypes = await _scheduleRepository.GetTrainingTypes(_cls.Token);
    }

    private string GetStyle(Training training)
    {
        var trainingType = (_trainingTypes?.FirstOrDefault(x => x.Id == training.RoosterTrainingTypeId)) ?? (_trainingTypes?.FirstOrDefault(x => x.IsDefault));
        return PlannerHelper.HeaderStyle(trainingType);
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
