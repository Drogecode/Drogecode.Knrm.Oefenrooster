using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IReportActionService
{
    Task<MultipleReportActionsResponse> GetListActionsUser(List<Guid> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt);
    Task<AnalyzeYearChartAllResponse> AnalyzeYearChartsAll(List<Guid> users, Guid customerId, CancellationToken clt);
}
