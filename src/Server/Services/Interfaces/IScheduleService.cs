using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IScheduleService
{
    Task<ScheduleForUserResponse> ScheduleForUserAsync(Guid userId, Guid customerId, int relativeWeek, CancellationToken token);
    Task<ScheduleForAllResponse> ScheduleForAllAsync(Guid userId, Guid customerId, int relativeWeek, CancellationToken token);
    Task<Training> PatchTrainingAsync(Guid userId, Guid customerId, Training training, CancellationToken token);
    Task PatchScheduleUserAsync(Guid userId, Guid customerId, PatchScheduleUserRequest body, CancellationToken token);
    Task<GetScheduledTrainingsForUserResponse> GetScheduledTrainingsForUser(Guid userId, Guid customerId, DateOnly fromDate, CancellationToken token);
    Task OtherScheduleUserAsync(Guid userId, Guid customerId, OtherScheduleUserRequest body, CancellationToken token);
}
