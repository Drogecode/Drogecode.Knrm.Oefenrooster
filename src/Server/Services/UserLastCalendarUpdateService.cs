using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserLastCalendarUpdateService : IUserLastCalendarUpdateService
{
    private readonly ILogger<UserLastCalendarUpdateService> _logger;
    private readonly DataContext _database;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UserLastCalendarUpdateService(ILogger<UserLastCalendarUpdateService> logger, DataContext database, IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _database = database;
        _dateTimeProvider = dateTimeProvider;
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
                    LastUpdate = _dateTimeProvider.UtcNow(),
                };
                _database.UserLastCalendarUpdates.Add(userLastCalendar);
            }
            else
            {
                userLastCalendar.LastUpdate = _dateTimeProvider.UtcNow();
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

    public async Task<List<DbUserLastCalendarUpdate>> GetLastUpdateUsers(int minutesInThePast, int maxMinutesInThePast, CancellationToken clt)
    {
        try
        {
            var dateNotBefore = _dateTimeProvider.UtcNow().AddMinutes(-minutesInThePast);
            var dateNotAfter = _dateTimeProvider.UtcNow().AddMinutes(-maxMinutesInThePast);
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