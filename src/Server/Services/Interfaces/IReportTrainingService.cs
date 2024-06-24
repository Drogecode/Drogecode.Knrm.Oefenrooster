using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportTraining;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IReportTrainingService
{
    Task<MultipleReportTrainingsResponse> GetListTrainingUser(List<Guid> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt);
    Task<AnalyzeYearChartAllResponse> AnalyzeYearChartsAll(List<Guid> users, Guid customerId, string timeZone, CancellationToken clt);
}