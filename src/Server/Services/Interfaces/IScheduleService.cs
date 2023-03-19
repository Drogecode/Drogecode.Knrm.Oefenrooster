using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IScheduleService
{
    Task<ScheduleForUserResponse> ScheduleForUserAsync(Guid userId, Guid customerId, int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, CancellationToken token);
    Task<ScheduleForAllResponse> ScheduleForAllAsync(Guid userId, Guid customerId, int forMonth, int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, CancellationToken token);
    Task<Training> PatchScheduleForUserAsync(Guid userId, Guid customerId, Training training, CancellationToken token);
    Task PatchAvailabilityUserAsync(Guid userId, Guid customerId, PatchScheduleUserRequest body, CancellationToken token);
    Task<bool> PatchTraining(Guid customerId, EditTraining patchedTraining, CancellationToken token);
    Task<bool> AddTrainingAsync(Guid customerId, EditTraining training, Guid trainingId, CancellationToken token);
    Task<GetScheduledTrainingsForUserResponse> GetScheduledTrainingsForUser(Guid userId, Guid customerId, DateTime fromDate, CancellationToken token);
    Task OtherScheduleUserAsync(Guid userId, Guid customerId, OtherScheduleUserRequest body, CancellationToken token);
}
