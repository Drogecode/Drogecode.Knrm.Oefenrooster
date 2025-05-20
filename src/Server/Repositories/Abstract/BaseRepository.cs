using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Repositories.Abstract;

public abstract class BaseRepository
{
    
    protected readonly ILogger<BaseRepository> Logger;
    protected readonly DataContext Database;
    protected readonly IMemoryCache MemoryCache;
    protected readonly IDateTimeService DateTimeService;
    internal MemoryCacheEntryOptions CacheOptions;

    public BaseRepository(
        ILogger<BaseRepository> logger,
        DataContext database,
        IMemoryCache memoryCache,
        IDateTimeService dateTimeService)
    {
        Logger = logger;
        Database = database;
        MemoryCache = memoryCache;
        DateTimeService = dateTimeService;
        
        var cacheOptions = new MemoryCacheEntryOptions();
        cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(3));
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(7));
        CacheOptions = cacheOptions;
    }
}