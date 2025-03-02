using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;

public abstract class DrogeService : IDrogeService
{
    protected readonly ILogger<DrogeService> Logger;
    protected readonly DataContext Database;
    protected readonly IMemoryCache MemoryCache;
    protected readonly IDateTimeService DateTimeService;

    public DrogeService(
        ILogger<DrogeService> logger,
        DataContext database,
        IMemoryCache memoryCache,
        IDateTimeService dateTimeService)
    {
        Logger = logger;
        Database = database;
        MemoryCache = memoryCache;
        DateTimeService = dateTimeService;
    }

    public async Task<int> SaveDb(CancellationToken clt)
    {
        clt.ThrowIfCancellationRequested();
        return await Database.SaveChangesAsync(clt);
    }
}