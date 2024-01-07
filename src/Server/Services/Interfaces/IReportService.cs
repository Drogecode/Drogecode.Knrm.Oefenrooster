
using Drogecode.Knrm.Oefenrooster.Shared.Models.Report;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IReportService
{
    Task<MultipleReportActionsResponse> GetListActionsUser(List<Guid> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt);
    Task<MultipleReportTrainingsResponse> GetListTrainingUser(List<Guid> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt);
}
