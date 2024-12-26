using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Exceptions;
using Drogecode.Knrm.Oefenrooster.Shared.Extensions;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Errors;
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

    public async Task<GetAllDefaultGroupsResponse> GetAllDefaultGroupsForUser(Guid customerId, Guid userId)
    {
        var sw = Stopwatch.StartNew();
        var response = new GetAllDefaultGroupsResponse();
        var list = new List<DefaultGroup>();

        var dbGroups = await _database.UserDefaultGroups
            .Where(x => x.CustomerId == customerId && x.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
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
            var currentDefaults = await _database.UserDefaultAvailables.Where(x => x.CustomerId == customerId && x.UserId == userId && x.DefaultGroupId == null).ToListAsync();
            foreach (var current in currentDefaults)
            {
                current.DefaultGroupId = newDefaultGroup.Id;
                _database.UserDefaultAvailables.Update(current);
            }

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

    public async Task<MultipleDefaultSchedulesResponse> GetAllDefaultsForUser(Guid customerId, Guid userId, Guid groupId)
    {
        var sw = Stopwatch.StartNew();
        var response = new MultipleDefaultSchedulesResponse();
        var list = new List<DefaultSchedule>();
        var group = await _database.UserDefaultGroups.FindAsync(groupId);
        var groupIsDefault = group?.IsDefault ?? false;
        var dbDefaults = await _database.RoosterDefaults
            .Where(x => x.CustomerId == customerId && x.ValidFrom <= DateTime.UtcNow && x.ValidUntil >= DateTime.UtcNow)
            .Include(x => x.UserDefaultAvailables!.Where(y => y.CustomerId == customerId && y.UserId == userId && y.DefaultGroupId == groupId))
            .AsNoTracking()
            .ToListAsync();
        foreach (var dbDefault in dbDefaults)
        {
            var userDefaults = dbDefault.UserDefaultAvailables?.Where(x => x.CustomerId == customerId && x.UserId == userId && x.ValidUntil >= _dateTimeService.UtcNow() && x.DefaultGroupId == groupId)
                .OrderBy(x => x.ValidUntil);
            var innerList = new List<DefaultUserSchedule>();
            if (userDefaults is not null)
            {
                foreach (var userDefault in userDefaults)
                {
                    innerList.Add(new DefaultUserSchedule
                    {
                        UserDefaultAvailableId = userDefault.Id,
                        GroupId = userDefault.DefaultGroupId,
                        Available = (Availabilty)(int)(userDefault.Available ?? Availability.None), //ToDo Remove when all users on v0.3.50 or above
                        Availability = userDefault.Available,
                        ValidFromUser = userDefault.ValidFrom,
                        ValidUntilUser = userDefault.ValidUntil,
                        Assigned = userDefault.Assigned,
                    });
                }
            }

            var defaultSchedule = dbDefault.ToDefaultSchedule(innerList);
            list.Add(defaultSchedule);
        }

        response.DefaultSchedules = list;
        response.TotalCount = list.Count;
        response.Success = list.Count > 0;
        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }

    public async Task<PutGroupResponse> PutGroup(DefaultGroup body, Guid customerId, Guid userId)
    {
        var sw = Stopwatch.StartNew();
        var result = new PutGroupResponse();

        var dbDefault = await _database.UserDefaultGroups.FirstOrDefaultAsync(x => x.Id == body.Id);
        if (dbDefault is not null)
        {
            sw.Stop();
            result.Success = false;
            result.Error = PutError.IdAlreadyExists;
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return result;
        }

        dbDefault = body.ToDbUserDefaultGroup(customerId, userId);
        dbDefault.Id = Guid.NewGuid();
        await _database.UserDefaultGroups.AddAsync(dbDefault);
        result.Success = (await _database.SaveChangesAsync()) >= 1;
        result.Group = dbDefault.ToDefaultGroups();


        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
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
            result.Error = PutError.IdAlreadyExists;
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return result;
        }

        var dbCustomer = await _database.Customers.FindAsync(customerId) ?? throw new DrogeCodeNullException($"Customer {customerId} not found");
        dbDefault = body.ToDbRoosterDefault(customerId, dbCustomer.TimeZone);
        dbDefault.Id = Guid.NewGuid();
        await _database.RoosterDefaults.AddAsync(dbDefault);
        result.Success = (await _database.SaveChangesAsync()) >= 1;
        result.NewId = dbDefault.Id;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<PatchDefaultScheduleResponse> PatchDefaultSchedule(DefaultSchedule body, Guid customerId, Guid userId)
    {
        var sw = Stopwatch.StartNew();
        var result = new PatchDefaultScheduleResponse();

        var dbDefault = await _database.RoosterDefaults.FirstOrDefaultAsync(x => x.Id == body.Id);
        if (dbDefault is null)
        {
            sw.Stop();
            result.Success = false;
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return result;
        }

        var dbCustomer = await _database.Customers.FindAsync(customerId) ?? throw new DrogeCodeNullException($"Customer {customerId} not found");
        dbDefault.Name = body.Name;
        dbDefault.TimeStart = TimeOnly.FromTimeSpan(body.TimeStart ?? throw new ArgumentNullException("TimeStart is null"));
        dbDefault.TimeEnd = TimeOnly.FromTimeSpan(body.TimeEnd ?? throw new ArgumentNullException("TimeEnd is null"));
        dbDefault.ValidFrom = (body.ValidFromDefault ?? throw new ArgumentNullException("ValidFromDefault is null")).ToUniversalTime();
        dbDefault.ValidUntil = (body.ValidUntilDefault ?? throw new ArgumentNullException("ValidUntil is null")).ToUniversalTime();
        dbDefault.WeekDay = body.WeekDay;
        dbDefault.Order = body.Order;
        dbDefault.RoosterTrainingTypeId = body.RoosterTrainingTypeId;
        dbDefault.CountToTrainingTarget = body.CountToTrainingTarget;
        dbDefault.VehicleIds = body.VehicleIds;
        _database.RoosterDefaults.Update(dbDefault);
        result.Success = (await _database.SaveChangesAsync()) >= 1;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<GetAllDefaultScheduleResponse> GetAllDefaultSchedule(Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new GetAllDefaultScheduleResponse
        {
            DefaultSchedules = await _database.RoosterDefaults.Where(x => x.CustomerId == customerId).OrderBy(x => x.Order).Select(x => x.ToDefaultSchedule(null)).ToListAsync(clt)
        };
        result.Success = result.DefaultSchedules is not null && result.DefaultSchedules.Any();
        result.TotalCount = result.DefaultSchedules?.Count ?? -2;

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<PatchDefaultScheduleForUserResponse> PatchDefaultScheduleForUser(PatchDefaultUserSchedule body, Guid customerId, Guid userId)
    {
        var sw = Stopwatch.StartNew();
        var result = new PatchDefaultScheduleForUserResponse();
        DbRoosterDefault? dbDefault = DbQuery(body, userId);
        body.Availability ??= (Availability)(int)(body.Available ?? Availabilty.None); //ToDo Remove when all users on v0.3.50 or above

        var dbGroup = await _database.UserDefaultGroups.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.UserId == userId && x.Id == body.GroupId && !x.IsDefault);
        if (dbDefault is null)
        {
            sw.Stop();
            result.Success = false;
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return result;
        }

        var userDefault = dbDefault.UserDefaultAvailables?.FirstOrDefault(y => y.UserId == userId && y.Id == body.UserDefaultAvailableId && y.DefaultGroupId == body.GroupId);
        var allActiveUserDefaults = await _database.UserDefaultAvailables.Where(y =>
                y.UserId == userId && y.DefaultGroupId == body.GroupId && y.RoosterDefaultId == body.DefaultId && y.ValidFrom <= _dateTimeService.UtcNow() && y.ValidUntil >= _dateTimeService.UtcNow())
            .ToListAsync();
        var validFrom = dbGroup?.ValidFrom ?? body.ValidFromUser ?? _dateTimeService.UtcNow();
        var validUntil = dbGroup?.ValidUntil ?? body.ValidUntilUser ?? DateTime.MaxValue;
        if (validFrom.Kind == DateTimeKind.Unspecified)
            validFrom = validFrom.DateTimeWithZone(dbDefault.Customer.TimeZone).ToUniversalTime();
        if (validUntil.Kind == DateTimeKind.Unspecified)
            validUntil = validUntil.DateTimeWithZone(dbDefault.Customer.TimeZone).ToUniversalTime();

        if (allActiveUserDefaults.Count > 1)
        {
            foreach (var wrongDefault in allActiveUserDefaults)
            {
                if (wrongDefault.Id.Equals(body.UserDefaultAvailableId)) continue;
                wrongDefault.ValidUntil = _dateTimeService.UtcNow().AddDays(-1);
                _database.UserDefaultAvailables.Update(wrongDefault);
                _logger.LogWarning("Fixed wrong default rooster with Id `{Id}` for `{User}`", wrongDefault.Id, userId);
            }
        }

        if (userDefault?.ValidFrom?.Date.CompareTo(_dateTimeService.UtcNow().Date) >= 0)
        {
            userDefault.DefaultGroupId = body.GroupId;
            userDefault.Available = body.Availability;
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
                Available = body.Availability,
                Assigned = body.Assigned,
                ValidFrom = validFrom,
                ValidUntil = validUntil
            };
            _database.UserDefaultAvailables.Add(userDefault);
        }

        await _database.SaveChangesAsync();
        body.UserDefaultAvailableId = userDefault.Id;
        var dbPatched = DbQuery(body, userId);
        var patched = dbPatched?.ToPatchDefaultUserSchedule(userId, userDefault.Id);
        sw.Stop();
        result.Success = true;
        result.Patched = patched;
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;

        DbRoosterDefault? DbQuery(PatchDefaultUserSchedule body, Guid userId)
        {
            return _database.RoosterDefaults
                .Include(x => x.UserDefaultAvailables!.Where(y => y.UserId == userId && y.Id == body.UserDefaultAvailableId))
                .Include(c => c.Customer)
                .FirstOrDefault(z => z.Id == body.DefaultId);
        }
    }
}