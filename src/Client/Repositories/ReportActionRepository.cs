using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
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
        foreach (var user in users)
        {
            workingList.Add(user.Id);
        }

        var usersAsString = System.Text.Json.JsonSerializer.Serialize(workingList);
        var result = await _reportActionClient.GetLastActionsAsync(usersAsString, count, skip, clt);
        return result;
    }

    public async Task<AnalyzeYearChartAllResponse?> AnalyzeYearChartsAll(IEnumerable<DrogeUser> users, IEnumerable<string?>? prio, int? years, CancellationToken clt)
    {
        try
        {
            var request = new AnalyzeActionRequest()
            {
                Users = users.Select(x => (Guid?)x.Id).ToList(),
                Prio = prio,
                Years = years
            };
            var result = await _reportActionClient.AnalyzeYearChartsAllAsync(request, clt);
            return result;
        }
        catch (Exception ex)
        {
            DebugHelper.WriteLine(ex);
        }

        return new AnalyzeYearChartAllResponse();
    }

    public async Task<DistinctResponse?> Distinct(DistinctReport column, CancellationToken clt)
    {
        var result = await _reportActionClient.DistinctAsync(column, clt);
        return result;
    }

    public async Task<AnalyzeHoursResult?> AnalyzeHoursAsync(int year, string type, CancellationToken clt)
    {
        var result = await _reportActionClient.AnalyzeHoursAsync(year, type, clt);
        return result;
    }
}