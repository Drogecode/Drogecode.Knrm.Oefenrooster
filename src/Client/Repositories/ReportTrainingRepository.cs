using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
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

    public async Task<MultipleReportTrainingsResponse> GetLastTraining(IEnumerable<DrogeUser> users, int count, int skip, CancellationToken clt)
    {
        var workingList = new List<Guid>();
        foreach(var user in users)
        {
            workingList.Add(user.Id);
        }
        var usersAsString = System.Text.Json.JsonSerializer.Serialize(workingList);
        var result = await _reportTrainingClient.GetLastTrainingsAsync(usersAsString, count, skip, clt);
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
        var result = await _reportTrainingClient.AnalyzeYearChartsAllAsync(usersAsString, clt);
        return result;
    }
}