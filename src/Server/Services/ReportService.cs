using Drogecode.Knrm.Oefenrooster.Shared.Models.Report;

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

    public Task<MultipleReportActionsResponse> GetListActionsUser(List<Guid> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
    {
        throw new NotImplementedException();
    }

    public Task<MultipleReportTrainingsResponse> GetListTrainingUser(List<Guid> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
    {
        throw new NotImplementedException();
    }
}
