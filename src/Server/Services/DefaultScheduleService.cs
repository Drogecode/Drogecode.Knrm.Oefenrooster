using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Exceptions;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.Graph.Models.ODataErrors;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class DefaultScheduleService : IDefaultScheduleService
{
    private readonly ILogger<DefaultScheduleService> _logger;
    private readonly IDateTimeService _dateTimeService;
    private readonly Database.DataContext _database;
    public DefaultScheduleService(ILogger<DefaultScheduleService> logger, IDateTimeService dateTimeService, Database.DataContext database)
    {
        _logger = logger;
        _dateTimeService = dateTimeService;
        _database = database;
    }

    public async Task<List<DefaultSchedule>> GetAlldefaultsForUser(Guid customerId, Guid userId)
    {
        var list = new List<DefaultSchedule>();
        var dbDefaults = _database.RoosterDefaults.Include(x => x.UserDefaultAvailables.Where(y => y.UserId == userId));
        foreach (var dbDefault in dbDefaults)
        {
            if (dbDefault is null) continue;
            var userDefaults = dbDefault?.UserDefaultAvailables?.Where(x => x.UserId == userId /*&& x.ValidFrom > _dateTimeService.UtcNow()*/).OrderBy(x => x.ValidFrom);
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

    public async Task<PutDefaultScheduleResponse> PutDefaultSchedule(DefaultSchedule body, Guid customerId, Guid userId)
    {
        var sw = Stopwatch.StartNew();
        var result = new PutDefaultScheduleResponse();

        var dbDefault = await _database.RoosterDefaults.FirstOrDefaultAsync(x => x.Id == body.Id);
        if (dbDefault is not null)
        {
            sw.Stop();
            result.Success = false;
            result.Error = PutDefaultScheduleError.IdAlreadyExists;
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return result;
        }
        var dbCustomer = await _database.Customers.FindAsync(customerId) ?? throw new DrogeCodeNullException($"Customer {customerId} not found");
        dbDefault = body.ToDbRoosterDefault(customerId, dbCustomer.TimeZone);
        dbDefault.Id = Guid.NewGuid();
        await _database.RoosterDefaults.AddAsync(dbDefault);
        result.Success = (await _database.SaveChangesAsync()) >= 1;
        result.DefaultSchedule = dbDefault.ToDefaultSchedule();
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<PatchDefaultScheduleForUserResponse> PatchDefaultScheduleForUser(PatchDefaultUserSchedule body, Guid customerId, Guid userId)
    {
        var dbDefault = _database.RoosterDefaults.Include(x => x.UserDefaultAvailables!.Where(y => y.UserId == userId && y.Id == body.UserDefaultAvailableId)).FirstOrDefault(z => z.Id == body.DefaultId);
        if (dbDefault is null) return new PatchDefaultScheduleForUserResponse { Success = false };
        var userDefault = dbDefault.UserDefaultAvailables?.FirstOrDefault(y => y.UserId == userId && y.Id == body.UserDefaultAvailableId);
        if (userDefault?.ValidFrom?.Date.Equals(_dateTimeService.UtcNow().Date) == true)
        {
            userDefault!.Available = body.Available;
            userDefault.Assigned = body.Assigned;
            userDefault.ValidFrom = _dateTimeService.UtcNow();
            userDefault.ValidUntil = DateTime.MaxValue;
            _database.UserDefaultAvailables.Update(userDefault);
        }
        else
        {
            if (userDefault is not null)
            {
                userDefault.ValidUntil = _dateTimeService.UtcNow().AddDays(-1);
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
                ValidFrom = _dateTimeService.UtcNow(),
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
