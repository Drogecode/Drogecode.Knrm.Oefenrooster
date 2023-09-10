using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Exceptions;
using Drogecode.Knrm.Oefenrooster.Shared.Extensions;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
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
    public async Task<GetAllDefaultGroupsResponse> GetAlldefaultGroupsForUser(Guid customerId, Guid userId)
    {

        var sw = Stopwatch.StartNew();
        var response = new GetAllDefaultGroupsResponse();
        var list = new List<DefaultGroup>();

        var dbGroups = await _database.UserDefaultGroups.Where(x => x.CustomerId == customerId && x.UserId == userId).ToListAsync();
        foreach (var dbGroup in dbGroups)
        {
            list.Add(dbGroup.ToDefaultGroups());
        }
        if (!list.Any(x => x.IsDefault))
        {
            var newDefaultGroup = new DbUserDefaultGroup
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                UserId = userId,
                IsDefault = true,
                ValidFrom = _dateTimeService.UtcNow(),
                ValidUntil = DateTime.MaxValue,
            };
            _database.UserDefaultGroups.Add(newDefaultGroup);
            await _database.SaveChangesAsync();
            list.Add(newDefaultGroup.ToDefaultGroups());
        }

        response.Groups = list;
        response.TotalCount = list.Count;
        response.Success = list.Count > 0;
        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }

    public async Task<MultipleDefaultSchedulesResponse> GetAlldefaultsForUser(Guid customerId, Guid userId, Guid groupId)
    {
        var sw = Stopwatch.StartNew();
        var response = new MultipleDefaultSchedulesResponse();
        var list = new List<DefaultSchedule>();
        var group = await _database.UserDefaultGroups.FindAsync(groupId);
        var groupIsDefault = group?.IsDefault ?? false;
        var dbDefaults = _database.RoosterDefaults.Include(x => x.UserDefaultAvailables.Where(y => y.CustomerId == customerId && y.UserId == userId && (y.DefaultGroupId == groupId || (groupIsDefault && y.DefaultGroupId == null))));
        foreach (var dbDefault in dbDefaults)
        {
            if (dbDefault is null) continue;
            var userDefaults = dbDefault?.UserDefaultAvailables?.Where(x => x.UserId == userId && x.ValidUntil >= _dateTimeService.UtcNow()).OrderBy(x => x.ValidUntil);
            var innerList = new List<DefaultUserSchedule>();
            if (userDefaults is not null)
            {
                foreach (var userDefault in userDefaults)
                {
                    if (userDefault is null) continue;
                    innerList.Add(new DefaultUserSchedule
                    {
                        UserDefaultAvailableId = userDefault.Id,
                        GroupId = userDefault.DefaultGroupId,
                        Available = userDefault.Available,
                        ValidFromUser = userDefault.ValidFrom,
                        ValidUntilUser = userDefault.ValidUntil,
                        Assigned = userDefault.Assigned,
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
        response.DefaultSchedules = list;
        response.TotalCount = list.Count;
        response.Success = list.Count > 0;
        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
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
        DbRoosterDefault? dbDefault = DbQuery(body, userId);
        if (dbDefault is null) return new PatchDefaultScheduleForUserResponse { Success = false };
        var userDefault = dbDefault.UserDefaultAvailables?.FirstOrDefault(y => y.UserId == userId && y.Id == body.UserDefaultAvailableId);
        var validFrom = body.ValidFromUser ?? _dateTimeService.UtcNow();
        var validUntil = body.ValidUntilUser ?? DateTime.MaxValue;
        if (validFrom.Kind == DateTimeKind.Unspecified)
            validFrom = validFrom.DateTimeWithZone(dbDefault.Customer.TimeZone).ToUniversalTime();
        if (validUntil.Kind == DateTimeKind.Unspecified)
            validUntil = validUntil.DateTimeWithZone(dbDefault.Customer.TimeZone).ToUniversalTime();

        if (userDefault?.ValidFrom?.Date.CompareTo(_dateTimeService.UtcNow().Date) >= 0)
        {
            userDefault.DefaultGroupId = body.GroupId;
            userDefault!.Available = body.Available;
            userDefault.Assigned = body.Assigned;
            userDefault.ValidFrom = validFrom;
            userDefault.ValidUntil = validUntil;
            _database.UserDefaultAvailables.Update(userDefault);
        }
        else
        {
            if (userDefault is not null)
            {
                userDefault.ValidUntil = _dateTimeService.UtcNow().AddDays(-1);
                userDefault.DefaultGroupId = body.GroupId;
                _database.UserDefaultAvailables.Update(userDefault);
                validFrom = _dateTimeService.UtcNow();
            }
            userDefault = new DbUserDefaultAvailable
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CustomerId = customerId,
                DefaultGroupId = body.GroupId,
                RoosterDefaultId = body.DefaultId,
                Available = body.Available,
                Assigned = body.Assigned,
                ValidFrom = validFrom,
                ValidUntil = validUntil
            };
            _database.UserDefaultAvailables.Add(userDefault);
        }
        _database.SaveChanges();
        body.UserDefaultAvailableId = userDefault.Id;
        var dbPatched = DbQuery(body, userId);
        var patched = dbPatched?.ToPatchDefaultUserSchedule(userId, userDefault.Id);
        return new PatchDefaultScheduleForUserResponse
        {
            Success = true,
            Patched = patched
        };

        DbRoosterDefault? DbQuery(PatchDefaultUserSchedule body, Guid userId)
        {
            return _database.RoosterDefaults
                .Include(x => x.UserDefaultAvailables!.Where(y => y.UserId == userId && y.Id == body.UserDefaultAvailableId))
                .Include(c => c.Customer)
                .FirstOrDefault(z => z.Id == body.DefaultId);
        }
    }
}
