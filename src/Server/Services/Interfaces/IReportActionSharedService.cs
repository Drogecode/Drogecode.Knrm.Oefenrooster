using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportActionShared;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IReportActionSharedService
{
    Task<MultipleReportActionShareConfigurationResponse> GetAllReportActionSharedConfiguration(Guid customerId, Guid userId, CancellationToken clt);
    Task<PutReportActionSharedResponse> PutReportActionShared(ReportActionSharedConfiguration body, Guid customerId, Guid userId, CancellationToken clt);
    Task<ReportActionSharedConfiguration?> GetReportActionSharedConfiguration(Guid customerId, Guid sharedId, CancellationToken clt);
    Task<AuthenticateExternalResult> AuthenticateExternal(AuthenticateExternalRequest body, CancellationToken clt);
    Task<DeleteResponse> DeleteReportActionSharedConfiguration(Guid itemId, Guid customerId, Guid userId, CancellationToken clt);
}