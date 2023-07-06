using Drogecode.Knrm.Oefenrooster.Server.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Drogecode.Knrm.Oefenrooster.Server.Health;

public class DatabaseHealthCheck : IHealthCheck
{
    //https://youtu.be/p2faw9DCSsY
    //Using NuGet instead of this
    private readonly ILogger<DefaultScheduleService> _logger;
    private readonly Database.DataContext _database;
    public DatabaseHealthCheck(ILogger<DefaultScheduleService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            _database.Users.Count();
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db health check failed");
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}
