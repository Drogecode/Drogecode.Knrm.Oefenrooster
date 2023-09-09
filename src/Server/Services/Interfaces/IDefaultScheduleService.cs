using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IDefaultScheduleService
{
    Task<List<DefaultSchedule>> GetAlldefaultsForUser(Guid customerId, Guid userId);
    Task<PutDefaultScheduleResponse> PutDefaultSchedule(DefaultSchedule body, Guid customerId, Guid userId);
    Task<PatchDefaultScheduleForUserResponse> PatchDefaultScheduleForUser(PatchDefaultUserSchedule body, Guid customerId, Guid userId);
}
