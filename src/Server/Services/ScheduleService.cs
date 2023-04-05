using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Exceptions;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using System.Data;
using System.Runtime.Intrinsics.X86;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ScheduleService : IScheduleService
{
    private readonly ILogger<ScheduleService> _logger;
    private readonly Database.DataContext _database;
    public ScheduleService(ILogger<ScheduleService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<ScheduleForUserResponse> ScheduleForUserAsync(Guid userId, Guid customerId, int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, CancellationToken token)
    {
        var result = new ScheduleForUserResponse();
        var startDate = (new DateTime(yearStart, monthStart, dayStart, 0, 0, 0)).ToUniversalTime();
        var tillDate = (new DateTime(yearEnd, monthEnd, dayEnd, 23, 59, 59, 999)).ToUniversalTime();
        var defaults = _database.RoosterDefaults.Where(x => x.CustomerId == customerId && x.ValidFrom <= startDate && x.ValidUntil >= startDate);
        var trainings = _database.RoosterTrainings.Where(x => x.CustomerId == customerId && x.DateStart >= startDate && x.DateStart <= tillDate);
        var availables = await _database.RoosterAvailables.Where(x => x.CustomerId == customerId && x.UserId == userId && x.Date >= startDate && x.Date <= tillDate).ToListAsync(cancellationToken: token);
        var roosterTrainingTypes = await _database.RoosterTrainingTypes.Where(x => x.CustomerId == customerId).ToListAsync(cancellationToken: token);

        var scheduleDate = DateOnly.FromDateTime(startDate);
        var till = DateOnly.FromDateTime(tillDate);
        do
        {
            var defaultsFound = new List<Guid?>();
            var defaultsToday = defaults.Where(x => x.WeekDay == scheduleDate.DayOfWeek);
            var start = scheduleDate.ToDateTime(new TimeOnly(0, 0, 0, 0), DateTimeKind.Utc);
            var end = scheduleDate.ToDateTime(new TimeOnly(23, 59, 59, 999), DateTimeKind.Utc);
            var trainingsToday = trainings.Where(x => x.DateStart >= start && x.DateStart <= end);
            if (trainingsToday != null)
            {
                foreach (var training in trainingsToday)
                {
                    var ava = availables.FirstOrDefault(x => x.TrainingId == training.Id);
                    result.Trainings.Add(new Training
                    {
                        DefaultId = training.RoosterDefaultId,
                        TrainingId = training.Id,
                        Name = training.Name,
                        DateStart = training.DateStart,
                        DateEnd = training.DateEnd,
                        Availabilty = ava?.Available,
                        Assigned = ava?.Assigned ?? false,
                        RoosterTrainingTypeId = training.RoosterTrainingTypeId,
                        VehicleId = ava?.VehicleId,
                        CountToTrainingTarget = training.CountToTrainingTarget,
                        Pin = training.Pin,
                    });
                    if (training.RoosterDefaultId != null)
                        defaultsFound.Add(training.RoosterDefaultId);
                }
            }
            foreach (var def in defaultsToday)
            {
                if (!defaultsFound.Contains(def.Id))
                {
                    result.Trainings.Add(new Training
                    {
                        DefaultId = def.Id,
                        DateStart = scheduleDate.ToDateTime(def.TimeStart, DateTimeKind.Utc),
                        DateEnd = scheduleDate.ToDateTime(def.TimeEnd, DateTimeKind.Utc),
                        Availabilty = Availabilty.None,
                        RoosterTrainingTypeId = def.RoosterTrainingTypeId,
                        CountToTrainingTarget = def.CountToTrainingTarget,
                        Pin = false,
                    });
                }
            }
            token.ThrowIfCancellationRequested();
            scheduleDate = scheduleDate.AddDays(1);

        } while (scheduleDate <= till);

        return result;
    }

    public async Task<Training> PatchScheduleForUserAsync(Guid userId, Guid customerId, Training training, CancellationToken token)
    {
        training.Updated = false;
        DbRoosterTraining? dbTraining = null;
        if (training.TrainingId == null || training.TrainingId == Guid.Empty)
        {
            dbTraining = await _database.RoosterTrainings.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.RoosterDefaultId == training.DefaultId && x.DateStart == training.DateStart && x.DateEnd == training.DateEnd);
            if (dbTraining == null)
            {
                training.TrainingId = Guid.NewGuid();
                if (!await AddTrainingInternalAsync(customerId, training, token)) return training;
            }
            else
                training.TrainingId = dbTraining.Id;
        }
        token = CancellationToken.None;
        if (dbTraining == null)
            dbTraining = await _database.RoosterTrainings.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Id == training.TrainingId);
        if (dbTraining == null)
        {
            if (!await AddTrainingInternalAsync(customerId, training, token)) return training;
            dbTraining = await _database.RoosterTrainings.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Id == training.TrainingId);
            if (dbTraining == null)
            {
                _logger.LogWarning("Two tries to add training {date} for user {userId} from customer {customerId} failed", training.DateStart, userId, customerId);
                return training;
            }
        }
        var available = await _database.RoosterAvailables.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.UserId == userId && x.TrainingId == dbTraining.Id);
        if (available == null)
        {
            if (!await AddAvailableInternalAsync(userId, customerId, training)) return training;
        }
        else if (!await PatchAvailableInternalAsync(available, training)) return training;
        training.Updated = true;
        return training;
    }

    public async Task<bool> PatchTraining(Guid customerId, EditTraining patchedTraining, CancellationToken token)
    {
        var oldTraining = await _database.RoosterTrainings.FindAsync(new object?[] { patchedTraining.Id }, cancellationToken: token);
        if (oldTraining == null) return false;
        if (patchedTraining.Name?.Length > DefaultSettingsHelper.MAX_LENGTH_TRAINING_TITLE)
            throw new DrogeCodeToLongException();
        DateTime dateStart = ((patchedTraining.Date ?? throw new ArgumentNullException("Date is null")) + (patchedTraining.TimeStart ?? throw new ArgumentNullException("TimeStart is null"))).ToUniversalTime();
        DateTime dateEnd = ((patchedTraining.Date ?? throw new ArgumentNullException("Date is null")) + (patchedTraining.TimeEnd ?? throw new ArgumentNullException("TimeEnd is null"))).ToUniversalTime();
        oldTraining.RoosterTrainingTypeId = patchedTraining.RoosterTrainingTypeId;
        oldTraining.Name = patchedTraining.Name;
        oldTraining.DateStart = dateStart;
        oldTraining.DateEnd = dateEnd;
        oldTraining.CountToTrainingTarget = patchedTraining.CountToTrainingTarget;
        oldTraining.Pin = patchedTraining.Pin;
        _database.RoosterTrainings.Update(oldTraining);
        return (await _database.SaveChangesAsync()) > 0;
    }

    public async Task<bool> AddTrainingAsync(Guid customerId, EditTraining newTraining, Guid trainingId, CancellationToken token)
    {
        DateTime dateStart = ((newTraining.Date ?? throw new ArgumentNullException("Date is null")) + (newTraining.TimeStart ?? throw new ArgumentNullException("TimeStart is null"))).ToUniversalTime();
        DateTime dateEnd = ((newTraining.Date ?? throw new ArgumentNullException("Date is null")) + (newTraining.TimeEnd ?? throw new ArgumentNullException("TimeEnd is null"))).ToUniversalTime();
        var training = new Training
        {
            TrainingId = trainingId,
            DefaultId = null,
            RoosterTrainingTypeId = newTraining.RoosterTrainingTypeId,
            Name = newTraining.Name,
            DateStart = dateStart,
            DateEnd = dateEnd,
            CountToTrainingTarget = newTraining.CountToTrainingTarget,
            Pin = newTraining.Pin,
        };
        return await AddTrainingInternalAsync(customerId, training, token);
    }

    private async Task<bool> AddTrainingInternalAsync(Guid customerId, Training training, CancellationToken token)
    {
        if (training.Name?.Length > DefaultSettingsHelper.MAX_LENGTH_TRAINING_TITLE)
            throw new DrogeCodeToLongException();
        token.ThrowIfCancellationRequested();
        token = CancellationToken.None;
        await _database.RoosterTrainings.AddAsync(new DbRoosterTraining
        {
            Id = training.TrainingId ?? throw new NoNullAllowedException("TrainingId is still null while adding new training"),
            RoosterDefaultId = training.DefaultId,
            CustomerId = customerId,
            RoosterTrainingTypeId = training.RoosterTrainingTypeId,
            Name = training.Name,
            DateStart = training.DateStart,
            DateEnd = training.DateEnd,
            CountToTrainingTarget = training.CountToTrainingTarget,
            Pin = training.Pin,
        });
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
            Available = training.Availabilty,
        });
        return (await _database.SaveChangesAsync()) > 0;
    }

    private async Task<bool> PatchAvailableInternalAsync(DbRoosterAvailable? available, Training training)
    {
        available!.Available = training.Availabilty;
        _database.RoosterAvailables.Update(available);
        return (await _database.SaveChangesAsync()) > 0;
    }

    public async Task<ScheduleForAllResponse> ScheduleForAllAsync(Guid userId, Guid customerId, int forMonth, int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, CancellationToken token)
    {
        var result = new ScheduleForAllResponse();
        var startDate = (new DateTime(yearStart, monthStart, dayStart, 0, 0, 0)).ToUniversalTime();
        var tillDate = (new DateTime(yearEnd, monthEnd, dayEnd, 23, 59, 59, 999)).ToUniversalTime();
        var users = _database.Users.Where(x => x.CustomerId == customerId && x.DeletedOn == null);
        var defaults = _database.RoosterDefaults.Where(x => x.CustomerId == customerId && x.ValidFrom <= startDate && x.ValidUntil >= startDate);
        var trainings = _database.RoosterTrainings.Where(x => x.CustomerId == customerId && x.DateStart >= startDate && x.DateStart <= tillDate).ToList();
        var availables = _database.RoosterAvailables.Include(i => i.User).Where(x => x.CustomerId == customerId && x.Date >= startDate && x.Date <= tillDate).ToList();

        var scheduleDate = DateOnly.FromDateTime(startDate);
        do
        {
            var defaultsFound = new List<Guid?>();
            var defaultsToday = defaults.Where(x => x.WeekDay == scheduleDate.DayOfWeek);
            var start = scheduleDate.ToDateTime(new TimeOnly(0, 0, 0, 0), DateTimeKind.Utc);
            var end = scheduleDate.ToDateTime(new TimeOnly(23, 59, 59, 999), DateTimeKind.Utc);
            var trainingsToday = trainings.Where(x => x.DateStart >= start && x.DateStart <= end).ToList();
            if (trainingsToday.Count > 0)
            {
                foreach (var training in trainingsToday)
                {
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
                        Pin = training.Pin,
                    };
                    foreach (var a in ava)
                    {
                        if (a == null) continue;
                        newPlanner.PlanUsers.Add(new PlanUser
                        {
                            UserId = a.UserId,
                            Availabilty = a.Available,
                            Assigned = a.Assigned,
                            Name = a.User?.Name ?? "Name not found",
                            PlannedFunctionId = a.UserFunctionId ?? users.FirstOrDefault(x => x.Id == a.UserId)?.UserFunctionId,
                            UserFunctionId = users.FirstOrDefault(x => x.Id == a.UserId)?.UserFunctionId,
                            VehicleId = a.VehicleId,
                        });
                        if (training.CountToTrainingTarget && scheduleDate.Month == forMonth && a.Assigned)
                        {
                            var indexUser = result.UserTrainingCounters.FindIndex(X => X.UserId.Equals(a.UserId));
                            if (indexUser >= 0)
                                result.UserTrainingCounters[indexUser].Count++;
                            else
                            {
                                result.UserTrainingCounters.Add(new UserTrainingCounter
                                {
                                    UserId = a.UserId,
                                    Count = 1
                                });
                            }
                        }
                    }
                    result.Planners.Add(newPlanner);
                    defaultsFound.Add(training.RoosterDefaultId);
                }
            }
            foreach (var def in defaultsToday)
            {
                if (!defaultsFound.Contains(def.Id))
                {
                    result.Planners.Add(new PlannedTraining
                    {
                        DefaultId = def.Id,
                        DateStart = scheduleDate.ToDateTime(def.TimeStart, DateTimeKind.Utc),
                        DateEnd = scheduleDate.ToDateTime(def.TimeEnd, DateTimeKind.Utc),
                        IsCreated = false,
                        RoosterTrainingTypeId = def.RoosterTrainingTypeId,
                        CountToTrainingTarget = def.CountToTrainingTarget,
                        Pin = false
                    });
                }
            }
            token.ThrowIfCancellationRequested();
            scheduleDate = scheduleDate.AddDays(1);

        } while (scheduleDate <= DateOnly.FromDateTime(tillDate));

        return result;
    }

    public async Task PatchAssignedUserAsync(Guid userId, Guid customerId, PatchAssignedUserRequest body, CancellationToken token)
    {
        if (body.User == null || body.TrainingId == null)
        {
            _logger.LogWarning("user is null {UserIsNull} or trainingId is null {TrainingIsNull}", body.User == null, body.TrainingId == null);
            return;
        }
        if (!_database.RoosterTrainings.Any(x => x.CustomerId == customerId && x.Id == body.TrainingId))
        {
            _logger.LogWarning("No training with '{Id}' found", body.TrainingId);
            return;
        }
        var ava = await _database.RoosterAvailables.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.TrainingId == body.TrainingId && x.UserId == body.User.UserId, cancellationToken: token);
        if (ava == null)
        {
            _logger.LogWarning("No ava with '{Id}' found for user '{User}'", body.TrainingId, body.User.UserId);
            return;
        }
        ava.Assigned = body.User.Assigned;
        ava.UserFunctionId = body.User.PlannedFunctionId;
        ava.VehicleId = body.User.VehicleId;
        _database.RoosterAvailables.Update(ava);
        await _database.SaveChangesAsync(token);
    }

    public async Task PutAssignedUserAsync(Guid userId, Guid customerId, OtherScheduleUserRequest body, CancellationToken token)
    {
        if (body.UserId == null || body.TrainingId == null || body.FunctionId == null)
        {
            _logger.LogWarning("userId is null {UserIsNull} or trainingId is null {TrainingIsNull} or functionId is null {FunctionId}", body.UserId == null, body.TrainingId == null, body.FunctionId == null);
            return;
        }
        var user = await _database.Users.FirstOrDefaultAsync(x => x.Id == userId && x.DeletedOn == null && x.CustomerId == customerId, cancellationToken: token);
        if (user == null)
        {
            _logger.LogWarning("No user with '{Id}' found", body.UserId);
            return;
        }
        var training = await _database.RoosterTrainings.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Id == body.TrainingId, cancellationToken: token);
        if (training == null)
        {
            _logger.LogWarning("No training with '{Id}' found", body.TrainingId);
            return;
        }
        var ava = await _database.RoosterAvailables.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.TrainingId == body.TrainingId && x.UserId == body.UserId, cancellationToken: token);
        if (ava == null)
        {
            if (body.Assigned)// only add to db if if assigned.
            {
                ava = new DbRoosterAvailable
                {
                    Id = Guid.NewGuid(),
                    UserId = body.UserId ?? Guid.Empty,
                    CustomerId = customerId,
                    TrainingId = body.TrainingId ?? Guid.Empty,
                    UserFunctionId = body.FunctionId,
                    Date = training.DateStart,
                    Available = Availabilty.None,
                    Assigned = body.Assigned
                };
                _database.RoosterAvailables.Add(ava);
                await _database.SaveChangesAsync(token);
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
            await _database.SaveChangesAsync(token);
        }
    }

    public async Task<GetScheduledTrainingsForUserResponse> GetScheduledTrainingsForUser(Guid userId, Guid customerId, DateTime fromDate, CancellationToken token)
    {
        var result = new GetScheduledTrainingsForUserResponse();
        var scheduled = await _database.RoosterAvailables.Include(i => i.Training).Where(x => x.CustomerId == customerId && x.UserId == userId && x.Assigned == true && x.Date >= fromDate).OrderBy(x => x.Date).ToListAsync(cancellationToken: token);
        foreach (var schedul in scheduled)
        {

            if (schedul.Training == null)
            {
                _logger.LogWarning("No training found for schedule '{ScheduleId}'", schedul.Id);
                continue;
            }
            result.Trainings.Add(new Training
            {
                TrainingId = schedul.TrainingId,
                DefaultId = schedul.TrainingId,
                Name = schedul.Training.Name,
                DateStart = schedul.Training.DateStart,
                DateEnd = schedul.Training.DateEnd,
                Availabilty = schedul.Available,
                Assigned = schedul.Assigned,
                RoosterTrainingTypeId = schedul.Training.RoosterTrainingTypeId,
                VehicleId = schedul.VehicleId,
                Pin = schedul.Training.Pin
            });
        }
        return result;
    }

    public async Task<GetPinnedTrainingsForUserResponse> GetPinnedTrainingsForUser(Guid userId, Guid customerId, DateTime fromDate, CancellationToken token)
    {
        var result = new GetPinnedTrainingsForUserResponse();
        var trainings = await _database.RoosterTrainings.Include(i => i.RoosterAvailables!.Where(r => r.CustomerId == customerId && r.UserId == userId)).Where(x => x.CustomerId == customerId && x.Pin && x.DateStart >= fromDate && (x.RoosterAvailables == null || !x.RoosterAvailables.Any(r => r.Available > 0))).OrderBy(x => x.DateStart).ToListAsync(cancellationToken: token);
        foreach (var training in trainings)
        {
            var availabilty = training.RoosterAvailables?.FirstOrDefault(r => r.CustomerId == customerId && r.UserId == userId);
            result.Trainings.Add(new Training
            {
                TrainingId = training.Id,
                DefaultId = training.RoosterDefaultId,
                Name = training.Name,
                DateStart = training.DateStart,
                DateEnd = training.DateEnd,
                Availabilty = availabilty?.Available,
                Assigned = availabilty?.Assigned ?? false,
                RoosterTrainingTypeId = training.RoosterTrainingTypeId,
                VehicleId = availabilty?.VehicleId,
                CountToTrainingTarget = training.CountToTrainingTarget,
                Pin = training.Pin,
            });
        }
        return result;
    }

    public async Task<List<PlannerTrainingType>> GetTrainingTypes(Guid customerId, CancellationToken token)
    {
        var result = new List<PlannerTrainingType>();
        var typesFromDb = await _database.RoosterTrainingTypes.Where(x => x.CustomerId == customerId).ToListAsync(cancellationToken: token);
        foreach (var type in typesFromDb)
        {
            var newType = new PlannerTrainingType
            {
                Id = type.Id,
                Name = type.Name,
                CountToTrainingTarget = type.CountToTrainingTarget,
                IsDefault = type.IsDefault,
                Order = type.Order,
            };
            if (!string.IsNullOrEmpty(type.ColorLight))
                newType.ColorLight = type.ColorLight;
            if (!string.IsNullOrEmpty(type.ColorDark))
                newType.ColorDark = type.ColorDark;
            result.Add(newType);
        }
        return result;
    }
}
