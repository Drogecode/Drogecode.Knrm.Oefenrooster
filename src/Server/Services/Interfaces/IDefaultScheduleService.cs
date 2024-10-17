using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Microsoft.AspNetCore.Mvc;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IDefaultScheduleService
{
    Task<GetAllDefaultGroupsResponse> GetAllDefaultGroupsForUser(Guid customerId, Guid userId);
    Task<MultipleDefaultSchedulesResponse> GetAllDefaultsForUser(Guid customerId, Guid userId, Guid groupId);
    Task<PutGroupResponse> PutGroup(DefaultGroup body, Guid customerId, Guid userId);
    Task<PutDefaultScheduleResponse> PutDefaultSchedule(DefaultSchedule body, Guid customerId, Guid userId);
    Task<PatchDefaultScheduleResponse> PatchDefaultSchedule(DefaultSchedule body, Guid customerId, Guid userId);
    Task<GetAllDefaultScheduleResponse> GetAllDefaultSchedule(Guid customerId, Guid userId, CancellationToken clt);
    Task<PatchDefaultScheduleForUserResponse> PatchDefaultScheduleForUser(PatchDefaultUserSchedule body, Guid customerId, Guid userId);
}
