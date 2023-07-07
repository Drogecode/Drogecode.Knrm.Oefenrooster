namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ReportService : IReportService
{
    private readonly ILogger<ReportService> _logger;
    private readonly Database.DataContext _database;
    public ReportService(ILogger<ReportService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }
}
