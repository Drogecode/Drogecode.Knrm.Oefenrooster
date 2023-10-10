using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Server.Services;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;

namespace Drogecode.Knrm.Oefenrooster.Server.Background;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private CancellationToken _clt;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken clt)
    {
        _clt = clt;
        int errorCount = 0;
        while (!clt.IsCancellationRequested && _configuration.GetValue<bool>("Drogecode:RunBackgroundService"))
        {
            try
            {
                await SyncSharePointActions();
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

    private async Task SyncSharePointActions()
    {
        using var scope = _scopeFactory.CreateScope();
        var graphService = scope.ServiceProvider.GetRequiredService<IGraphService>();
        graphService.InitializeGraph();
        await graphService.SyncSharePointActions(DefaultSettingsHelper.KnrmHuizenId, _clt);
        _clt.ThrowIfCancellationRequested();
        await graphService.SyncSharePointTrainings(DefaultSettingsHelper.KnrmHuizenId, _clt);
    }
}