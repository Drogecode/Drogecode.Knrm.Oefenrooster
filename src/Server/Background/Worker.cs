using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Hubs;
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
    private int _errorCount = 0;

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
        _memoryCache.Set(NEXT_USER_SYNC, DateTime.SpecifyKind(DateTime.Today.AddDays(1).AddHours(1), DateTimeKind.Utc)); // do not run on startup
        while (!_clt.IsCancellationRequested && _configuration.GetValue<bool>("Drogecode:RunBackgroundService"))
        {
            try
            {
                var sleep = 60 * (_errorCount * 3 + 1);
                for (int i = 0; i < sleep; i++)
                {
                    if (_clt.IsCancellationRequested) return; // run once every second.
                    await Task.Delay(1000, clt);
                }

                var successfull = await SyncSharePoint();

                if (successfull && _errorCount > 0)
                {
                    _errorCount--;
                    _logger.LogInformation("No error in worker, decreasing counter with one `{errorCount}`", _errorCount);
                }
                else if (!successfull)
                {
                    _errorCount++;
                    _logger.LogWarning("Error in worker, increasing counter with one `{errorCount}`", _errorCount);
                }
            }
            catch (Exception ex)
            {
                _errorCount++;
                _logger.LogError(ex, "Error `{errorCount}` in worker", _errorCount);
            }

            if (_clt.IsCancellationRequested) return;
        }
    }

    private async Task<bool> SyncSharePoint()
    {
        using var scope = _scopeFactory.CreateScope();
        var graphService = scope.ServiceProvider.GetRequiredService<IGraphService>();
        graphService.InitializeGraph();

        var result = true;
        result = (result & await SyncSharePointActions(graphService));
        result = (result & await SyncSharePointUsers(scope, graphService));
        return result;
    }

    private async Task<bool> SyncSharePointActions(IGraphService graphService)
    {
        var result = true;
        result = (result & await RunBackgroundTask(async () => await graphService.SyncSharePointActions(DefaultSettingsHelper.KnrmHuizenId, _clt), "SyncSharePointActions", _clt));
        result = (result & await RunBackgroundTask(async () => await graphService.SyncSharePointTrainings(DefaultSettingsHelper.KnrmHuizenId, _clt), "SyncSharePointTrainings", _clt));
        return result;
    }

    private async Task<bool> SyncSharePointUsers(IServiceScope scope, IGraphService graphService)
    {
        var nextSync = _memoryCache.Get<DateTime?>(NEXT_USER_SYNC);
        if (nextSync is not null && nextSync.Value.CompareTo(DateTime.UtcNow) > 0)
            return true;

        var userControllerLogger = scope.ServiceProvider.GetRequiredService<ILogger<UserController>>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
        var functionService = scope.ServiceProvider.GetRequiredService<IFunctionService>();
        var refreshHub = scope.ServiceProvider.GetRequiredService<RefreshHub>();
        var userController = new UserController(userControllerLogger, userService, auditService, graphService, functionService, refreshHub);
        await userController.InternalSyncAllUsers(DefaultSettingsHelper.SystemUser, DefaultSettingsHelper.KnrmHuizenId, _clt);

        _clt.ThrowIfCancellationRequested();
        _memoryCache.Set(NEXT_USER_SYNC, DateTime.SpecifyKind(DateTime.Today.AddDays(1).AddHours(1), DateTimeKind.Utc));
        return true;
    }

    private async Task<bool> RunBackgroundTask<TRet>(Func<Task<TRet>> function, string name, CancellationToken clt)
    {
        try
        {
            await function();
            clt.ThrowIfCancellationRequested();
            return true;
        }
        catch (Exception ex)
        {
            _errorCount++;
            _logger.LogError(ex, "Error in background service `{name}` {errorCount}`", name, _errorCount);
            clt.ThrowIfCancellationRequested();
            return false;
        }
    }
}