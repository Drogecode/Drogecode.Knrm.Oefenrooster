using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IScheduleService
{
    Task<MultipleTrainingsResponse> ScheduleForUserAsync(Guid userId, Guid customerId, int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, CancellationToken token);
    Task<ScheduleForAllResponse> ScheduleForAllAsync(Guid userId, Guid customerId, int forMonth, int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, CancellationToken token);
    Task<PatchScheduleForUserResponse> PatchScheduleForUserAsync(Guid userId, Guid customerId, Training training, CancellationToken token);
    Task<Guid?> PatchAssignedUserAsync(Guid userId, Guid customerId, PatchAssignedUserRequest body, CancellationToken token);
    Task<PatchTrainingResponse> PatchTraining(Guid customerId, EditTraining patchedTraining, CancellationToken token);
    Task<AddTrainingResponse> AddTrainingAsync(Guid customerId, EditTraining training, Guid trainingId, CancellationToken token);
    Task<GetScheduledTrainingsForUserResponse> GetScheduledTrainingsForUser(Guid userId, Guid customerId, DateTime? fromDate, CancellationToken token);
    Task<GetPinnedTrainingsForUserResponse> GetPinnedTrainingsForUser(Guid userId, Guid customerId, DateTime fromDate, CancellationToken token);
    Task PutAssignedUserAsync(Guid userId, Guid customerId, OtherScheduleUserRequest body, CancellationToken token);
}
