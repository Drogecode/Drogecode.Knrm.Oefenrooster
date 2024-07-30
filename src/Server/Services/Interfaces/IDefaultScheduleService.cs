using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IDefaultScheduleService
{
    Task<GetAllDefaultGroupsResponse> GetAllDefaultGroupsForUser(Guid customerId, Guid userId);
    Task<MultipleDefaultSchedulesResponse> GetAllDefaultsForUser(Guid customerId, Guid userId, Guid groupId);
    Task<PutGroupResponse> PutGroup(DefaultGroup body, Guid customerId, Guid userId);
    Task<PutDefaultScheduleResponse> PutDefaultSchedule(DefaultSchedule body, Guid customerId, Guid userId);
    Task<PatchDefaultScheduleForUserResponse> PatchDefaultScheduleForUser(PatchDefaultUserSchedule body, Guid customerId, Guid userId);
}
