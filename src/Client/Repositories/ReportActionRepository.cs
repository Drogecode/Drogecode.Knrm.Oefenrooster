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

    public async Task<MultipleReportActionsResponse> GetLastActions(IEnumerable<DrogeUser> users, IEnumerable<string> types, string? search, int count, int skip, CancellationToken clt)
    {
        var workingUserList = new List<Guid>();
        foreach (var user in users)
        {
            workingUserList.Add(user.Id);
        }

        var body = new GetLastActionsRequest
        {
            Users = workingUserList,
            Types = types.ToList(),
            Skip = skip,
            Count = count,
            Search = string.IsNullOrWhiteSpace(search) ? null : search.Split(',').ToList(),
        };
        var result = await _reportActionClient.GetLastActionsPOSTAsync(body, clt);
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

    public async Task<AnalyzeHoursResult?> AnalyzeHoursAsync(int year, string type, List<string>? boats, CancellationToken clt)
    {
        var body = new AnalyzeHoursRequest
        {
            Year = year,
            Type = type,
            Boats = boats,
        };
        var result = await _reportActionClient.GetAnalyzeHoursAsync(body, clt);
        return result;
    }
}