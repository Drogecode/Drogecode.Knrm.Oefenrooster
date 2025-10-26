using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;

public abstract class DrogeService : IDrogeService
{
    protected readonly ILogger<DrogeService> Logger;
    protected readonly DataContext Database;
    protected readonly IMemoryCache MemoryCache;
    protected readonly IDateTimeProvider DateTimeProvider;
    internal readonly MemoryCacheEntryOptions CacheOptions;

    protected DrogeService(
        ILogger<DrogeService> logger,
        DataContext database,
        IMemoryCache memoryCache,
        IDateTimeProvider dateTimeProvider)
    {
        Logger = logger;
        Database = database;
        MemoryCache = memoryCache;
        DateTimeProvider = dateTimeProvider;
        
        var cacheOptions = new MemoryCacheEntryOptions();
        cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(3));
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(7));
        cacheOptions.Priority = CacheItemPriority.Normal;
        CacheOptions = cacheOptions;
    }

    public async Task<int> SaveDb(CancellationToken clt)
    {
        clt.ThrowIfCancellationRequested();
        return await Database.SaveChangesAsync(clt);
    }
}