using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IReportActionService
{
    Task<MultipleReportActionsResponse> GetListActionsUser(List<Guid?> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt);
    Task<AnalyzeYearChartAllResponse> AnalyzeYearChartsAll(AnalyzeActionRequest actionRequest, Guid customerId, string timeZone, CancellationToken clt);
    Task<DistinctResponse> Distinct(DistinctReport column, Guid customerId, CancellationToken clt);
    Task<AnalyzeHoursResult> AnalyzeHours(int year, string type, string timeZone, Guid customerId, CancellationToken clt);
    Task<KillDbResponse> KillDb(Guid customerId, CancellationToken clt);
}
