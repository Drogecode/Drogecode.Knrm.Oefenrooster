using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class DefaultScheduleService : IDefaultScheduleService
{
    private readonly ILogger<DefaultScheduleService> _logger;
    private readonly Database.DataContext _database;
    public DefaultScheduleService(ILogger<DefaultScheduleService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<List<DefaultSchedule>> GetAlldefaultsForUser(Guid customerId, Guid userId)
    {
        var list = new List<DefaultSchedule>();
        var dbDefaults = _database.RoosterDefaults.Include(x => x.UserDefaultAvailables.Where(y => y.UserId == userId));
        foreach (var dbDefault in dbDefaults)
        {
            if (dbDefault is null) continue;
            var userDefault = dbDefault?.UserDefaultAvailables?.FirstOrDefault(x => x.UserId == userId);
            list.Add(new DefaultSchedule
            {
                Id = dbDefault!.Id,
                RoosterTrainingTypeId = dbDefault.RoosterTrainingTypeId,
                UserDefaultAvailableId = userDefault?.Id,
                WeekDay = dbDefault.WeekDay,
                Available = userDefault?.Available,
                TimeStart = dbDefault.TimeStart,
                TimeEnd = dbDefault.TimeEnd,
                ValidFromDefault = dbDefault.ValidFrom,
                ValidUntilDefault = dbDefault.ValidUntil,
                ValidFromUser = userDefault?.ValidFrom,
                ValidUntilUser = userDefault?.ValidUntil,
                CountToTrainingTarget = dbDefault.CountToTrainingTarget,
                Assigned = userDefault?.Assigned ?? false
            });
        }
        return list;
    }

    public async Task<PatchDefaultScheduleForUserResponse> PatchDefaultScheduleForUser(DefaultSchedule body, Guid customerId, Guid userId)
    {
        var dbDefault = _database.RoosterDefaults.Include(x => x.UserDefaultAvailables.Where(y => y.UserId == userId))?.FirstOrDefault(x => x.Id == body.Id);
        if (dbDefault is null || body.ValidFromUser is null || body.ValidUntilUser is null) return new PatchDefaultScheduleForUserResponse { Success = false };
        var userDefault = dbDefault.UserDefaultAvailables?.FirstOrDefault(y => y.UserId == userId);
        if (userDefault?.ValidFrom?.Date.Equals(DateTime.UtcNow.Date) == true)
        {
            userDefault!.Available = body.Available;
            userDefault.Assigned = body.Assigned;
            userDefault.ValidFrom = DateTime.UtcNow;
            userDefault.ValidUntil = DateTime.MaxValue;
            _database.UserDefaultAvailables.Update(userDefault);
            _database.SaveChanges();
        }
        else
        {
            if (userDefault is not null)
            {
                userDefault.ValidUntil = DateTime.UtcNow.AddDays(-1);
                _database.UserDefaultAvailables.Update(userDefault);
            }
            userDefault = new DbUserDefaultAvailable
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CustomerId = customerId,
                RoosterDefaultId = body.Id,
                Available = body.Available,
                Assigned = body.Assigned,
                ValidFrom = DateTime.UtcNow,
                ValidUntil = DateTime.MaxValue
            };
            _database.UserDefaultAvailables.Add(userDefault);
        }
        _database.SaveChanges();
        body.UserDefaultAvailableId = userDefault.Id;
        return new PatchDefaultScheduleForUserResponse
        {
            Success = true,
            Patched = body
        };
    }
}
