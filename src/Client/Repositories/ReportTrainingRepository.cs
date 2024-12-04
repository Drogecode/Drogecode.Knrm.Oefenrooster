using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportTraining;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class ReportTrainingRepository
{
    
    private readonly IReportTrainingClient _reportTrainingClient;

    public ReportTrainingRepository(IReportTrainingClient reportTrainingClient)
    {
        _reportTrainingClient = reportTrainingClient;
    }
    
    public async Task<MultipleReportTrainingsResponse?> GetLastTrainingsForCurrentUser(int count, int skip, CancellationToken clt)
    {
        var result = await _reportTrainingClient.GetLastTrainingsForCurrentUserAsync(count, skip, clt);
        return result;
    }

    public async Task<MultipleReportTrainingsResponse> GetLastTraining(IEnumerable<DrogeUser> users, IEnumerable<string> types, int count, int skip, CancellationToken clt)
    {
        var workingList = new List<Guid>();
        foreach(var user in users)
        {
            workingList.Add(user.Id);
        }
        var usersAsString = System.Text.Json.JsonSerializer.Serialize(workingList);
        var typesAsString = System.Text.Json.JsonSerializer.Serialize(types);
        var result = await _reportTrainingClient.GetLastTrainingsAsync(usersAsString, count, skip, typesAsString, clt);
        return result;
    }
    
    public async Task<AnalyzeYearChartAllResponse?> AnalyzeYearChartsAll(IEnumerable<DrogeUser> users, int? years, CancellationToken clt)
    {
        try
        {
            var request = new AnalyzeTrainingRequest()
            {
                Users = users.Select(x => (Guid?)x.Id).ToList(),
                Years = years
            };
            var result = await _reportTrainingClient.AnalyzeYearChartsAllAsync(request, clt);
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
        var result = await _reportTrainingClient.DistinctAsync(column, clt);
        return result;
    }

    public async Task<AnalyzeHoursResult?> AnalyzeHoursAsync(int year, string type, CancellationToken clt)
    {
        var result = await _reportTrainingClient.AnalyzeHoursAsync(year, type, clt);
        return result;
    }
}