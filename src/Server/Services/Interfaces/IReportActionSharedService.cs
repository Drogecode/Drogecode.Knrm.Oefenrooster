using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportActionShared;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IReportActionSharedService
{
    Task<PutReportActionSharedResponse> PutReportActionShared(ReportActionSharedConfiguration body, Guid customerId, Guid userId, CancellationToken clt);
}