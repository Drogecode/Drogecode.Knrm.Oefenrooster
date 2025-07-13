using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Repositories.Abstract;
using Drogecode.Knrm.Oefenrooster.Server.Repositories.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Repositories;

public class RoosterDefaultsRepository : BaseRepository, IRoosterDefaultsRepository
{
    public RoosterDefaultsRepository(
        ILogger<RoosterDefaultsRepository> logger,
        DataContext database,
        IMemoryCache memoryCache,
        IDateTimeProvider dateTimeProvider) : base(logger, database, memoryCache, dateTimeProvider)
    {
    }

    public async Task<List<DbRoosterDefault>> GetDefaultsForCustomerInSpan(bool cache, Guid customerId, DateTime tillDate, DateTime startDate, CancellationToken clt)
    {
        var cacheKey = $"DefaultCusSpan-{customerId}{tillDate}-{startDate}";
        MemoryCache.TryGetValue(cacheKey, out List<DbRoosterDefault>? result);
        if (result is not null && cache)
            return result;
        
        result = await Database.RoosterDefaults.AsNoTracking().Where(x => x.CustomerId == customerId && x.ValidFrom <= startDate && x.ValidUntil >= tillDate)
            .AsSingleQuery().ToListAsync(cancellationToken: clt);
        
        MemoryCache.Set(cacheKey, result, CacheOptions);
        return result;
    }
}