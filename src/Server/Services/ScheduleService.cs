using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Graph;
using MudBlazor.Extensions;
using System.Data;

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

    public async Task<ScheduleForUserResponse> ScheduleForUserAsync(Guid userId, Guid customerId, int relativeWeek, CancellationToken token)
    {
        var result = new ScheduleForUserResponse();
        var d = DateTime.UtcNow.StartOfWeek(System.DayOfWeek.Monday).AddDays(relativeWeek * 7);
        var startDate = DateOnly.FromDateTime(d);
        var tillDate = DateOnly.FromDateTime(d.AddDays(6));
        var defaults = _database.RoosterDefaults.Where(x => x.CustomerId == customerId).ToList();
        var trainings = _database.RoosterTrainings.Where(x => x.CustomerId == customerId && x.Date >= startDate && x.Date <= tillDate).ToList();
        var availables = _database.RoosterAvailables.Where(x => x.CustomerId == customerId && x.UserId == userId && x.Date >= startDate && x.Date <= tillDate).ToList();

        var scheduleDate = startDate;
        do
        {
            var defaultsFound = new List<Guid?>();
            var defaultsToday = defaults.Where(x => x.WeekDay == scheduleDate.DayOfWeek);
            var trainingsToday = trainings.Where(x => x.Date == scheduleDate).ToList();
            if (trainingsToday.Count > 0)
            {
                foreach (var training in trainingsToday)
                {
                    var ava = availables.FirstOrDefault(x => x.TrainingId == training.Id);
                    result.Trainings.Add(new Training
                    {
                        DefaultId = training.RoosterDefaultId,
                        TrainingId = training.Id,
                        Date = training.Date,
                        Availabilty = ava?.Available,
                        StartTime = training.StartTime,
                        EndTime = training.EndTime,
                        Assigned = ava?.Assigned ?? false,
                    });
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
                        Date = scheduleDate,
                        StartTime = scheduleDate.ToDateTime(def.StartTime, DateTimeKind.Utc),
                        EndTime = scheduleDate.ToDateTime(def.EndTime, DateTimeKind.Utc),
                        Availabilty = Availabilty.None,
                    });
                }
            }
            token.ThrowIfCancellationRequested();
            scheduleDate = scheduleDate.AddDays(1);

        } while (scheduleDate <= tillDate);

        return result;
    }

    public async Task<Training> PatchTrainingAsync(Guid userId, Guid customerId, Training training, CancellationToken token)
    {
        training.Updated = false;
        DbRoosterTraining? dbTraining = null;
        if (training.TrainingId == null || training.TrainingId == Guid.Empty)
        {
            dbTraining = await _database.RoosterTrainings.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.RoosterDefaultId == training.DefaultId && x.Date == training.Date && x.StartTime == training.StartTime && x.EndTime == training.EndTime);
            if (dbTraining == null)
            {
                training.TrainingId = Guid.NewGuid();
                token.ThrowIfCancellationRequested();
                if (!await AddTrainingInternalAsync(customerId, training)) return training;
            }
            else
                training.TrainingId = dbTraining.Id;
        }
        token = CancellationToken.None;
        if (dbTraining == null)
            dbTraining = await _database.RoosterTrainings.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Id == training.TrainingId);
        if (dbTraining == null)
        {
            if (!await AddTrainingInternalAsync(customerId, training)) return training;
            dbTraining = await _database.RoosterTrainings.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Id == training.TrainingId);
            if (dbTraining == null)
            {
                _logger.LogWarning("Two tries to add training {date} for user {userId} from customer {customerId} failed", training.Date, userId, customerId);
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

    private async Task<bool> AddTrainingInternalAsync(Guid customerId, Training training)
    {
        await _database.RoosterTrainings.AddAsync(new DbRoosterTraining
        {
            Id = training.TrainingId ?? throw new NoNullAllowedException("TrainingId is still null while adding new training"),
            RoosterDefaultId = training.DefaultId,
            CustomerId = customerId,
            Date = training.Date,
            StartTime = training.StartTime,
            EndTime = training.EndTime,
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
            Date = training.Date,
            Available = training.Availabilty
        });
        return (await _database.SaveChangesAsync()) > 0;
    }

    private async Task<bool> PatchAvailableInternalAsync(DbRoosterAvailable? available, Training training)
    {
        available!.Available = training.Availabilty;
        _database.RoosterAvailables.Update(available);
        return (await _database.SaveChangesAsync()) > 0;
    }

    public async Task<ScheduleForAllResponse> ScheduleForAllAsync(Guid userId, Guid customerId, int relativeWeek, CancellationToken token)
    {
        var result = new ScheduleForAllResponse();
        var d = DateTime.UtcNow.StartOfWeek(System.DayOfWeek.Monday).AddDays(relativeWeek * 7);
        var startDate = DateOnly.FromDateTime(d);
        var tillDate = DateOnly.FromDateTime(d.AddDays(6));
        var users = _database.Users.Where(x=> x.CustomerId == customerId && x.DeletedOn == null);
        var defaults = _database.RoosterDefaults.Where(x => x.CustomerId == customerId).ToList();
        var trainings = _database.RoosterTrainings.Where(x => x.CustomerId == customerId && x.Date >= startDate && x.Date <= tillDate).ToList();
        var availables = _database.RoosterAvailables.Include(i => i.User).Where(x => x.CustomerId == customerId && x.Date >= startDate && x.Date <= tillDate).ToList();

        var scheduleDate = startDate;
        do
        {
            var defaultsFound = new List<Guid?>();
            var defaultsToday = defaults.Where(x => x.WeekDay == scheduleDate.DayOfWeek);
            var trainingsToday = trainings.Where(x => x.Date == scheduleDate).ToList();
            if (trainingsToday.Count > 0)
            {
                foreach (var training in trainingsToday)
                {
                    var ava = availables.FindAll(x => x.TrainingId == training.Id);
                    var newPlanner = new Shared.Models.Schedule.Planner
                    {
                        DefaultId = training.RoosterDefaultId,
                        TrainingId = training.Id,
                        Date = training.Date,
                        StartTime = training.StartTime,
                        EndTime = training.EndTime,
                        IsCreated = true,
                    };
                    foreach (var a in ava)
                    {
                        if (a == null ) continue;
                        newPlanner.PlanUsers.Add(new PlanUser
                        {
                            UserId = a.UserId,
                            Availabilty = a.Available,
                            Assigned = a.Assigned,
                            Name = a.User?.Name ?? "Name not found",
                            UserFunctionId = a.UserFunctionId ?? users.FirstOrDefault(x=>x.Id == a.UserId)?.UserFunctionId,
                        }) ;
                    }
                    result.Planners.Add(newPlanner);
                    defaultsFound.Add(training.RoosterDefaultId);
                }
            }
            foreach (var def in defaultsToday)
            {
                if (!defaultsFound.Contains(def.Id))
                {
                    result.Planners.Add(new Shared.Models.Schedule.Planner
                    {
                        DefaultId = def.Id,
                        Date = scheduleDate,
                        StartTime = scheduleDate.ToDateTime(def.StartTime, DateTimeKind.Utc),
                        EndTime = scheduleDate.ToDateTime(def.EndTime, DateTimeKind.Utc),
                        IsCreated = false
                    });
                }
            }
            token.ThrowIfCancellationRequested();
            scheduleDate = scheduleDate.AddDays(1);

        } while (scheduleDate <= tillDate);

        return result;
    }

    public async Task PatchScheduleUserAsync(Guid userId, Guid customerId, PatchScheduleUserRequest body, CancellationToken token)
    {
        if (body.User == null || body.TrainingId == null)
        {
            _logger.LogWarning("user is null {UserIsNull} or trainingId is null {TrainingIsNull}", body.User == null, body.TrainingId == null);
            return;
        }
        var training = await _database.RoosterTrainings.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Id == body.TrainingId, cancellationToken: token);
        if (training == null)
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
        _database.RoosterAvailables.Update(ava);
        await _database.SaveChangesAsync(token);
    }

    public async Task<GetScheduledTrainingsForUserResponse> GetScheduledTrainingsForUser(Guid userId, Guid customerId, DateOnly fromDate, CancellationToken token)
    {
        var result = new GetScheduledTrainingsForUserResponse();
        var scheduled = await _database.RoosterAvailables.Include(i => i.Training).Where(x => x.CustomerId == customerId && x.UserId == userId && x.Assigned == true && x.Date >= fromDate).ToListAsync(cancellationToken: token);
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
                DefaultId= schedul.TrainingId,
                Date = schedul.Date,
                StartTime = schedul.Training.StartTime,
                EndTime = schedul.Training.EndTime,
                Availabilty = schedul.Available,
                Assigned= schedul.Assigned,
            });
        }
        return result;
    }
}
