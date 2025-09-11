using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Repositories.Interfaces;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Extensions;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Diagnostics;
using Ganss.Xss;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ScheduleService : DrogeService, IScheduleService
{
    private readonly IRoosterDefaultsRepository _roosterDefaultsRepository;
    private readonly IUserDefaultAvailableRepository _userDefaultAvailableRepository;
    private readonly IUserHolidaysRepository _userHolidaysRepository;

    public ScheduleService(
        ILogger<ScheduleService> logger,
        DataContext database,
        IMemoryCache memoryCache,
        IDateTimeProvider dateTimeProvider,
        IRoosterDefaultsRepository roosterDefaultsRepository,
        IUserDefaultAvailableRepository userDefaultAvailableRepository,
        IUserHolidaysRepository userHolidaysRepository) : base(logger, database, memoryCache, dateTimeProvider)
    {
        _roosterDefaultsRepository = roosterDefaultsRepository;
        _userDefaultAvailableRepository = userDefaultAvailableRepository;
        _userHolidaysRepository = userHolidaysRepository;
    }

    public async Task<MultipleTrainingsResponse> ScheduleForUserAsync(Guid userId, Guid customerId, int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd,
        CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new MultipleTrainingsResponse();
        var startDate = (new DateTime(yearStart, monthStart, dayStart, 0, 0, 0)).ToUniversalTime();
        var tillDate = (new DateTime(yearEnd, monthEnd, dayEnd, 23, 59, 59, 999)).ToUniversalTime();
        var defaults = await _roosterDefaultsRepository.GetDefaultsForCustomerInSpan(true, customerId, startDate, tillDate, clt);

        // Get default availability for user, do not check cache.
        var defaultAveUser = await _userDefaultAvailableRepository.GetUserDefaultAvailableForCustomerInSpan(false, customerId, userId, startDate, tillDate, clt);
        var userHolidays = await _userHolidaysRepository.GetUserHolidaysForUser(false, customerId, userId, tillDate, startDate, clt);
        var trainings = Database.RoosterTrainings
            .AsNoTracking()
            .Include(x => x.LinkReportTrainingRoosterTrainings)
            .Include(x => x.TrainingTargetSet)
            .Where(x => x.CustomerId == customerId && x.DateStart >= startDate && x.DateStart <= tillDate)
            .OrderBy(x => x.DateStart);
        var availables = await Database.RoosterAvailables
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId && x.UserId == userId && x.Date >= startDate && x.Date <= tillDate)
            .AsSingleQuery()
            .ToListAsync(cancellationToken: clt);

        var scheduleDate = DateOnly.FromDateTime(startDate);
        var till = DateOnly.FromDateTime(tillDate);
        do
        {
            var defaultsFound = new List<Guid?>();
            var start = scheduleDate.ToDateTime(new TimeOnly(0, 0, 0), DateTimeKind.Utc);
            var end = scheduleDate.ToDateTime(new TimeOnly(23, 59, 59), DateTimeKind.Utc);
            var defaultsToday = defaults.Where(x => x.WeekDay == scheduleDate.DayOfWeek && x.ValidFrom <= start && x.ValidUntil >= end).ToList();
            var trainingsToday = trainings.Where(x => x.DateStart >= start && x.DateStart <= end).ToList();
            foreach (var training in trainingsToday)
            {
                if (training.DeletedOn is null)
                {
                    var ava = availables.FirstOrDefault(x => x.TrainingId == training.Id);
                    Availability? available = ava?.Available;
                    AvailabilitySetBy? setBy = ava?.SetBy;
                    GetAvailability(defaultAveUser, userHolidays, training.DateStart, training.DateEnd, training.RoosterDefaultId, null, ref available, ref setBy);

                    result.Trainings.Add(new Training
                    {
                        DefaultId = training.RoosterDefaultId,
                        TrainingTargetSetId = training.TrainingTargetSetId,
                        TrainingId = training.Id,
                        Name = training.Name,
                        DateStart = training.DateStart,
                        DateEnd = training.DateEnd,
                        Availability = available,
                        SetBy = setBy ?? AvailabilitySetBy.None,
                        Assigned = ava?.Assigned ?? false,
                        RoosterTrainingTypeId = training.RoosterTrainingTypeId,
                        CountToTrainingTarget = training.CountToTrainingTarget,
                        IsPinned = training.IsPinned,
                        IsPermanentPinned = training.IsPermanentPinned,
                        ShowTime = training.ShowTime ?? true,
                        HasDescription = training.Description?.IsHtmlOnlyWhitespaceOrBreaks() ?? false,
                        HasTargets = training.TrainingTargetSet?.TrainingTargetIds.Count > 0,
                        LinkedReports = training.LinkReportTrainingRoosterTrainings?.Count ?? 0
                    });
                }

                if (training.RoosterDefaultId != null)
                    defaultsFound.Add(training.RoosterDefaultId);
            }

            foreach (var def in defaultsToday)
            {
                if (!defaultsFound.Contains(def.Id))
                {
                    Availability? available = null;
                    AvailabilitySetBy? setBy = null;
                    var trainingStart = scheduleDate.ToDateTime(def.TimeStart).DateTimeWithZone(def.TimeZone);
                    var trainingEnd = scheduleDate.ToDateTime(def.TimeEnd).DateTimeWithZone(def.TimeZone);
                    GetAvailability(defaultAveUser, userHolidays, trainingStart, trainingEnd, def.Id, null, ref available, ref setBy);
                    result.Trainings.Add(new Training
                    {
                        DefaultId = def.Id,
                        Name = def.Name,
                        DateStart = trainingStart,
                        DateEnd = trainingEnd,
                        Availability = available ?? Availability.None,
                        SetBy = setBy ?? AvailabilitySetBy.None,
                        RoosterTrainingTypeId = def.RoosterTrainingTypeId,
                        CountToTrainingTarget = def.CountToTrainingTarget,
                        IsPinned = false,
                        IsPermanentPinned = false,
                        ShowTime = def.ShowTime ?? true,
                    });
                }
            }

            clt.ThrowIfCancellationRequested();
            scheduleDate = scheduleDate.AddDays(1);
        } while (scheduleDate <= till);

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }


    private static void GetAvailability(List<DbUserDefaultAvailable>? defaultAveUser, List<DbUserHolidays> userHolidays, DateTime start, DateTime end, Guid? roosterDefaultId, Guid? userId,
        ref Availability? available, ref AvailabilitySetBy? setBy)
    {
        if ((setBy != AvailabilitySetBy.User && setBy is null) || available is null || available == Availability.None)
        {
            var userHoliday = userHolidays.FirstOrDefault(x => x.ValidFrom <= start && x.ValidUntil >= end && (userId is null || x.UserId == userId));
            if (userHoliday?.Available != null)
            {
                available = userHoliday.Available;
                setBy = AvailabilitySetBy.Holiday;
            }
        }

        if ((setBy != AvailabilitySetBy.User && setBy != AvailabilitySetBy.Holiday && setBy is null) || available is null || available == Availability.None)
        {
            var defAvaForUser = defaultAveUser?.Where(x => x.RoosterDefaultId == roosterDefaultId && x.ValidFrom <= start && x.ValidUntil >= end && (userId is null || x.UserId == userId))
                .OrderBy(x => x.DefaultGroup?.IsDefault).FirstOrDefault();
            if (defAvaForUser?.Available != null)
            {
                available = defAvaForUser.Available;
                setBy = AvailabilitySetBy.DefaultAvailable;
            }
        }

        if (available is null || available == Availability.None)
        {
            setBy = AvailabilitySetBy.None;
        }
    }

    public async Task<PatchScheduleForUserResponse> PatchScheduleForUserAsync(Guid userId, Guid customerId, Training training, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        training.Updated = false;
        var result = new PatchScheduleForUserResponse();
        if (training.DateStart.CompareTo(DateTimeProvider.UtcNow()) >= 0)
        {
            result.PatchedTraining = training;
            var dbTraining = await CreateAndGetTraining(userId, customerId, training, clt);
            if (dbTraining is null) return result;
            var available = await Database.RoosterAvailables.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.UserId == userId && x.TrainingId == dbTraining.Id);
            if (available is null)
            {
                if (!await AddAvailableInternalAsync(userId, customerId, training)) return result;
            }
            else if (!await PatchAvailableInternalAsync(userId, available, training)) return result;

            training.Updated = true;
            result.Success = true;
            result.PatchedTraining = training;
            result.AvailableId = available?.Id;
            result.CalendarEventId = available?.CalendarEventId;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    private async Task<DbRoosterTraining?> CreateAndGetTraining(Guid userId, Guid customerId, TrainingAdvance? training, CancellationToken clt)
    {
        DbRoosterTraining? dbTraining = null;
        if (training == null) return dbTraining;
        if (training.TrainingId == null || training.TrainingId == Guid.Empty)
        {
            dbTraining = await Database.RoosterTrainings.FirstOrDefaultAsync(x =>
                x.CustomerId == customerId && x.RoosterDefaultId == training.DefaultId && x.DateStart == training.DateStart && x.DateEnd == training.DateEnd, cancellationToken: clt);
            if (dbTraining == null)
            {
                training.TrainingId = Guid.NewGuid();
                if (!await AddTrainingInternalAsync(customerId, training, clt)) return dbTraining;
            }
            else
                training.TrainingId = dbTraining.Id;
        }

        clt = CancellationToken.None;
        if (dbTraining == null)
            dbTraining = await Database.RoosterTrainings.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Id == training.TrainingId);
        if (dbTraining == null)
        {
            if (!await AddTrainingInternalAsync(customerId, training, clt)) return dbTraining;
            dbTraining = await Database.RoosterTrainings.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Id == training.TrainingId);
            if (dbTraining == null)
            {
                Logger.LogWarning("Two tries to add training {date} for user {userId} from customer {customerId} failed", training.DateStart, userId, customerId);
                return dbTraining;
            }
        }

        return dbTraining;
    }

    public async Task<PatchTrainingResponse> PatchTraining(Guid customerId, PlannedTraining patchedTraining, bool inRoleEditPast, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new PatchTrainingResponse();
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedAttributes.Add("data-list");
        var oldTraining = await Database.RoosterTrainings
            .Include(x => x.RoosterAvailables)
            .Where(x => x.Id == patchedTraining.TrainingId)
            .FirstOrDefaultAsync(clt);
        if (oldTraining == null) return result;
        if (patchedTraining.Name?.Length > DefaultSettingsHelper.MAX_LENGTH_TRAINING_TITLE)
            throw new DrogeCodeToLongException();
        if (!inRoleEditPast && oldTraining.DateEnd < DateTimeProvider.UtcNow().AddDays(AccessesSettings.AUTH_scheduler_edit_past_days - 1))
            throw new UnauthorizedAccessException();

        oldTraining.RoosterTrainingTypeId = patchedTraining.RoosterTrainingTypeId;
        oldTraining.TrainingTargetSetId = patchedTraining.TrainingTargetSetId;
        oldTraining.Name = sanitizer.Sanitize(patchedTraining.Name ?? string.Empty);
        oldTraining.Description = sanitizer.Sanitize(patchedTraining.Description ?? string.Empty);
        oldTraining.DateStart = patchedTraining.DateStart;
        oldTraining.DateEnd = patchedTraining.DateEnd;
        oldTraining.CountToTrainingTarget = patchedTraining.CountToTrainingTarget;
        oldTraining.IsPinned = patchedTraining.IsPinned;
        oldTraining.IsPermanentPinned = patchedTraining.IsPermanentPinned;
        oldTraining.ShowTime = patchedTraining.ShowTime;
        if (oldTraining.RoosterAvailables is not null)
        {
            foreach (var available in oldTraining.RoosterAvailables)
            {
                available.Date = patchedTraining.DateStart;
            }
        }

        Database.RoosterTrainings.Update(oldTraining);
        result.Success = (await Database.SaveChangesAsync(clt)) > 0;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    /// <inheritdoc />
    public async Task PatchLastSynced(Guid customerId, Guid userId, Guid? availableId, CancellationToken clt)
    {
        var oldAva = await Database.RoosterAvailables
            .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.UserId == userId && x.Id == availableId, clt);
        if (oldAva == null)
        {
            Logger.LogWarning("No rooster available found `{customerId}` for user `{userId}` with availableId `{availableId}`", customerId, userId, availableId);
            return;
        }

        oldAva.LastSyncOn = DateTimeProvider.UtcNow();
        Database.RoosterAvailables.Update(oldAva);
    }

    /// <inheritdoc />
    public async Task PatchAvailableLastChanged(Guid customerId, Guid currentUserId, PlanUser user, CancellationToken clt)
    {
        var oldAva = await Database.RoosterAvailables
            .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.UserId == user.UserId && x.Id == user.AvailableId, clt);
        if (oldAva == null)
        {
            Logger.LogWarning("No rooster available found `{customerId}` for user `{userId}` with availableId `{availableId}`", customerId, user.UserId, user.AvailableId);
            return;
        }

        oldAva.LastUpdateOn = DateTimeProvider.UtcNow();
        oldAva.LastUpdateBy = currentUserId;
        Database.RoosterAvailables.Update(oldAva);
    }

    public async Task<AddTrainingResponse> AddTrainingAsync(Guid customerId, PlannedTraining newTraining, Guid trainingId, CancellationToken token)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new AddTrainingResponse
        {
            NewId = trainingId
        };
        var sanitizer = new HtmlSanitizer();
        var training = new Training
        {
            TrainingId = trainingId,
            DefaultId = newTraining.DefaultId,
            RoosterTrainingTypeId = newTraining.RoosterTrainingTypeId,
            TrainingTargetSetId = newTraining.TrainingTargetSetId,
            Name = sanitizer.Sanitize(newTraining.Name ?? string.Empty),
            Description = sanitizer.Sanitize(newTraining.Description ?? string.Empty),
            DateStart = newTraining.DateStart,
            DateEnd = newTraining.DateEnd,
            CountToTrainingTarget = newTraining.CountToTrainingTarget,
            IsPinned = newTraining.IsPinned,
            IsPermanentPinned = newTraining.IsPermanentPinned,
            ShowTime = newTraining.ShowTime,
        };
        result.Success = await AddTrainingInternalAsync(customerId, training, token);
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    private async Task<bool> AddTrainingInternalAsync(Guid customerId, TrainingAdvance training, CancellationToken clt)
    {
        if (training.Name?.Length > DefaultSettingsHelper.MAX_LENGTH_TRAINING_TITLE)
            throw new DrogeCodeToLongException();
        clt.ThrowIfCancellationRequested();
        clt = CancellationToken.None;
        await Database.RoosterTrainings.AddAsync(new DbRoosterTraining
        {
            Id = training.TrainingId ?? throw new NoNullAllowedException("TrainingId is still null while adding new training"),
            RoosterDefaultId = training.DefaultId,
            TrainingTargetSetId = training.TrainingTargetSetId,
            CustomerId = customerId,
            RoosterTrainingTypeId = training.RoosterTrainingTypeId,
            Name = training.Name,
            Description = training.Description,
            DateStart = training.DateStart,
            DateEnd = training.DateEnd,
            CountToTrainingTarget = training.CountToTrainingTarget,
            IsPinned = training.IsPinned,
            IsPermanentPinned = training.IsPermanentPinned,
            ShowTime = training.ShowTime
        }, clt);
        if (training.DefaultId is not null && training.DefaultId != Guid.Empty)
        {
            var roosterDefault = await Database.RoosterDefaults.FirstOrDefaultAsync(x => x.Id == training.DefaultId);
            if (roosterDefault?.VehicleIds is not null)
            {
                foreach (var vehicleFromRoosterDefault in roosterDefault.VehicleIds)
                {
                    Database.LinkVehicleTraining.Add(new DbLinkVehicleTraining
                    {
                        Id = Guid.NewGuid(),
                        RoosterTrainingId = training.TrainingId.Value,
                        VehicleId = vehicleFromRoosterDefault,
                        CustomerId = customerId,
                        IsSelected = true,
                    });
                }

                var defaultSelectedVehicles = await Database.Vehicles.Where(x => x.IsDefault).Select(x => x.Id).ToListAsync(clt);

                foreach (var defVehicle in defaultSelectedVehicles)
                {
                    if (roosterDefault.VehicleIds.Any(x => x == defVehicle)) continue;
                    Database.LinkVehicleTraining.Add(new DbLinkVehicleTraining
                    {
                        Id = Guid.NewGuid(),
                        RoosterTrainingId = training.TrainingId.Value,
                        VehicleId = defVehicle,
                        CustomerId = customerId,
                        IsSelected = false,
                    });
                }
            }
        }

        return (await Database.SaveChangesAsync()) > 0;
    }

    private async Task<bool> AddAvailableInternalAsync(Guid userId, Guid customerId, Training training)
    {
        await Database.RoosterAvailables.AddAsync(new DbRoosterAvailable
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CustomerId = customerId,
            TrainingId = training.TrainingId ?? throw new NoNullAllowedException("TrainingId is still null while adding available"),
            Date = training.DateStart,
            Available = training.Availability,
            SetBy = training.SetBy,
            LastUpdateOn = DateTimeProvider.UtcNow(),
            LastUpdateBy = userId
        });
        return (await Database.SaveChangesAsync()) > 0;
    }

    private async Task<bool> PatchAvailableInternalAsync(Guid userId, DbRoosterAvailable available, Training training)
    {
        available.Available = training.Availability;
        available.Date = training.DateStart;
        available.SetBy = training.SetBy;
        available.LastUpdateOn = DateTimeProvider.UtcNow();
        available.LastUpdateBy = userId;
        Database.RoosterAvailables.Update(available);
        return (await Database.SaveChangesAsync()) > 0;
    }

    public async Task<ScheduleForAllResponse> ScheduleForAllAsync(Guid userId, Guid customerId, int forMonth, int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd,
        bool countPerUser, bool includeUnAssigned, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new ScheduleForAllResponse();
        var startDate = (new DateTime(yearStart, monthStart, dayStart, 0, 0, 0)).ToUniversalTime();
        var tillDate = (new DateTime(yearEnd, monthEnd, dayEnd, 23, 59, 59, 999)).ToUniversalTime();
        var users = await Database.Users
            .AsNoTracking()
            .Include(x => x.UserDefaultAvailables)
            .Include(x => x.UserFunction)
            .Include(x => x.LinkedUserAsA.Where(x => x.DeletedOn == null))
            .Where(x => x.CustomerId == customerId && x.DeletedOn == null && x.UserFunction!.IsActive)
            .Select(x => new { x.Id, x.UserDefaultAvailables, x.UserFunctionId, x.Name, x.LinkedUserAsA, x.ExternalId })
            .AsSingleQuery().ToListAsync(cancellationToken: clt);
        var defaults = await _roosterDefaultsRepository.GetDefaultsForCustomerInSpan(true, customerId, startDate, tillDate, clt);
        var defaultAveUser = await _userDefaultAvailableRepository.GetUserDefaultAvailableForCustomerInSpan(true, customerId, null, startDate, tillDate, clt);
        var userHolidays = await _userHolidaysRepository.GetUserHolidaysForUser(true, customerId, null, tillDate, startDate, clt);
        var trainings = await Database.RoosterTrainings
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId && x.DateStart >= startDate && x.DateStart <= tillDate)
            .Include(x => x.LinkVehicleTrainings)
            .Include(x => x.LinkReportTrainingRoosterTrainings)
            .Include(x => x.TrainingTargetSet)
            .OrderBy(x => x.DateStart)
            .AsSingleQuery().ToListAsync(clt);
        var availables = await Database.RoosterAvailables
            .AsNoTracking()
            .Include(x => x.User)
            .ThenInclude(x => x.LinkedUserA)
            .Include(dbRoosterAvailable => dbRoosterAvailable.User)
            .ThenInclude(dbUsers => dbUsers.LinkedUserAsA.Where(x => x.DeletedOn == null))
            .Where(x => x.CustomerId == customerId && (includeUnAssigned || x.Assigned) && x.Date >= startDate && x.Date <= tillDate)
            .AsSingleQuery().ToListAsync(clt);

        var scheduleDate = DateOnly.FromDateTime(startDate);
        do
        {
            var defaultsFound = new List<Guid?>();
            var start = scheduleDate.ToDateTime(new TimeOnly(0, 0, 0, 0), DateTimeKind.Utc);
            var end = scheduleDate.ToDateTime(new TimeOnly(23, 59, 59, 999), DateTimeKind.Utc);
            var defaultsToday = defaults.Where(x => x.WeekDay == scheduleDate.DayOfWeek && x.ValidFrom <= start && x.ValidUntil >= end).ToList();
            var trainingsToday = trainings.Where(x => x.DateStart >= start && x.DateStart <= end).ToList();
            if (trainingsToday.Count > 0)
            {
                foreach (var training in trainingsToday)
                {
                    if (training.DeletedOn is null)
                    {
                        Guid? defVehicle = null;
                        var ava = availables.FindAll(x => x.TrainingId == training.Id);
                        var newPlanner = new PlannedTraining
                        {
                            DefaultId = training.RoosterDefaultId,
                            TrainingTargetSetId = training.TrainingTargetSetId,
                            TrainingId = training.Id,
                            Name = training.Name,
                            DateStart = training.DateStart,
                            DateEnd = training.DateEnd,
                            IsCreated = true,
                            RoosterTrainingTypeId = training.RoosterTrainingTypeId,
                            CountToTrainingTarget = training.CountToTrainingTarget,
                            IsPinned = training.IsPinned,
                            IsPermanentPinned = training.IsPermanentPinned,
                            ShowTime = training.ShowTime ?? true,
                            HasDescription = !training.Description?.IsHtmlOnlyWhitespaceOrBreaks() ?? false,
                            LinkedReports = training.LinkReportTrainingRoosterTrainings?.Count ?? 0
                        };
                        foreach (var user in users)
                        {
                            var avaUser = ava.FirstOrDefault(x => x.UserId == user.Id && (includeUnAssigned || x.Assigned));
                            if (avaUser is null && !includeUnAssigned) continue;
                            avaUser ??= new DbRoosterAvailable();
                            defVehicle ??= await GetDefaultVehicleForTraining(customerId, training, clt);
                            if (avaUser.VehicleId is null ||
                                (avaUser.VehicleId != defVehicle && !(training.LinkVehicleTrainings?.Any(x => x.IsSelected && x.VehicleId == avaUser.VehicleId) ?? false)))
                            {
                                /*if (avaUser is { Assigned: true, VehicleId: not null } && avaUser.UserId != Guid.Empty && avaUser.VehicleId != defVehicle &&
                                    !await IsVehicleSelectedForTraining(customerId, training.Id, avaUser.VehicleId, clt))
                                {
                                    // Fix on the run.
                                    avaUser.VehicleId = null;
                                    Database.RoosterAvailables.Update(avaUser);
                                }*/

                                avaUser.VehicleId = defVehicle;
                            }

                            Availability? available = avaUser.Available;
                            AvailabilitySetBy? setBy = avaUser.SetBy;
                            GetAvailability(defaultAveUser, userHolidays, training.DateStart, training.DateEnd, training.RoosterDefaultId, user.Id, ref available, ref setBy);

                            newPlanner.PlanUsers.Add(new PlanUser
                            {
                                UserId = user.Id,
                                AvailableId = avaUser.Id,
                                Availability = available,
                                SetBy = setBy ?? AvailabilitySetBy.None,
                                Assigned = avaUser.Assigned,
                                Name = user.Name,
                                PlannedFunctionId = avaUser.UserFunctionId ?? user.UserFunctionId,
                                UserFunctionId = user.UserFunctionId,
                                VehicleId = avaUser.VehicleId,
                                Buddy = avaUser.User?.LinkedUserAsA?.FirstOrDefault(x => x is { LinkType: UserUserLinkType.Buddy, DeletedOn: null })?.UserB?.Name,
                            });
                            if (countPerUser && training.CountToTrainingTarget && scheduleDate.Month == forMonth && avaUser.Assigned)
                            {
                                var indexUser = result.UserTrainingCounters.FindIndex(x => x.UserId.Equals(avaUser.UserId));
                                if (indexUser >= 0)
                                    result.UserTrainingCounters[indexUser].Count++;
                                else
                                {
                                    result.UserTrainingCounters.Add(new UserTrainingCounter
                                    {
                                        UserId = avaUser.UserId,
                                        Count = 1
                                    });
                                }
                            }
                        }

                        result.Planners.Add(newPlanner);
                    }

                    defaultsFound.Add(training.RoosterDefaultId);
                }
            }

            foreach (var def in defaultsToday)
            {
                if (!defaultsFound.Contains(def.Id))
                {
                    var trainingStart = scheduleDate.ToDateTime(def.TimeStart).DateTimeWithZone(def.TimeZone);
                    var trainingEnd = scheduleDate.ToDateTime(def.TimeEnd).DateTimeWithZone(def.TimeZone);
                    var newPlanner = new PlannedTraining
                    {
                        DefaultId = def.Id,
                        Name = def.Name,
                        DateStart = trainingStart,
                        DateEnd = trainingEnd,
                        IsCreated = false,
                        RoosterTrainingTypeId = def.RoosterTrainingTypeId,
                        CountToTrainingTarget = def.CountToTrainingTarget,
                        IsPinned = false,
                        IsPermanentPinned = false,
                        ShowTime = def.ShowTime ?? true,
                    };
                    if (includeUnAssigned)
                    {
                        foreach (var user in users)
                        {
                            Availability? available = null;
                            AvailabilitySetBy? setBy = null;
                            GetAvailability(defaultAveUser, userHolidays, trainingStart, trainingEnd, def.Id, user.Id, ref available, ref setBy);
                            newPlanner.PlanUsers.Add(new PlanUser
                            {
                                UserId = user.Id,
                                Availability = available,
                                SetBy = setBy ?? AvailabilitySetBy.None,
                                Assigned = false,
                                Name = user.Name,
                                PlannedFunctionId = user.UserFunctionId,
                                UserFunctionId = user.UserFunctionId,
                                Buddy = user.LinkedUserAsA?.FirstOrDefault(x => x is { LinkType: UserUserLinkType.Buddy, DeletedOn: null })?.UserB?.Name,
                            });
                        }
                    }

                    result.Planners.Add(newPlanner);
                }
            }

            clt.ThrowIfCancellationRequested();
            scheduleDate = scheduleDate.AddDays(1);
        } while (scheduleDate <= DateOnly.FromDateTime(tillDate));

        _ = await Database.SaveChangesAsync(clt);
        result.Success = true;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    private async Task<Guid?> GetDefaultVehicleForTraining(Guid? customerId, DbRoosterTraining training, CancellationToken clt)
    {
        var cacheKey = $"DefVehTr-{customerId}{training.Id}";
        MemoryCache.TryGetValue(cacheKey, out Guid? result);
        if (result is not null)
            return result.Value;
        var vehicles = await InternalGetVehiclesCached(customerId, clt);
        DbVehicles? vehiclePrev = null;
        foreach (var vehicle in vehicles)
        {
            if (training.LinkVehicleTrainings?.Any(x => x.VehicleId == vehicle.Id) == true)
            {
                var veh = training.LinkVehicleTrainings.FirstOrDefault(x => x.VehicleId == vehicle.Id);
                if (!veh!.IsSelected) continue;
                if (vehicle.IsDefault)
                {
                    vehiclePrev = vehicle;
                    break;
                }

                vehiclePrev ??= vehicle;
            }
            else if (vehicle.IsDefault)
            {
                vehiclePrev = vehicle;
                break;
            }
        }

        var cacheOptions = new MemoryCacheEntryOptions();
        cacheOptions.SetSlidingExpiration(TimeSpan.FromSeconds(5));
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(15));
        MemoryCache.Set(cacheKey, vehiclePrev?.Id, cacheOptions);
        return vehiclePrev?.Id;
    }

    private async Task<List<DbVehicles>> InternalGetVehiclesCached(Guid? customerId, CancellationToken clt)
    {
        var cacheKey = $"VehsFoCus-{customerId}";
        MemoryCache.TryGetValue(cacheKey, out List<DbVehicles>? result);
        if (result is not null) return result;
        result = await Database.Vehicles.Where(x => x.CustomerId == customerId).OrderBy(x => x.Order).ToListAsync(clt);
        var cacheOptions = new MemoryCacheEntryOptions();
        cacheOptions.SetSlidingExpiration(TimeSpan.FromSeconds(30));
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        MemoryCache.Set(cacheKey, result, cacheOptions);
        return result;
    }

    public async Task<GetTrainingByIdResponse> GetTrainingById(Guid userId, Guid customerId, Guid trainingId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new GetTrainingByIdResponse();
        var dbTraining = await Database.RoosterTrainings
            .Include(x => x.RoosterAvailables)
            .Include(x => x.TrainingTargetSet)
            .FirstOrDefaultAsync(x => x.Id == trainingId && x.DeletedOn == null, clt);
        if (dbTraining is not null)
        {
            result.Training = dbTraining.ToTraining(userId);
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<GetDescriptionByTrainingIdResponse> GetDescriptionByTrainingId(Guid userId, Guid customerId, Guid trainingId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new GetDescriptionByTrainingIdResponse();
        var description = await Database.RoosterTrainings.Where(x => x.Id == trainingId && x.DeletedOn == null)
            .Select(x => x.Description)
            .FirstOrDefaultAsync(clt);
        if (!string.IsNullOrEmpty(description))
        {
            result.Description = description;
            result.Success = true;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<GetPlannedTrainingResponse> GetPlannedTrainingById(Guid customerId, Guid trainingId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new GetPlannedTrainingResponse();
        var dbTraining = await Database.RoosterTrainings
            .Include(x => x.RoosterAvailables!.Where(y => y.User!.DeletedOn == null))
            .ThenInclude(x => x.User)
            .ThenInclude(x => x!.LinkedUserAsA!.Where(y => y.DeletedOn == null))
            .ThenInclude(x => x.UserB)
            .Include(x => x.RoosterTrainingType)
            .Include(x => x.RoosterAvailables!.Where(y => y.User!.DeletedOn == null))
            .Include(x => x.TrainingTargetSet)
            .Include(x => x.LinkVehicleTrainings!)
            .ThenInclude(x => x.Vehicles)
            .AsSingleQuery()
            .FirstOrDefaultAsync(x => x.Id == trainingId && x.DeletedOn == null, clt);
        if (dbTraining is not null)
        {
            result = await dDbTrainingToGetPlannedTrainingResponse(result, dbTraining, clt);

            var ava = await Database.UserDefaultAvailables.Include(x => x.DefaultGroup).Where(x => x.CustomerId == customerId && x.RoosterDefaultId == dbTraining.RoosterDefaultId)
                .ToListAsync(cancellationToken: clt);
            var userHolidays = await Database.UserHolidays.Where(x => x.CustomerId == customerId && x.ValidFrom <= dbTraining.DateEnd.AddDays(1) && x.ValidUntil >= dbTraining.DateStart.AddDays(-1))
                .ToListAsync(cancellationToken: clt);
            var availables = Database.RoosterAvailables.Where(x => x.CustomerId == customerId && x.TrainingId == trainingId);
            var users = await Database.Users
                .Include(x => x.UserFunction)
                .Include(x => x.LinkedUserAsA.Where(x => x.DeletedOn == null))
                .ThenInclude(x => x.UserB)
                .Where(x => x.CustomerId == customerId && x.DeletedOn == null && x.UserFunction!.IsActive)
                .ToListAsync(cancellationToken: clt);

            foreach (var user in users)
            {
                var avaUser = await availables.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken: clt);
                Availability? available = avaUser?.Available;
                AvailabilitySetBy? setBy = avaUser?.SetBy;
                GetAvailability(ava, userHolidays, dbTraining.DateStart, dbTraining.DateEnd, dbTraining.RoosterDefaultId, user.Id, ref available, ref setBy);
                if (result.Training!.PlanUsers.Any(x => x.UserId == user.Id))
                {
                    var d = result.Training.PlanUsers.FirstOrDefault(x => x.UserId == user.Id);
                    d!.Availability = available;
                    d.SetBy = setBy ?? AvailabilitySetBy.None;
                    d.Name = user.Name;
                    d.UserFunctionId = user.UserFunctionId;
                    d.PlannedFunctionId = avaUser?.UserFunctionId ?? user.UserFunctionId;
                    d.Buddy = user.LinkedUserAsA?.FirstOrDefault(x => x is { LinkType: UserUserLinkType.Buddy, DeletedOn: null })?.UserB.Name;
                }
                else if (!(available is null || setBy is null || setBy == AvailabilitySetBy.None))
                {
                    result.Training!.PlanUsers.Add(new PlanUser
                    {
                        AvailableId = avaUser?.Id,
                        Availability = available,
                        SetBy = setBy.Value,
                        Name = user.Name,
                        UserId = user.Id,
                        Buddy = user.LinkedUserAsA?.FirstOrDefault(x => x is { LinkType: UserUserLinkType.Buddy, DeletedOn: null })?.UserB.Name,
                        UserFunctionId = user.UserFunctionId,
                        PlannedFunctionId = avaUser?.UserFunctionId ?? user.UserFunctionId,
                    });
                }
            }
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<GetPlannedTrainingResponse> GetPlannedTrainingForDefaultDate(Guid customerId, DateTime date, Guid defaultId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        var result = new GetPlannedTrainingResponse();
        var dbTraining = await Database.RoosterTrainings
            .Include(x => x.RoosterAvailables)
            .Include(x => x.TrainingTargetSet)
            .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.DateStart.Date == date.Date && x.RoosterDefaultId == defaultId);
        if (dbTraining is not null)
            result = await dDbTrainingToGetPlannedTrainingResponse(result, dbTraining, clt);
        else
        {
            var def = await Database.RoosterDefaults.FirstOrDefaultAsync(x => x.Id == defaultId && x.WeekDay == date.DayOfWeek);
            if (def is not null)
            {
                var trainingStart = date.Date.AddHours(def.TimeStart.Hour).AddMinutes(def.TimeStart.Minute).AddSeconds(def.TimeStart.Second);
                var trainingEnd = date.Date.AddHours(def.TimeEnd.Hour).AddMinutes(def.TimeEnd.Minute).AddSeconds(def.TimeEnd.Second);

                var users = await Database.Users
                    .Include(x => x.UserDefaultAvailables)
                    .Include(x => x.UserFunction)
                    .Include(x => x.LinkedUserAsA!.Where(y => y.DeletedOn == null))
                    .ThenInclude(x => x.UserB)
                    .Where(x => x.CustomerId == customerId && x.DeletedOn == null && x.UserFunction!.IsActive)
                    .ToListAsync(cancellationToken: clt);
                var defaultAveUser = await Database.UserDefaultAvailables.Include(x => x.DefaultGroup)
                    .Where(x => x.CustomerId == customerId && x.ValidFrom <= trainingStart && x.ValidUntil >= trainingEnd).ToListAsync(cancellationToken: clt);
                var userHolidays = await Database.UserHolidays.Where(x => x.CustomerId == customerId && x.ValidFrom <= trainingStart && x.ValidUntil >= trainingEnd)
                    .ToListAsync(cancellationToken: clt);

                var newPlanner = new PlannedTraining
                {
                    DefaultId = def.Id,
                    Name = def.Name,
                    DateStart = trainingStart,
                    DateEnd = trainingEnd,
                    IsCreated = false,
                    RoosterTrainingTypeId = def.RoosterTrainingTypeId,
                    CountToTrainingTarget = def.CountToTrainingTarget,
                    IsPinned = false,
                    IsPermanentPinned = false,
                    ShowTime = def.ShowTime ?? true,
                };
                foreach (var dbUser in users)
                {
                    Availability? available = null;
                    AvailabilitySetBy? setBy = null;
                    GetAvailability(defaultAveUser, userHolidays, trainingStart, trainingEnd, def.Id, dbUser.Id, ref available, ref setBy);
                    newPlanner.PlanUsers.Add(new PlanUser
                    {
                        UserId = dbUser.Id,
                        Availability = available,
                        SetBy = setBy ?? AvailabilitySetBy.None,
                        Assigned = false,
                        Name = dbUser.Name,
                        PlannedFunctionId = dbUser.UserFunctionId,
                        UserFunctionId = dbUser.UserFunctionId,
                        Buddy = dbUser.LinkedUserAsA?.FirstOrDefault(x => x is { LinkType: UserUserLinkType.Buddy, DeletedOn: null })?.UserB.Name
                    });
                }

                result.Training = newPlanner;
                result.Success = true;
            }
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    private async Task<GetPlannedTrainingResponse> dDbTrainingToGetPlannedTrainingResponse(GetPlannedTrainingResponse result, DbRoosterTraining? dbTraining, CancellationToken clt)
    {
        if (dbTraining is not null)
        {
            var training = dbTraining?.ToPlannedTraining();
            if (training is not null)
            {
                var defaultVehicle = await GetDefaultVehicleForTraining(dbTraining!.CustomerId, dbTraining, clt);
                foreach (var user in training.PlanUsers)
                {
                    user.VehicleId ??= defaultVehicle;
                }

                result.Training = training;
                result.Success = true;
            }
        }

        return result;
    }

    public async Task<PatchAssignedUserResponse?> PatchAssignedUserAsync(Guid userId, Guid customerId, PatchAssignedUserRequest body, CancellationToken clt)
    {
        var result = new PatchAssignedUserResponse
        {
            IdPatched = body.TrainingId
        };
        if (body.User == null)
        {
            Logger.LogWarning("user is null {UserIsNull}", body.User == null);
            return null;
        }

        if (body.TrainingId is null)
        {
            var training = await CreateAndGetTraining(userId, customerId, body.Training, clt);
            if (training is not null)
            {
                body.TrainingId = training.Id;
                body.Training!.TrainingId = training.Id;
                result.IdPatched = training.Id;
            }
            else
            {
                Logger.LogWarning("Unable to find or create training");
                return null;
            }
        }

        if (!await Database.RoosterTrainings.AnyAsync(x => x.CustomerId == customerId && x.Id == body.TrainingId, cancellationToken: clt))
        {
            Logger.LogWarning("No training with '{Id}' found", body.TrainingId);
            return result;
        }

        var ava = await Database.RoosterAvailables.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.TrainingId == body.TrainingId && x.UserId == body.User.UserId, cancellationToken: clt);
        if (ava is null)
        {
            await PutAssignedUserAsync(userId, customerId, new OtherScheduleUserRequest
            {
                Assigned = body.User.Assigned,
                FunctionId = null,
                TrainingId = body.TrainingId,
                UserId = body.User.UserId,
                Training = body.Training,
            }, clt);
            ava = await Database.RoosterAvailables.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.TrainingId == body.TrainingId && x.UserId == body.User.UserId, cancellationToken: clt);
            if (ava is null)
            {
                Logger.LogWarning("No RoosterAvailable with '{Id}' found for user '{User}' when Assigned = '{Assigned}'", body.TrainingId, body.User.UserId, body.User.Assigned);
                return result;
            }
        }

        ava.Assigned = body.User.Assigned;
        ava.UserFunctionId = body.User.PlannedFunctionId;
        ava.VehicleId = body.User.VehicleId;
        ava.LastUpdateBy = userId;
        ava.LastUpdateOn = DateTimeProvider.UtcNow();
        clt.ThrowIfCancellationRequested();
        clt = CancellationToken.None;
        Database.RoosterAvailables.Update(ava);
        await Database.SaveChangesAsync(clt);
        result.Success = true;
        result.AvailableId = ava.Id;
        result.CalendarEventId = ava.CalendarEventId;
        result.Availability = ava.Available;
        result.SetBy = ava.SetBy;
        return result;
    }

    public async Task<PutAssignedUserResponse> PutAssignedUserAsync(Guid userId, Guid customerId, OtherScheduleUserRequest body, CancellationToken clt)
    {
        var result = new PutAssignedUserResponse
        {
            IdPut = body.TrainingId
        };
        if (body.UserId == null || body.TrainingId == null)
        {
            Logger.LogWarning("userId is null {UserIsNull} or trainingId is null {TrainingIsNull}", body.UserId == null, body.TrainingId == null);
            return result;
        }

        var user = await Database.Users.FirstOrDefaultAsync(x => x.Id == body.UserId && x.DeletedOn == null && x.CustomerId == customerId, cancellationToken: clt);
        if (user is null)
        {
            Logger.LogWarning("No user with '{Id}' found", body.UserId);
            return result;
        }

        body.FunctionId ??= user.UserFunctionId;

        var training = await CreateAndGetTraining(userId, customerId, body.Training, clt);
        if (training is null)
        {
            Logger.LogWarning("No training with '{Id}' found", body.TrainingId);
            return result;
        }

        result.IdPut = training.Id;
        var ava = await Database.RoosterAvailables.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.TrainingId == body.TrainingId && x.UserId == body.UserId, cancellationToken: clt);
        if (ava is null)
        {
            if (body.Assigned) // only add to db if assigned.
            {
                ava = new DbRoosterAvailable
                {
                    Id = Guid.NewGuid(),
                    UserId = body.UserId ?? Guid.Empty,
                    CustomerId = customerId,
                    TrainingId = body.TrainingId ?? Guid.Empty,
                    UserFunctionId = body.FunctionId,
                    Date = training.DateStart,
                    Available = Availability.None,
                    SetBy = AvailabilitySetBy.None,
                    Assigned = body.Assigned,
                    LastUpdateBy = userId,
                    LastUpdateOn = DateTimeProvider.UtcNow()
                };
                Database.RoosterAvailables.Add(ava);
                await Database.SaveChangesAsync(clt);
            }
        }
        else
        {
            ava.Assigned = body.Assigned;
            ava.LastUpdateBy = userId;
            ava.LastUpdateOn = DateTimeProvider.UtcNow();
            if (body.Assigned)
                ava.UserFunctionId = body.FunctionId;
            else
                ava.UserFunctionId = null;
            Database.RoosterAvailables.Update(ava);
            await Database.SaveChangesAsync(clt);
        }

        result.AvailableId = ava?.Id;
        result.CalendarEventId = ava?.CalendarEventId;
        result.Success = true;
        result.Availability = ava?.Available;
        result.SetBy = ava?.SetBy;
        return result;
    }

    public async Task<GetScheduledTrainingsForUserResponse> GetScheduledTrainingsForUser(Guid userId, Guid customerId, DateTime? fromDate, int take, int skip, OrderAscDesc order,
        CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new GetScheduledTrainingsForUserResponse();
        var userHolidays = await Database.UserHolidays.Where(x => x.CustomerId == customerId && x.UserId == userId && x.ValidFrom <= fromDate).ToListAsync(cancellationToken: clt);
        var defaultAveUser = await Database.UserDefaultAvailables.Include(x => x.DefaultGroup).Where(x => x.CustomerId == customerId && x.UserId == userId && x.ValidFrom <= fromDate)
            .ToListAsync(cancellationToken: clt);
        var scheduled = Database.RoosterAvailables
            .Where(x => x.CustomerId == customerId && x.UserId == userId && x.Assigned == true && x.Training.DeletedOn == null && (fromDate == null || x.Date >= fromDate))
            .Include(i => i.Training)
            .ThenInclude(i => i.RoosterAvailables!.Where(y => y.User!.DeletedOn == null))
            .Include(i => i.Training)
            .ThenInclude(i => i.TrainingTargetSet)
            .Include(i => i.Training)
            .ThenInclude(i => i.LinkVehicleTrainings)
            .Include(i => i.Training)
            .ThenInclude(i => i.LinkReportTrainingRoosterTrainings);
        var users = Database.Users.Where(x => x.CustomerId == customerId && x.DeletedOn == null);
        result.TotalCount = await scheduled.CountAsync(clt);
        var schedules = order switch
        {
            OrderAscDesc.Asc => scheduled.OrderBy(x => x.Date),
            OrderAscDesc.Desc => scheduled.OrderByDescending(x => x.Date),
            _ => throw new UnreachableException($"OrderAscDesc has unknown value {order}")
        };
        foreach (var schedule in await schedules.Skip(skip).Take(take).ToListAsync(clt))
        {
            if (schedule?.Training == null)
            {
                Logger.LogWarning("No training found for schedule '{ScheduleId}'", schedule?.Id);
                continue;
            }

            var plan = new PlannedTraining
            {
                TrainingId = schedule.TrainingId,
                DefaultId = schedule.Training.RoosterDefaultId,
                TrainingTargetSetId = schedule.Training.TrainingTargetSetId,
                Name = schedule.Training.Name,
                DateStart = schedule.Training.DateStart,
                DateEnd = schedule.Training.DateEnd,
                RoosterTrainingTypeId = schedule.Training.RoosterTrainingTypeId,
                PlannedFunctionId = schedule.UserFunctionId ?? (await users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken: clt))?.UserFunctionId,
                IsPinned = schedule.Training.IsPinned,
                IsPermanentPinned = schedule.Training.IsPermanentPinned,
                CountToTrainingTarget = schedule.Training.CountToTrainingTarget,
                IsCreated = true,
                ShowTime = schedule.Training.ShowTime ?? true,
                HasDescription = !schedule.Training.Description?.IsHtmlOnlyWhitespaceOrBreaks() ?? false,
                HasTargets = schedule.Training.TrainingTargetSet?.TrainingTargetIds.Count > 0,
                LinkedReports = schedule.Training.LinkReportTrainingRoosterTrainings?.Count ?? 0,
                PlanUsers = schedule.Training.RoosterAvailables!.Select(a =>
                    new PlanUser
                    {
                        AvailableId = a.Id,
                        UserId = a.UserId,
                        Availability = a.Available,
                        Assigned = a.Assigned,
                        Name = users.FirstOrDefault(x => x.Id == a.UserId)?.Name ?? "Name not found",
                        PlannedFunctionId = a.UserFunctionId ?? users.FirstOrDefault(x => x.Id == a.UserId)?.UserFunctionId,
                        UserFunctionId = users.FirstOrDefault(x => x.Id == a.UserId)?.UserFunctionId,
                        VehicleId = a.VehicleId,
                        Buddy = a.User?.LinkedUserAsA?.FirstOrDefault(x => x is { LinkType: UserUserLinkType.Buddy, DeletedOn: null })?.UserB.Name,
                    }).ToList()
            };
            var defaultVehicle = await GetDefaultVehicleForTraining(customerId, schedule.Training, clt);
            foreach (var user in plan.PlanUsers.Where(x => x.Assigned))
            {
                Availability? availabilty = user.Availability;
                AvailabilitySetBy? setBy = user.SetBy;
                GetAvailability(defaultAveUser, userHolidays, plan.DateStart, plan.DateEnd, plan.DefaultId, null, ref availabilty, ref setBy);
                user.Availability = availabilty;
                user.SetBy = setBy ?? user.SetBy;
                if (user.VehicleId is null ||
                    (user.VehicleId != defaultVehicle && !(schedule.Training.LinkVehicleTrainings?.Any(x => x.IsSelected && x.VehicleId == user.VehicleId) ?? false)))
                    user.VehicleId = defaultVehicle;
            }

            result.Trainings.Add(plan);
        }

        sw.Stop();
        result.Success = true;
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<GetUserMonthInfoResponse> GetUserMonthInfo(Guid userId, Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new GetUserMonthInfoResponse();

        var scheduled = Database.RoosterAvailables
            .Where(x => x.CustomerId == customerId && x.UserId == userId && x.Assigned == true && x.Training.DeletedOn == null)
            .Include(i => i.Training);
        result.TotalCount = await scheduled.CountAsync(clt);

        foreach (var schedule in scheduled)
        {
            var userMonthInfo = result.UserMonthInfo.FirstOrDefault(x => x.Year == schedule.Training.DateStart.Year && x.Month == schedule.Training.DateStart.Month);
            if (userMonthInfo is null)
            {
                userMonthInfo = new UserMonthInfo
                {
                    Year = schedule.Training.DateStart.Year,
                    Month = schedule.Training.DateStart.Month,
                    All = 1,
                    Valid = schedule.Training.CountToTrainingTarget ? 1 : 0,
                };
                result.UserMonthInfo.Add(userMonthInfo);
            }
            else
            {
                userMonthInfo.All += 1;
                if (schedule.Training.CountToTrainingTarget)
                    userMonthInfo.Valid += 1;
            }
        }

        sw.Stop();
        result.Success = true;
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<GetPinnedTrainingsForUserResponse> GetPinnedTrainingsForUser(Guid userId, Guid customerId, DateTime fromDate, CancellationToken token)
    {
        var result = new GetPinnedTrainingsForUserResponse();
        var trainings = await Database.RoosterTrainings
            .Include(i => i.RoosterAvailables!.Where(r => r.CustomerId == customerId && r.UserId == userId))
            .Include(i => i.TrainingTargetSet)
            .Where(x => x.CustomerId == customerId && (x.IsPinned || x.IsPermanentPinned) && x.DeletedOn == null && x.DateStart >= fromDate &&
                        (x.IsPermanentPinned || x.RoosterAvailables == null || !x.RoosterAvailables.Any(r => r.UserId == userId && r.Available > 0)))
            .OrderBy(x => x.DateStart)
            .ToListAsync(cancellationToken: token);
        var userHolidays = await Database.UserHolidays.Where(x => x.CustomerId == customerId && x.UserId == userId && x.ValidFrom <= fromDate).ToListAsync(cancellationToken: token);
        var defaultAveUser = await Database.UserDefaultAvailables.Include(x => x.DefaultGroup).Where(x => x.CustomerId == customerId && x.UserId == userId && x.ValidFrom <= fromDate)
            .ToListAsync(cancellationToken: token);

        foreach (var training in trainings)
        {
            var ava = training.RoosterAvailables?.FirstOrDefault(r => r.CustomerId == customerId && r.UserId == userId);
            Availability? available = ava?.Available;
            AvailabilitySetBy? setBy = ava?.SetBy;
            GetAvailability(defaultAveUser, userHolidays, training.DateStart, training.DateEnd, training.RoosterDefaultId, null, ref available, ref setBy);
            result.Trainings.Add(new Training
            {
                TrainingId = training.Id,
                DefaultId = training.RoosterDefaultId,
                TrainingTargetSetId = training.TrainingTargetSetId,
                Name = training.Name,
                DateStart = training.DateStart,
                DateEnd = training.DateEnd,
                Availability = available,
                SetBy = setBy ?? AvailabilitySetBy.None,
                Assigned = ava?.Assigned ?? false,
                RoosterTrainingTypeId = training.RoosterTrainingTypeId,
                CountToTrainingTarget = training.CountToTrainingTarget,
                IsPinned = training.IsPinned,
                IsPermanentPinned = training.IsPermanentPinned,
                ShowTime = training.ShowTime ?? true,
                HasDescription =  !training.Description?.IsHtmlOnlyWhitespaceOrBreaks() ?? false,
                HasTargets = training.TrainingTargetSet?.TrainingTargetIds.Count > 0
            });
        }

        return result;
    }

    public async Task<List<DbRoosterAvailable>> GetTrainingsThatRequireCalendarUpdate(Guid userId, Guid customerId)
    {
        var compareDate = DateTimeProvider.UtcNow().AddDays(-1);
        var ava = await Database.RoosterAvailables.Where(x =>
                x.CustomerId == customerId && x.Training.DeletedOn == null &&
                x.LastUpdateOn != null && x.LastUpdateOn > compareDate &&
                x.LastUpdateBy == userId &&
                (x.LastSyncOn == null || x.LastUpdateOn > x.LastSyncOn))
            .Include(x => x.Training)
            .ThenInclude(x => x.TrainingTargetSet)
            .AsNoTracking()
            .ToListAsync();
        return ava;
    }

    public async Task<bool> PatchEventIdForUserAvailible(Guid userId, Guid customerId, Guid? availableId, string? calendarEventId, CancellationToken clt)
    {
        var ava = await Database.RoosterAvailables.FindAsync(availableId);
        if (ava is null || ava.CustomerId != customerId)
        {
            return false;
        }

        ava.CalendarEventId = calendarEventId;
        ava.LastSyncOn = DateTimeProvider.UtcNow();
        Database.RoosterAvailables.Update(ava);
        return (await Database.SaveChangesAsync(clt) > 0);
    }

    public async Task<bool> DeleteTraining(Guid userId, Guid customerId, Guid trainingId, CancellationToken clt)
    {
        var training = await Database.RoosterTrainings.Include(x => x.RoosterAvailables).FirstOrDefaultAsync(x => x.Id == trainingId, clt);
        if (training?.CustomerId == customerId)
        {
            training.DeletedOn = DateTimeProvider.UtcNow();
            training.DeletedBy = userId;
            if (training.RoosterAvailables?.Any() == true)
            {
                foreach (var trainingRoosterAvailable in training.RoosterAvailables)
                {
                    trainingRoosterAvailable.LastUpdateBy = userId;
                    trainingRoosterAvailable.LastUpdateOn = DateTimeProvider.UtcNow();
                }
            }

            Database.RoosterTrainings.Update(training);
            return await Database.SaveChangesAsync(clt) > 0;
        }

        return false;
    }
}