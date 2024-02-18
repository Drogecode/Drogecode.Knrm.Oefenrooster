using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Server.Services;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Background;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _memoryCache;
    private CancellationToken _clt;

    private const string NEXT_USER_SYNC = "all_usr_sync";

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration, IMemoryCache memoryCache)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _memoryCache = memoryCache;
    }

    protected override async Task ExecuteAsync(CancellationToken clt)
    {
        _clt = clt;
        int errorCount = 0;
        _memoryCache.Set(NEXT_USER_SYNC, DateTime.UtcNow); // do not run on startup
        while (!clt.IsCancellationRequested && _configuration.GetValue<bool>("Drogecode:RunBackgroundService"))
        {
            try
            {
                await SyncSharePoint();
                for (int i = 0; i < 60; i++)// run once every minute
                {
                    await Task.Delay(1000, clt);
                }
            }
            catch (Exception ex)
            {
                errorCount++;
                _logger.LogError(ex, "Error {errorCount} in worker", errorCount);
                var sleep = 60 * errorCount;
                for (int i = 0; i < sleep; i++)// run once every minute
                {
                    await Task.Delay(1000, clt);
                }
            }
        }
    }

    private async Task SyncSharePoint()
    {
        using var scope = _scopeFactory.CreateScope();
        var graphService = scope.ServiceProvider.GetRequiredService<IGraphService>();
        graphService.InitializeGraph();

        await SyncSharePointActions(graphService);
        await SyncSharePointUsers(graphService);
    }

    private async Task SyncSharePointActions(IGraphService graphService)
    {
        await graphService.SyncSharePointActions(DefaultSettingsHelper.KnrmHuizenId, _clt);
        _clt.ThrowIfCancellationRequested();
        await graphService.SyncSharePointTrainings(DefaultSettingsHelper.KnrmHuizenId, _clt);
        _clt.ThrowIfCancellationRequested();
    }
    private async Task SyncSharePointUsers(IGraphService graphService)
    {
        var nextSync = _memoryCache.Get<DateTime?>(NEXT_USER_SYNC);
        if (nextSync is not null && nextSync.Value.CompareTo(DateTime.UtcNow) > 0)
            return;
        //ToDo sync

        _clt.ThrowIfCancellationRequested();
        _memoryCache.Set(NEXT_USER_SYNC, DateTime.SpecifyKind( DateTime.Today.AddDays(1).AddHours(1), DateTimeKind.Utc));
    }
}