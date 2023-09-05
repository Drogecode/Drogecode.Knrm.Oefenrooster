using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Microsoft.Graph.Models.ODataErrors;

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
            var userDefaults = dbDefault?.UserDefaultAvailables?.Where(x => x.UserId == userId /*&& x.ValidFrom > DateTime.UtcNow*/).OrderBy(x=>x.ValidFrom);
            var innerList = new List<DefaultUserSchedule>();
            if (userDefaults is not null)
            {
                foreach (var userDefault in userDefaults)
                {
                    innerList.Add(new DefaultUserSchedule
                    {
                        UserDefaultAvailableId = userDefault?.Id,
                        Available = userDefault?.Available,
                        ValidFromUser = userDefault?.ValidFrom,
                        ValidUntilUser = userDefault?.ValidUntil,
                        Assigned = userDefault?.Assigned ?? false,
                    });
                }
            }
            var defaultSchedule = new DefaultSchedule
            {
                Id = dbDefault!.Id,
                RoosterTrainingTypeId = dbDefault.RoosterTrainingTypeId,
                WeekDay = dbDefault.WeekDay,
                TimeStart = dbDefault.TimeStart,
                TimeEnd = dbDefault.TimeEnd,
                ValidFromDefault = dbDefault.ValidFrom,
                ValidUntilDefault = dbDefault.ValidUntil,
                CountToTrainingTarget = dbDefault.CountToTrainingTarget,
                Order = dbDefault.Order,
                UserSchedules = innerList,
            };
            list.Add(defaultSchedule);
        }
        return list;
    }

    public async Task<PatchDefaultScheduleForUserResponse> PatchDefaultScheduleForUser(PatchDefaultUserSchedule body, Guid customerId, Guid userId)
    {
        var dbDefault = _database.RoosterDefaults.Include(x => x.UserDefaultAvailables!.Where(y => y.UserId == userId && x.Id == body.UserDefaultAvailableId))?.FirstOrDefault(x => x.Id == body.DefaultId);
        if (dbDefault is null) return new PatchDefaultScheduleForUserResponse { Success = false };
        var userDefault = dbDefault.UserDefaultAvailables?.FirstOrDefault(y => y.UserId == userId && y.Id == body.UserDefaultAvailableId);
        if (userDefault?.ValidFrom?.Date.Equals(DateTime.UtcNow.Date) == true)
        {
            userDefault!.Available = body.Available;
            userDefault.Assigned = body.Assigned;
            userDefault.ValidFrom = DateTime.UtcNow;
            userDefault.ValidUntil = DateTime.MaxValue;
            _database.UserDefaultAvailables.Update(userDefault);
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
                RoosterDefaultId = body.DefaultId,
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
