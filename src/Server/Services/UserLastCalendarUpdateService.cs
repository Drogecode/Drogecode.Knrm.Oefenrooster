using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserLastCalendarUpdateService : IUserLastCalendarUpdateService
{
    private readonly ILogger<UserLastCalendarUpdateService> _logger;
    private readonly DataContext _database;
    private readonly IDateTimeService _dateTimeService;

    public UserLastCalendarUpdateService(ILogger<UserLastCalendarUpdateService> logger, DataContext database, IDateTimeService dateTimeService)
    {
        _logger = logger;
        _database = database;
        _dateTimeService = dateTimeService;
    }

    public async Task<bool> AddOrUpdateLastUpdateUser(Guid customerId, Guid userId, CancellationToken clt)
    {
        try
        {
            var userLastCalendar = await _database.UserLastCalendarUpdates.Where(x => x.CustomerId == customerId && x.UserId == userId).FirstOrDefaultAsync(clt);
            if (userLastCalendar is null)
            {
                userLastCalendar = new DbUserLastCalendarUpdate()
                {
                    Id = Guid.CreateVersion7(),
                    CustomerId = customerId,
                    UserId = userId,
                    LastUpdate = _dateTimeService.UtcNow(),
                };
                _database.UserLastCalendarUpdates.Add(userLastCalendar);
            }
            else
            {
                userLastCalendar.LastUpdate = _dateTimeService.UtcNow();
                _database.UserLastCalendarUpdates.Update(userLastCalendar);
            }

            var result = await _database.SaveChangesAsync(clt) > 0;
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in AddOrUpdateLastUpdateUser");
            return false;
        }
    }

    public async Task<List<DbUserLastCalendarUpdate>> GetLastUpdateUsers(CancellationToken clt)
    {
        try
        {
            var dateNotBefore = _dateTimeService.UtcNow().AddMinutes(-15);
            var dateNotAfter = _dateTimeService.UtcNow().AddHours(-1);
            var users = await _database.UserLastCalendarUpdates
                .Where(x => x.LastUpdate <= dateNotBefore && x.LastUpdate >= dateNotAfter)
                .AsNoTracking()
                .ToListAsync(clt);
            return users;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in GetLastUpdateUsers");
            throw;
        }
    }
}