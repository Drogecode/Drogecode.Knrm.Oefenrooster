using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ReportActionService : IReportActionService
{
    private readonly ILogger<ReportActionService> _logger;
    private readonly Database.DataContext _database;
    public ReportActionService(ILogger<ReportActionService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<MultipleReportActionsResponse> GetListActionsUser(List<Guid> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var listWhere = _database.ReportActions.Include(x=>x.Users).Where(x => x.Users.Count(y => users.Contains(y.DrogeCodeId)) == users.Count());
        var sharePointActionsUser = new MultipleReportActionsResponse
        {
            Actions = await listWhere.Skip(skip).Take(count).Select(x=>x.ToDrogeAction()).ToListAsync(clt),
            TotalCount = listWhere.Count()
        };
        sw.Stop();
        sharePointActionsUser.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return sharePointActionsUser;
    }
}
