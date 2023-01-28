using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IScheduleService
{
    Task<ScheduleForUserResponse> ScheduleForUserAsync(Guid userId, Guid customerId, int relativeWeek);
    Task<ScheduleForAllResponse> ScheduleForAllAsync(Guid userId, Guid customerId, int relativeWeek);
    Task<Training> PatchTrainingAsync(Guid userId, Guid customerId, Training training);
}
