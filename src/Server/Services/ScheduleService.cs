using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Exceptions;
using Drogecode.Knrm.Oefenrooster.Shared.Extensions;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ScheduleService : IScheduleService
{
    private readonly ILogger<ScheduleService> _logger;
    private readonly Database.DataContext _database;
    private readonly IMemoryCache _memoryCache;

    public ScheduleService(
        ILogger<ScheduleService> logger,
        Database.DataContext database,
        IMemoryCache memoryCache)
    {
        _logger = logger;
        _database = database;
        _memoryCache = memoryCache;
    }

    public async Task<MultipleTrainingsResponse> ScheduleForUserAsync(Guid userId, Guid customerId, int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd,
        CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultipleTrainingsResponse();
        var startDate = (new DateTime(yearStart, monthStart, dayStart, 0, 0, 0)).ToUniversalTime();
        var tillDate = (new DateTime(yearEnd, monthEnd, dayEnd, 23, 59, 59, 999)).ToUniversalTime();
        var defaults = await _database.RoosterDefaults.Where(x => x.CustomerId == customerId && x.ValidFrom <= tillDate && x.ValidUntil >= startDate)
            .AsSingleQuery().ToListAsync(cancellationToken: clt);
        var defaultAveUser = await _database.UserDefaultAvailables.Include(x => x.DefaultGroup)
            .Where(x => x.CustomerId == customerId && x.UserId == userId && x.ValidFrom <= tillDate && x.ValidUntil >= startDate)
            .AsSingleQuery().ToListAsync(cancellationToken: clt);
        var userHolidays = await _database.UserHolidays.Where(x => x.CustomerId == customerId && x.UserId == userId && x.ValidFrom <= tillDate && x.ValidUntil >= startDate)
            .AsSingleQuery().ToListAsync(cancellationToken: clt);
        var trainings = _database.RoosterTrainings.Where(x => x.CustomerId == customerId && x.DateStart >= startDate && x.DateStart <= tillDate).OrderBy(x => x.DateStart);
        var availables = await _database.RoosterAvailables.Where(x => x.CustomerId == customerId && x.UserId == userId && x.Date >= startDate && x.Date <= tillDate)
            .AsSingleQuery().ToListAsync(cancellationToken: clt);
        var roosterTrainingTypes = await _database.RoosterTrainingTypes.Where(x => x.CustomerId == customerId)
            .AsSingleQuery().ToListAsync(cancellationToken: clt);

        var scheduleDate = DateOnly.FromDateTime(startDate);
        var till = DateOnly.FromDateTime(tillDate);
        do
        {
            var defaultsFound = new List<Guid?>();
            var start = scheduleDate.ToDateTime(new TimeOnly(0, 0, 0), DateTimeKind.Utc);
            var end = scheduleDate.ToDateTime(new TimeOnly(23, 59, 59), DateTimeKind.Utc);
            var defaultsToday = defaults.Where(x => x.WeekDay == scheduleDate.DayOfWeek && x.ValidFrom <= start && x.ValidUntil >= end);
            var trainingsToday = trainings.Where(x => x.DateStart >= start && x.DateStart <= end);
            if (trainingsToday != null)
            {
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
                            TrainingId = training.Id,
                            Name = training.Name,
                            DateStart = training.DateStart,
                            DateEnd = training.DateEnd,
                            Availabilty = (Availabilty)(int)(available ?? Availability.None), //ToDo Remove when all users on v0.3.50 or above
                            Availability = available,
                            SetBy = setBy ?? AvailabilitySetBy.None,
                            Assigned = ava?.Assigned ?? false,
                            RoosterTrainingTypeId = training.RoosterTrainingTypeId,
                            CountToTrainingTarget = training.CountToTrainingTarget,
                            IsPinned = training.IsPinned,
                            ShowTime = training.ShowTime ?? true,
                        });
                    }

                    if (training.RoosterDefaultId != null)
                        defaultsFound.Add(training.RoosterDefaultId);
                }
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
                        Availabilty = (Availabilty)(int)(available ?? Availability.None), //ToDo Remove when all users on v0.3.50 or above
                        Availability = available ?? Availability.None,
                        SetBy = setBy ?? AvailabilitySetBy.None,
                        RoosterTrainingTypeId = def.RoosterTrainingTypeId,
                        CountToTrainingTarget = def.CountToTrainingTarget,
                        IsPinned = false,
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
        DbUserDefaultAvailable? defAvaForUser = null;
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
            defAvaForUser = defaultAveUser?.Where(x => x.RoosterDefaultId == roosterDefaultId && x.ValidFrom <= start && x.ValidUntil >= end && (userId is null || x.UserId == userId))
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
        var sw = Stopwatch.StartNew();
        training.Updated = false;
        training.Availability ??= (Availability)(int)(training.Availabilty ?? Availabilty.None); //ToDo Remove when all users on v0.3.50 or above
        var result = new PatchScheduleForUserResponse();
        if (training.DateStart.CompareTo(DateTime.UtcNow) >= 0)
        {
            result.PatchedTraining = training;
            var dbTraining = await CreateAndGetTraining(userId, customerId, training, clt);
            if (dbTraining is null) return result;
            var available = await _database.RoosterAvailables.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.UserId == userId && x.TrainingId == dbTraining.Id);
            if (available is null)
            {
                if (!await AddAvailableInternalAsync(userId, customerId, training)) return result;
            }
            else if (!await PatchAvailableInternalAsync(available, training)) return result;

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
            dbTraining = await _database.RoosterTrainings.FirstOrDefaultAsync(x =>
                x.CustomerId == customerId && x.RoosterDefaultId == training.DefaultId && x.DateStart == training.DateStart && x.DateEnd == training.DateEnd);
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
            dbTraining = await _database.RoosterTrainings.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Id == training.TrainingId);
        if (dbTraining == null)
        {
            if (!await AddTrainingInternalAsync(customerId, training, clt)) return dbTraining;
            dbTraining = await _database.RoosterTrainings.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Id == training.TrainingId);
            if (dbTraining == null)
            {
                _logger.LogWarning("Two tries to add training {date} for user {userId} from customer {customerId} failed", training.DateStart, userId, customerId);
                return dbTraining;
            }
        }

        return dbTraining;
    }

    public async Task<PatchTrainingResponse> PatchTraining(Guid customerId, PlannedTraining patchedTraining, bool inRoleEditPast, CancellationToken token)
    {
        var sw = Stopwatch.StartNew();
        var result = new PatchTrainingResponse();
        var oldTraining = await _database.RoosterTrainings.FindAsync(new object?[] { patchedTraining.TrainingId }, cancellationToken: token);
        if (oldTraining == null) return result;
        if (patchedTraining.Name?.Length > DefaultSettingsHelper.MAX_LENGTH_TRAINING_TITLE)
            throw new DrogeCodeToLongException();
        if (!inRoleEditPast && oldTraining.DateEnd < DateTime.UtcNow.AddDays(AccessesSettings.AUTH_scheduler_edit_past_days - 1))
        {
            throw new UnauthorizedAccessException();
        }

        oldTraining.RoosterTrainingTypeId = patchedTraining.RoosterTrainingTypeId;
        oldTraining.Name = patchedTraining.Name;
        oldTraining.Description = patchedTraining.Description;
        oldTraining.DateStart = patchedTraining.DateStart;
        oldTraining.DateEnd = patchedTraining.DateEnd;
        oldTraining.CountToTrainingTarget = patchedTraining.CountToTrainingTarget;
        oldTraining.IsPinned = patchedTraining.IsPinned;
        oldTraining.ShowTime = patchedTraining.ShowTime;
        _database.RoosterTrainings.Update(oldTraining);
        result.Success = (await _database.SaveChangesAsync()) > 0;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<AddTrainingResponse> AddTrainingAsync(Guid customerId, PlannedTraining newTraining, Guid trainingId, CancellationToken token)
    {
        var sw = Stopwatch.StartNew();
        var result = new AddTrainingResponse();
        result.NewId = trainingId;
        var training = new Training
        {
            TrainingId = trainingId,
            DefaultId = newTraining.DefaultId,
            RoosterTrainingTypeId = newTraining.RoosterTrainingTypeId,
            Name = newTraining.Name,
            Description = newTraining.Description,
            DateStart = newTraining.DateStart,
            DateEnd = newTraining.DateEnd,
            CountToTrainingTarget = newTraining.CountToTrainingTarget,
            IsPinned = newTraining.IsPinned,
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
        await _database.RoosterTrainings.AddAsync(new DbRoosterTraining
        {
            Id = training.TrainingId ?? throw new NoNullAllowedException("TrainingId is still null while adding new training"),
            RoosterDefaultId = training.DefaultId,
            CustomerId = customerId,
            RoosterTrainingTypeId = training.RoosterTrainingTypeId,
            Name = training.Name,
            Description = training.Description,
            DateStart = training.DateStart,
            DateEnd = training.DateEnd,
            CountToTrainingTarget = training.CountToTrainingTarget,
            IsPinned = training.IsPinned,
            ShowTime = training.ShowTime,
        }, clt);
        if (training.DefaultId is not null && training.DefaultId != Guid.Empty)
        {
            var roosterDefault = await _database.RoosterDefaults.FirstOrDefaultAsync(x => x.Id == training.DefaultId);
            if (roosterDefault?.VehicleIds is not null)
            {
                foreach (var vehicleFromRoosterDefault in roosterDefault.VehicleIds)
                {
                    _database.LinkVehicleTraining.Add(new DbLinkVehicleTraining
                    {
                        Id = Guid.NewGuid(),
                        RoosterTrainingId = training.TrainingId.Value,
                        VehicleId = vehicleFromRoosterDefault,
                        CustomerId = customerId,
                        IsSelected = true,
                    });
                }

                var defaultSelectedVehicles = await _database.Vehicles.Where(x => x.IsDefault).Select(x => x.Id).ToListAsync(clt);

                foreach (var defVehicle in defaultSelectedVehicles)
                {
                    if (roosterDefault.VehicleIds.Any(x => x == defVehicle)) continue;
                    _database.LinkVehicleTraining.Add(new DbLinkVehicleTraining
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

        return (await _database.SaveChangesAsync()) > 0;
    }

    private async Task<bool> AddAvailableInternalAsync(Guid userId, Guid customerId, Training training)
    {
        await _database.RoosterAvailables.AddAsync(new DbRoosterAvailable
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CustomerId = customerId,
            TrainingId = training.TrainingId ?? throw new NoNullAllowedException("TrainingId is still null while adding available"),
            Date = training.DateStart,
            Available = training.Availability,
            SetBy = training.SetBy
        });
        return (await _database.SaveChangesAsync()) > 0;
    }

    private async Task<bool> PatchAvailableInternalAsync(DbRoosterAvailable available, Training training)
    {
        available.Available = training.Availability;
        available.SetBy = training.SetBy;
        _database.RoosterAvailables.Update(available);
        return (await _database.SaveChangesAsync()) > 0;
    }

    public async Task<ScheduleForAllResponse> ScheduleForAllAsync(Guid userId, Guid customerId, int forMonth, int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd,
        bool countPerUser, bool includeUnAssigned, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var cacheKey = $"SchedForAll-{customerId}{forMonth}-{yearStart}-{monthStart}-{dayStart}-{yearEnd}-{monthEnd}-{dayEnd}-{countPerUser}-{includeUnAssigned}";
        _memoryCache.TryGetValue(cacheKey, out ScheduleForAllResponse? result);
        // Cache will give issues with editing trainings.
        /*if (result is not null)
        {
            sw.Stop();
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return result;
        }*/
        result = new ScheduleForAllResponse();
        var startDate = (new DateTime(yearStart, monthStart, dayStart, 0, 0, 0)).ToUniversalTime();
        var tillDate = (new DateTime(yearEnd, monthEnd, dayEnd, 23, 59, 59, 999)).ToUniversalTime();
        var users = await _database.Users.Include(x => x.UserDefaultAvailables).Include(x => x.UserFunction).Where(x => x.CustomerId == customerId && x.DeletedOn == null && x.UserFunction!.IsActive)
            .Select(x => new { x.Id, x.UserDefaultAvailables, x.UserFunctionId, x.Name })
            .AsSingleQuery().ToListAsync(cancellationToken: clt);
        var defaults = await _database.RoosterDefaults.Where(x => x.CustomerId == customerId && x.ValidFrom <= tillDate && x.ValidUntil >= startDate).Select(x =>
                new { x.Id, x.WeekDay, x.ValidFrom, x.ValidUntil, x.TimeZone, x.TimeStart, x.TimeEnd, x.Name, x.ShowTime, x.RoosterTrainingTypeId, x.CountToTrainingTarget })
            .AsSingleQuery().ToListAsync(cancellationToken: clt);
        var defaultAveUser = await _database.UserDefaultAvailables.Include(x => x.DefaultGroup).Where(x => x.CustomerId == customerId && x.ValidFrom <= tillDate && x.ValidUntil >= startDate)
            .AsSingleQuery().ToListAsync(cancellationToken: clt);
        var userHolidays = await _database.UserHolidays.Where(x => x.CustomerId == customerId && x.ValidFrom <= tillDate && x.ValidUntil >= startDate)
            .AsSingleQuery().ToListAsync(cancellationToken: clt);
        var trainings = _database.RoosterTrainings.Where(x => x.CustomerId == customerId && x.DateStart >= startDate && x.DateStart <= tillDate).Include(x=>x.LinkVehicleTrainings).OrderBy(x => x.DateStart)
            .AsSingleQuery().ToList();
        var availables = _database.RoosterAvailables.Where(x => x.CustomerId == customerId && (includeUnAssigned || x.Assigned) && x.Date >= startDate && x.Date <= tillDate)
            .AsSingleQuery().ToList();

        var scheduleDate = DateOnly.FromDateTime(startDate);
        do
        {
            var defaultsFound = new List<Guid?>();
            var start = scheduleDate.ToDateTime(new TimeOnly(0, 0, 0, 0), DateTimeKind.Utc);
            var end = scheduleDate.ToDateTime(new TimeOnly(23, 59, 59, 999), DateTimeKind.Utc);
            var defaultsToday = defaults.Where(x => x.WeekDay == scheduleDate.DayOfWeek && x.ValidFrom <= start && x.ValidUntil >= end);
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
                            TrainingId = training.Id,
                            Name = training.Name,
                            DateStart = training.DateStart,
                            DateEnd = training.DateEnd,
                            IsCreated = true,
                            RoosterTrainingTypeId = training.RoosterTrainingTypeId,
                            CountToTrainingTarget = training.CountToTrainingTarget,
                            IsPinned = training.IsPinned,
                            ShowTime = training.ShowTime ?? true,
                        };
                        foreach (var user in users)
                        {
                            if (user == null) continue;
                            var avaUser = ava.FirstOrDefault(x => x.UserId == user.Id && (includeUnAssigned || x.Assigned));
                            if (avaUser is null && !includeUnAssigned) continue;
                            avaUser ??= new DbRoosterAvailable();
                            defVehicle ??= await GetDefaultVehicleForTraining(customerId, training, clt);
                            if (avaUser.VehicleId is null || (avaUser.VehicleId != defVehicle && !(await IsVehicleSelectedForTraining(customerId, training.Id, avaUser.VehicleId, clt))))
                            {
                                /*if (avaUser is { Assigned: true, VehicleId: not null } && avaUser.UserId != Guid.Empty && avaUser.VehicleId != defVehicle &&
                                    !await IsVehicleSelectedForTraining(customerId, training.Id, avaUser.VehicleId, clt))
                                {
                                    // Fix on the run.
                                    avaUser.VehicleId = null;
                                    _database.RoosterAvailables.Update(avaUser);
                                }*/

                                avaUser.VehicleId = defVehicle;
                            }

                            Availability? available = avaUser.Available;
                            AvailabilitySetBy? setBy = avaUser.SetBy;
                            GetAvailability(defaultAveUser, userHolidays, training.DateStart, training.DateEnd, training.RoosterDefaultId, user.Id, ref available, ref setBy);
                            newPlanner.PlanUsers.Add(new PlanUser
                            {
                                UserId = user.Id,
                                Availabilty = (Availabilty)(int)(available ?? Availability.None), //ToDo Remove when all users on v0.3.50 or above
                                Availability = available,
                                SetBy = setBy ?? AvailabilitySetBy.None,
                                Assigned = avaUser.Assigned,
                                Name = user.Name ?? "Name not found",
                                PlannedFunctionId = avaUser.UserFunctionId ?? user.UserFunctionId,
                                UserFunctionId = user.UserFunctionId,
                                VehicleId = training.LinkVehicleTrainings?.Where(x=>x.IsSelected).Any(x=>x.VehicleId == avaUser.VehicleId) ?? false ? avaUser.VehicleId : null
                            });
                            if (countPerUser && training.CountToTrainingTarget && scheduleDate.Month == forMonth && avaUser.Assigned == true)
                            {
                                var indexUser = result.UserTrainingCounters.FindIndex(X => X.UserId.Equals(avaUser.UserId));
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
                                Availabilty = (Availabilty)(int)(available ?? Availability.None), //ToDo Remove when all users on v0.3.50 or above
                                Availability = available,
                                SetBy = setBy ?? AvailabilitySetBy.None,
                                Assigned = false,
                                Name = user.Name ?? "Name not found",
                                PlannedFunctionId = user.UserFunctionId,
                                UserFunctionId = user.UserFunctionId,
                            });
                        }
                    }

                    result.Planners.Add(newPlanner);
                }
            }

            clt.ThrowIfCancellationRequested();
            scheduleDate = scheduleDate.AddDays(1);
        } while (scheduleDate <= DateOnly.FromDateTime(tillDate));

        _ = await _database.SaveChangesAsync(clt);
        result.Success = true;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        /*var cacheOptions = new MemoryCacheEntryOptions();
        cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(3));
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
        _memoryCache.Set(cacheKey, result, cacheOptions);*/
        return result;
    }

    private async Task<Guid?> GetDefaultVehicleForTraining(Guid? customerId, DbRoosterTraining training, CancellationToken clt)
    {
        var cacheKey = $"DefVehTr-{customerId}{training.Id}";
        _memoryCache.TryGetValue(cacheKey, out Guid? result);
        if (result is not null)
            return result.Value;
        var vehicles = await InternalGetVehiclesCached(customerId, clt);
        DbVehicles? vehiclePrev = null;
        foreach (var vehicle in vehicles.OrderBy(x=>x.Order))
        {
            if (training.LinkVehicleTrainings?.Any(x => x.VehicleId == vehicle.Id) == true)
            {
                var veh = training.LinkVehicleTrainings.FirstOrDefault(x => x.VehicleId == vehicle.Id);
                if (!veh!.IsSelected) continue;
                vehiclePrev = vehicle;
                if (vehicle.IsDefault)
                    break;
            }
            else if (vehicle.IsDefault)
            {
                vehiclePrev = vehicle;
            }
        }

        var cacheOptions = new MemoryCacheEntryOptions();
        cacheOptions.SetSlidingExpiration(TimeSpan.FromSeconds(15));
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(45));
        _memoryCache.Set(cacheKey, vehiclePrev?.Id, cacheOptions);
        return vehiclePrev?.Id;
    }

    private async Task<List<DbVehicles>> InternalGetVehiclesCached(Guid? customerId, CancellationToken clt)
    {
        var cacheKey = $"VehsFoCus-{customerId}";
        _memoryCache.TryGetValue(cacheKey, out List<DbVehicles>? result);
        if (result is not null) return result;
        result = await _database.Vehicles.Where(x => x.CustomerId == customerId).ToListAsync(clt);
        var cacheOptions = new MemoryCacheEntryOptions();
        cacheOptions.SetSlidingExpiration(TimeSpan.FromSeconds(30));
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        _memoryCache.Set(cacheKey, result, cacheOptions);
        return result;
    }

    private async Task<bool> IsVehicleSelectedForTraining(Guid? customerId, Guid? trainingId, Guid? vehicleId, CancellationToken clt)
    {
        var cacheKey = $"IsVehSel-{customerId}{trainingId}{vehicleId}";
        _memoryCache.TryGetValue(cacheKey, out bool? result);
        if (result is not null)
            return result.Value;
        var links = await _database.LinkVehicleTraining.Where(x => x.CustomerId == customerId && x.RoosterTrainingId == trainingId).Select(x => new { x.VehicleId, x.IsSelected }).ToListAsync(clt);
        var isSelected = links.Any(x => x.VehicleId == vehicleId && x.IsSelected);
        var cacheOptions = new MemoryCacheEntryOptions();
        cacheOptions.SetSlidingExpiration(TimeSpan.FromSeconds(5));
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(15));
        _memoryCache.Set(cacheKey, isSelected, cacheOptions);
        return isSelected;
    }

    public async Task<GetTrainingByIdResponse> GetTrainingById(Guid userId, Guid customerId, Guid trainingId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new GetTrainingByIdResponse();
        var dbTraining = await _database.RoosterTrainings
            .Include(x => x.RoosterAvailables)
            .FirstOrDefaultAsync(x => x.Id == trainingId && x.DeletedOn == null, clt);
        if (dbTraining is not null)
        {
            var training = dbTraining.ToTraining(userId);
            result.Training = training;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<GetPlannedTrainingResponse> GetPlannedTrainingById(Guid customerId, Guid trainingId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new GetPlannedTrainingResponse();
        var dbTraining = await _database.RoosterTrainings
            .Include(x => x.RoosterAvailables!.Where(y => y.User!.DeletedOn == null))
            .ThenInclude(x => x.User)
            .ThenInclude(x => x!.LinkedUserAsA!.Where(y => y.DeletedOn == null))
            .ThenInclude(x => x.UserB)
            .Include(x => x.RoosterTrainingType)
            .Include(x => x.RoosterAvailables!.Where(y => y.User!.DeletedOn == null))
            .Include(x=>x.LinkVehicleTrainings!)
            .ThenInclude(x=>x.Vehicles)
            .AsSingleQuery()
            .FirstOrDefaultAsync(x => x.Id == trainingId && x.DeletedOn == null, clt);
        if (dbTraining is not null)
        {
            result = await dDbTrainingToGetPlannedTrainingResponse(result, dbTraining, clt);

            var ava = await _database.UserDefaultAvailables.Include(x => x.DefaultGroup).Where(x => x.CustomerId == customerId && x.RoosterDefaultId == dbTraining!.RoosterDefaultId)
                .ToListAsync(cancellationToken: clt);
            var userHolidays = await _database.UserHolidays.Where(x => x.CustomerId == customerId && x.ValidFrom <= dbTraining.DateEnd.AddDays(1) && x.ValidUntil >= dbTraining.DateStart.AddDays(-1))
                .ToListAsync(cancellationToken: clt);
            var availables = _database.RoosterAvailables.Where(x => x.CustomerId == customerId && x.TrainingId == trainingId);
            var users = await _database.Users.Include(x => x.UserFunction).Where(x => x.CustomerId == customerId && x.DeletedOn == null && x.UserFunction!.IsActive)
                .ToListAsync(cancellationToken: clt);

            foreach (var user in users)
            {
                var avaUser = availables.FirstOrDefault(x => x.UserId == user.Id);
                Availability? available = avaUser?.Available;
                AvailabilitySetBy? setBy = avaUser?.SetBy;
                GetAvailability(ava, userHolidays, dbTraining.DateStart, dbTraining.DateEnd, dbTraining.RoosterDefaultId, user.Id, ref available, ref setBy);
                if (result.Training!.PlanUsers.Any(x => x.UserId == user.Id))
                {
                    var d = result.Training.PlanUsers.FirstOrDefault(x => x.UserId == user.Id);
                    d!.Availabilty = (Availabilty)(int)(available ?? Availability.None); //ToDo Remove when all users on v0.3.50 or above
                    d!.Availability = available;
                    d.SetBy = setBy ?? AvailabilitySetBy.None;
                    d.Name = user.Name;
                    d.UserFunctionId = user.UserFunctionId;
                    d.PlannedFunctionId = avaUser?.UserFunctionId ?? user.UserFunctionId;
                }
                else if (!(available is null || setBy is null || setBy == AvailabilitySetBy.None))
                {
                    result.Training!.PlanUsers.Add(new PlanUser
                    {
                        Availabilty = (Availabilty)(int)available, //ToDo Remove when all users on v0.3.50 or above
                        Availability = available,
                        SetBy = setBy.Value,
                        Name = user.Name,
                        UserId = user.Id,
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
        var sw = Stopwatch.StartNew();
        date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        var result = new GetPlannedTrainingResponse();
        var dbTraining = await _database.RoosterTrainings
            .Include(x => x.RoosterAvailables)
            .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.DateStart.Date == date.Date && x.RoosterDefaultId == defaultId);
        if (dbTraining is not null)
            result = await dDbTrainingToGetPlannedTrainingResponse(result, dbTraining, clt);
        else
        {
            TimeOnly startTime = TimeOnly.FromDateTime(date);
            var def = await _database.RoosterDefaults.FirstOrDefaultAsync(x => x.Id == defaultId && x.WeekDay == date.DayOfWeek);
            if (def is not null)
            {
                var trainingStart = date.Date.AddHours(def.TimeStart.Hour).AddMinutes(def.TimeStart.Minute).AddSeconds(def.TimeStart.Second);
                var trainingEnd = date.Date.AddHours(def.TimeEnd.Hour).AddMinutes(def.TimeEnd.Minute).AddSeconds(def.TimeEnd.Second);

                var users = await _database.Users
                    .Include(x => x.UserDefaultAvailables)
                    .Include(x => x.UserFunction)
                    .Include(x => x.LinkedUserAsA!.Where(y => y.DeletedOn == null))
                    .ThenInclude(x => x.UserB)
                    .Where(x => x.CustomerId == customerId && x.DeletedOn == null && x.UserFunction!.IsActive)
                    .ToListAsync(cancellationToken: clt);
                var defaultAveUser = await _database.UserDefaultAvailables.Include(x => x.DefaultGroup)
                    .Where(x => x.CustomerId == customerId && x.ValidFrom <= trainingStart && x.ValidUntil >= trainingEnd).ToListAsync(cancellationToken: clt);
                var userHolidays = await _database.UserHolidays.Where(x => x.CustomerId == customerId && x.ValidFrom <= trainingStart && x.ValidUntil >= trainingEnd)
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
                        Availabilty = (Availabilty)(int)(available ?? Availability.None), //ToDo Remove when all users on v0.3.50 or above
                        Availability = available,
                        SetBy = setBy ?? AvailabilitySetBy.None,
                        Assigned = false,
                        Name = dbUser.Name ?? "Name not found",
                        PlannedFunctionId = dbUser.UserFunctionId,
                        UserFunctionId = dbUser.UserFunctionId,
                        Buddy = dbUser.LinkedUserAsA?.FirstOrDefault(x => x.LinkType == UserUserLinkType.Buddy)?.UserB?.Name
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

    public async Task<PatchAssignedUserResponse> PatchAssignedUserAsync(Guid userId, Guid customerId, PatchAssignedUserRequest body, CancellationToken clt)
    {
        var result = new PatchAssignedUserResponse();
        result.IdPatched = body.TrainingId;
        if (body.User == null)
        {
            _logger.LogWarning("user is null {UserIsNull}", body.User == null);
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
                _logger.LogWarning("Unable to find or create training");
                return null;
            }
        }

        if (!_database.RoosterTrainings.Any(x => x.CustomerId == customerId && x.Id == body.TrainingId))
        {
            _logger.LogWarning("No training with '{Id}' found", body.TrainingId);
            return result;
        }

        var ava = await _database.RoosterAvailables.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.TrainingId == body.TrainingId && x.UserId == body.User.UserId, cancellationToken: clt);
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
            ava = await _database.RoosterAvailables.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.TrainingId == body.TrainingId && x.UserId == body.User.UserId, cancellationToken: clt);
            if (ava is null)
            {
                _logger.LogWarning("No RoosterAvailable with '{Id}' found for user '{User}' when Assigned = '{Assigned}'", body.TrainingId, body.User.UserId, body.User.Assigned);
                return result;
            }
        }

        ava.Assigned = body.User.Assigned;
        ava.UserFunctionId = body.User.PlannedFunctionId;
        ava.VehicleId = body.User.VehicleId;
        _database.RoosterAvailables.Update(ava);
        await _database.SaveChangesAsync(clt);
        result.Success = true;
        result.AvailableId = ava.Id;
        result.CalendarEventId = ava.CalendarEventId;
        result.Availabilty = (Availabilty)(int)(ava.Available ?? Availability.None); //ToDo Remove when all users on v0.3.50 or above
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
            _logger.LogWarning("userId is null {UserIsNull} or trainingId is null {TrainingIsNull}", body.UserId == null, body.TrainingId == null);
            return result;
        }

        var user = await _database.Users.FirstOrDefaultAsync(x => x.Id == body.UserId && x.DeletedOn == null && x.CustomerId == customerId, cancellationToken: clt);
        if (user is null)
        {
            _logger.LogWarning("No user with '{Id}' found", body.UserId);
            return result;
        }

        if (body.FunctionId == null)
        {
            body.FunctionId = user.UserFunctionId;
        }

        var training = await CreateAndGetTraining(userId, customerId, body.Training, clt);
        if (training is null)
        {
            _logger.LogWarning("No training with '{Id}' found", body.TrainingId);
            return result;
        }

        result.IdPut = training.Id;
        var ava = await _database.RoosterAvailables.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.TrainingId == body.TrainingId && x.UserId == body.UserId, cancellationToken: clt);
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
                    Assigned = body.Assigned
                };
                _database.RoosterAvailables.Add(ava);
                await _database.SaveChangesAsync(clt);
            }
        }
        else
        {
            ava.Assigned = body.Assigned;
            if (body.Assigned)
                ava.UserFunctionId = body.FunctionId;
            else
                ava.UserFunctionId = null;
            _database.RoosterAvailables.Update(ava);
            await _database.SaveChangesAsync(clt);
        }

        result.AvailableId = ava?.Id;
        result.CalendarEventId = ava?.CalendarEventId;
        result.Success = true;
        result.Availabilty = result.Availabilty = (Availabilty)(int)(ava?.Available ?? Availability.None); //ToDo Remove when all users on v0.3.50 or above
        result.Availability = ava?.Available;
        result.SetBy = ava?.SetBy;
        return result;
    }

    public async Task<GetScheduledTrainingsForUserResponse> GetScheduledTrainingsForUser(Guid userId, Guid customerId, DateTime? fromDate, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new GetScheduledTrainingsForUserResponse();
        var userHolidays = await _database.UserHolidays.Where(x => x.CustomerId == customerId && x.UserId == userId && x.ValidFrom <= fromDate).ToListAsync(cancellationToken: clt);
        var defaultAveUser = await _database.UserDefaultAvailables.Include(x => x.DefaultGroup).Where(x => x.CustomerId == customerId && x.UserId == userId && x.ValidFrom <= fromDate)
            .ToListAsync(cancellationToken: clt);
        var scheduled = await _database.RoosterAvailables
            .Where(x => x.CustomerId == customerId && x.UserId == userId && x.Assigned == true && x.Training.DeletedOn == null && (fromDate == null || x.Date >= fromDate))
            .Include(i => i.Training)
            .ThenInclude(i => i.RoosterAvailables!.Where(y => y.User!.DeletedOn == null))
            .Include(i => i.Training)
            .ThenInclude(i => i.LinkVehicleTrainings)
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken: clt);
        var users = _database.Users.Where(x => x.CustomerId == customerId && x.DeletedOn == null);
        foreach (var schedul in scheduled)
        {
            if (schedul.Training == null)
            {
                _logger.LogWarning("No training found for schedule '{ScheduleId}'", schedul.Id);
                continue;
            }

            var defaultVehicle = await GetDefaultVehicleForTraining(customerId, schedul.Training, clt);
            var plan = new PlannedTraining
            {
                TrainingId = schedul.TrainingId,
                DefaultId = schedul.Training.RoosterDefaultId,
                Name = schedul.Training.Name,
                DateStart = schedul.Training.DateStart,
                DateEnd = schedul.Training.DateEnd,
                RoosterTrainingTypeId = schedul.Training.RoosterTrainingTypeId,
                PlannedFunctionId = schedul.UserFunctionId ?? users?.FirstOrDefault(x => x.Id == userId)?.UserFunctionId,
                IsPinned = schedul.Training.IsPinned,
                CountToTrainingTarget = schedul.Training.CountToTrainingTarget,
                IsCreated = true,
                ShowTime = schedul.Training.ShowTime ?? true,
                PlanUsers = schedul.Training.RoosterAvailables!.Select(a =>
                    new PlanUser
                    {
                        UserId = a.UserId,
                        Availabilty = (Availabilty)(int)(a.Available ?? Availability.None), //ToDo Remove when all users on v0.3.50 or above
                        Availability = a.Available,
                        Assigned = a.Assigned,
                        Name = users?.FirstOrDefault(x => x.Id == a.UserId)?.Name ?? "Name not found",
                        PlannedFunctionId = a.UserFunctionId ?? users?.FirstOrDefault(x => x.Id == a.UserId)?.UserFunctionId,
                        UserFunctionId = users?.FirstOrDefault(x => x.Id == a.UserId)?.UserFunctionId,
                        VehicleId = a.VehicleId,
                    }).ToList()
            };
            foreach (var user in plan.PlanUsers.Where(x => x.Assigned))
            {
                Availability? availabilty = user.Availability;
                AvailabilitySetBy? setBy = user.SetBy;
                GetAvailability(defaultAveUser, userHolidays, plan.DateStart, plan.DateEnd, plan.DefaultId, null, ref availabilty, ref setBy);
                user.Availability = availabilty;
                user.SetBy = setBy ?? user.SetBy;
                if (user.VehicleId is null || (user.VehicleId != defaultVehicle && !(await IsVehicleSelectedForTraining(customerId, schedul.TrainingId, user.VehicleId, clt))))
                    user.VehicleId = defaultVehicle;
            }

            result.Trainings.Add(plan);
            var userMonthInfo = result.UserMonthInfos.FirstOrDefault(x => x.Year == schedul.Training.DateStart.Year && x.Month == schedul.Training.DateStart.Month);
            if (userMonthInfo is null)
            {
                userMonthInfo = new UserMonthInfo
                {
                    Year = schedul.Training.DateStart.Year,
                    Month = schedul.Training.DateStart.Month,
                    All = 1,
                    Valid = schedul.Training.CountToTrainingTarget ? 1 : 0,
                };
                result.UserMonthInfos.Add(userMonthInfo);
            }
            else
            {
                userMonthInfo.All += 1;
                if (schedul.Training.CountToTrainingTarget)
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
        var trainings = await _database.RoosterTrainings
            .Include(i => i.RoosterAvailables!.Where(r => r.CustomerId == customerId && r.UserId == userId))
            .Where(x => x.CustomerId == customerId && x.IsPinned && x.DeletedOn == null && x.DateStart >= fromDate &&
                        (x.RoosterAvailables == null || !x.RoosterAvailables.Any(r => r.UserId == userId && r.Available > 0)))
            .OrderBy(x => x.DateStart)
            .ToListAsync(cancellationToken: token);
        var userHolidays = await _database.UserHolidays.Where(x => x.CustomerId == customerId && x.UserId == userId && x.ValidFrom <= fromDate).ToListAsync(cancellationToken: token);
        var defaultAveUser = await _database.UserDefaultAvailables.Include(x => x.DefaultGroup).Where(x => x.CustomerId == customerId && x.UserId == userId && x.ValidFrom <= fromDate)
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
                Name = training.Name,
                DateStart = training.DateStart,
                DateEnd = training.DateEnd,
                Availabilty = (Availabilty)(int)(available ?? Availability.None), //ToDo Remove when all users on v0.3.50 or above
                Availability = available,
                SetBy = setBy ?? AvailabilitySetBy.None,
                Assigned = ava?.Assigned ?? false,
                RoosterTrainingTypeId = training.RoosterTrainingTypeId,
                CountToTrainingTarget = training.CountToTrainingTarget,
                IsPinned = training.IsPinned,
                ShowTime = training.ShowTime ?? true,
            });
        }

        return result;
    }

    public async Task<bool> PatchEventIdForUserAvailible(Guid userId, Guid customerId, Guid? availableId, string? calendarEventId, CancellationToken clt)
    {
        var ava = await _database.RoosterAvailables.FindAsync(availableId, clt);
        if (ava is null || ava.CustomerId != customerId)
        {
            return false;
        }

        ava.CalendarEventId = calendarEventId;
        _database.RoosterAvailables.Update(ava);
        return (await _database.SaveChangesAsync(clt) > 0);
    }

    public async Task<bool> DeleteTraining(Guid userId, Guid customerId, Guid trainingId, CancellationToken clt)
    {
        var training = await _database.RoosterTrainings.FindAsync(trainingId);
        if (training?.CustomerId == customerId)
        {
            training.DeletedOn = DateTime.UtcNow;
            training.DeletedBy = userId;
            _database.RoosterTrainings.Update(training);
            return _database.SaveChanges() > 0;
        }

        return false;
    }
}