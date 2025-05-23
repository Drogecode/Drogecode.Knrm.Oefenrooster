﻿using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Repositories.Abstract;
using Drogecode.Knrm.Oefenrooster.Server.Repositories.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Repositories;

public class UserDefaultAvailableRepository : BaseRepository, IUserDefaultAvailableRepository
{
    public UserDefaultAvailableRepository(
        ILogger<RoosterDefaultsRepository> logger,
        DataContext database,
        IMemoryCache memoryCache,
        IDateTimeService dateTimeService) : base(logger, database, memoryCache, dateTimeService)
    {
    }

    public async Task<List<DbUserDefaultAvailable>> GetUserDefaultAvailableForCustomerInSpan(bool cache, Guid customerId, Guid? userId, DateTime tillDate, DateTime startDate, CancellationToken clt)
    {
       var cacheKey = $"DefaultAvaCusUsSpan-{customerId}-{userId}-{tillDate}-{startDate}";
       MemoryCache.TryGetValue(cacheKey, out List<DbUserDefaultAvailable>? result);
       if (result is not null && cache)
           return result;
       
       result = await Database.UserDefaultAvailables.AsNoTracking()
           .Include(x => x.DefaultGroup)
           .Where(x => x.CustomerId == customerId && (userId == null || x.UserId == userId) && x.ValidFrom <= tillDate && x.ValidUntil >= startDate)
           .AsSingleQuery()
           .ToListAsync(cancellationToken: clt);
       
       MemoryCache.Set(cacheKey, result, CacheOptions);
       return result;
    }
}