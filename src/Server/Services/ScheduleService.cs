using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
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

    public async Task<ScheduleForUserResponse> ScheduleForUserAsync(Guid userId, Guid customerId, int relativeWeek)
    {
        var result = new ScheduleForUserResponse();
        var d = DateTime.UtcNow.StartOfWeek(DayOfWeek.Monday).AddDays(relativeWeek * 7);
        var startDate = DateOnly.FromDateTime(d);
        var tillDate = DateOnly.FromDateTime(d.AddDays(6));
        var defaults = _database.RoosterDefaults.Where(x => x.CustomerId == customerId).ToList();
        var trainings = _database.RoosterTrainings.Where(x => x.CustomerId == customerId && x.Date > startDate && x.Date < tillDate).ToList();
        var availables = _database.RoosterAvailables.Where(x => x.CustomerId == customerId && x.UserId == userId).ToList();

        var scheduleDate = startDate;
        do
        {
            var defaultsToday = defaults.Where(x => x.WeekDay == scheduleDate.DayOfWeek);
            var trainingsToday = trainings.Where(x => x.Date == scheduleDate).ToList();
            if (trainingsToday.Count > 0)
            {
                // ToDo
                // 
            }
            foreach(var def in defaultsToday)
            {
                if (true)//only if no training is created for this default
                {
                    result.Trainings.Add(new Training
                    {
                        DefaultId = def.Id,
                        Date = scheduleDate,
                        StartTime = scheduleDate.ToDateTime(def.StartTime),
                        EndTime = scheduleDate.ToDateTime(def.EndTime),
                        Availabilty = Availabilty.None, //ToDo
                    });
                }
            }
            scheduleDate = scheduleDate.AddDays(1);

        } while (scheduleDate <= tillDate);

        return result;
    }
}
