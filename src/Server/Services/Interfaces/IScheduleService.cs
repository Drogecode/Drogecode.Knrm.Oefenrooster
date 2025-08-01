﻿using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IScheduleService : IDrogeService
{
    Task<MultipleTrainingsResponse> ScheduleForUserAsync(Guid userId, Guid customerId, int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, CancellationToken clt);
    Task<ScheduleForAllResponse> ScheduleForAllAsync(Guid userId, Guid customerId, int forMonth, int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, bool countPerUser, bool includeUnAssigned, CancellationToken clt);
    Task<GetTrainingByIdResponse> GetTrainingById(Guid userId, Guid customerId, Guid trainingId, CancellationToken clt);
    Task<GetDescriptionByTrainingIdResponse> GetDescriptionByTrainingId(Guid userId, Guid customerId, Guid trainingId, CancellationToken clt);
    Task<GetPlannedTrainingResponse> GetPlannedTrainingById(Guid customerId, Guid trainingId, CancellationToken clt);
    Task<GetPlannedTrainingResponse> GetPlannedTrainingForDefaultDate(Guid customerId, DateTime date, Guid defaultId, CancellationToken token);
    Task<PatchScheduleForUserResponse> PatchScheduleForUserAsync(Guid userId, Guid customerId, Training training, CancellationToken clt);
    Task<PatchAssignedUserResponse?> PatchAssignedUserAsync(Guid userId, Guid customerId, PatchAssignedUserRequest body, CancellationToken clt);
    Task<PatchTrainingResponse> PatchTraining(Guid customerId, PlannedTraining training, bool inRoleEditPast, CancellationToken clt);

    /// <summary>
    /// Patch last sync for availability.
    /// Requires calling IScheduleService.SaveDb();
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="userId"></param>
    /// <param name="availableId"></param>
    /// <param name="clt"></param>
    Task PatchLastSynced(Guid customerId, Guid userId, Guid availableId, CancellationToken clt);
    Task PatchAvailableLastChanged(Guid customerId, Guid currentUserId, PlanUser user, CancellationToken clt);
    Task<AddTrainingResponse> AddTrainingAsync(Guid customerId, PlannedTraining training, Guid trainingId, CancellationToken clt);
    Task<GetScheduledTrainingsForUserResponse> GetScheduledTrainingsForUser(Guid userId, Guid customerId, DateTime? fromDate, int take, int skip, OrderAscDesc order, CancellationToken clt);
    Task<GetUserMonthInfoResponse> GetUserMonthInfo(Guid userId, Guid customerId, CancellationToken clt);
    Task<GetPinnedTrainingsForUserResponse> GetPinnedTrainingsForUser(Guid userId, Guid customerId, DateTime fromDate, CancellationToken clt);
    Task<PutAssignedUserResponse> PutAssignedUserAsync(Guid userId, Guid customerId, OtherScheduleUserRequest body, CancellationToken clt);
    Task<bool> PatchEventIdForUserAvailible(Guid userId, Guid customerId, Guid? availableId, string? calendarEventId, CancellationToken clt);
    Task<List<DbRoosterAvailable>> GetTrainingsThatRequireCalendarUpdate(Guid userId, Guid customerId);
    Task<bool> DeleteTraining(Guid userId, Guid customerId, Guid trainingId, CancellationToken clt);
}
