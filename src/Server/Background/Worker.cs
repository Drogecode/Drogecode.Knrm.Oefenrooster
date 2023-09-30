using Drogecode.Knrm.Oefenrooster.Shared.Helpers;

namespace Drogecode.Knrm.Oefenrooster.Server.Background;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IGraphService _graphService;
    private CancellationToken _clt;

    public Worker(ILogger<Worker> logger, IGraphService graphService)
    {
        _logger = logger;
        _graphService = graphService;
    }

    protected override async Task ExecuteAsync(CancellationToken clt)
    {
        _clt = clt;
        int errorCount = 0;
        while (!clt.IsCancellationRequested)
        {
            try
            {
                await SyncSharePointActions();
                for (int i = 0; i < 60; i++)// run once every minute
                {
                    await Task.Delay(1000, clt);
                }
            }
            catch(Exception ex)
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
        _graphService.InitializeGraph();
        await _graphService.SyncSharePointActions(DefaultSettingsHelper.KnrmHuizenId, _clt);
    }
}