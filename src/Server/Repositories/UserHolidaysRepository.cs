using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Repositories.Abstract;
using Drogecode.Knrm.Oefenrooster.Server.Repositories.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Repositories;

public class UserHolidaysRepository : BaseRepository, IUserHolidaysRepository
{
    public UserHolidaysRepository(
        ILogger<RoosterDefaultsRepository> logger,
        DataContext database,
        IMemoryCache memoryCache,
        IDateTimeProvider dateTimeProvider) : base(logger, database, memoryCache, dateTimeProvider)
    {
    }

    public async Task<List<DbUserHolidays>> GetUserHolidaysForUser(bool cache, Guid customerId, Guid? userId, DateTime tillDate, DateTime startDate, CancellationToken clt)
    {
        var cacheKey = $"HolidaysForUser-{customerId}-{userId}-{tillDate}-{startDate}";
        MemoryCache.TryGetValue(cacheKey, out List<DbUserHolidays>? result);
        if (result is not null && cache)
            return result;
       
        result = await Database.UserHolidays
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId && (userId == null || x.UserId == userId) && x.ValidFrom <= tillDate && x.ValidUntil >= startDate)
            .AsSingleQuery()
            .ToListAsync(cancellationToken: clt);
       
        MemoryCache.Set(cacheKey, result, CacheOptions);
        return result;
        
    }
}