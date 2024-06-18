using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class ReportActionRepository
{
    private readonly IReportActionClient _reportActionClient;

    public ReportActionRepository(IReportActionClient reportActionClient)
    {
        _reportActionClient = reportActionClient;
    }
    
    public async Task<MultipleReportActionsResponse?> GetLastActionsForCurrentUser(int count, int skip, CancellationToken clt)
    {
        var result = await _reportActionClient.GetLastActionsForCurrentUserAsync(count, skip, clt);
        return result;
    }

    public async Task<MultipleReportActionsResponse> GetLastActions(IEnumerable<DrogeUser> users, int count, int skip, CancellationToken clt)
    {
        var workingList = new List<Guid>();
        foreach(var user in users)
        {
            workingList.Add(user.Id);
        }
        var usersAsString = System.Text.Json.JsonSerializer.Serialize(workingList);
        var result = await _reportActionClient.GetLastActionsAsync(usersAsString, count, skip, clt);
        return result;
    }
    
    public async Task<AnalyzeYearChartAllResponse?> AnalyzeYearChartsAll(IEnumerable<DrogeUser> users, CancellationToken clt)
    {
        var workingList = new List<Guid>();
        foreach(var user in users)
        {
            workingList.Add(user.Id);
        }
        var usersAsString = System.Text.Json.JsonSerializer.Serialize(workingList);
        var result = await _reportActionClient.AnalyzeYearChartsAllAsync(usersAsString, clt);
        return result;
    }
}