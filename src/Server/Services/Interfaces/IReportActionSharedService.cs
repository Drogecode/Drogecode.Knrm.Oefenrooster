using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportActionShared;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IReportActionSharedService
{
    Task<MultipleReportActionShareConfigurationResponse> GetAllReportActionSharedConfiguration(Guid customerId, CancellationToken clt);
    Task<PutReportActionSharedResponse> PutReportActionShared(ReportActionSharedConfiguration sharedConfiguration, Guid customerId, Guid userId, CancellationToken clt);
    Task<ReportActionSharedConfiguration?> GetReportActionSharedConfiguration(Guid customerId, Guid sharedId, CancellationToken clt);
    Task<AuthenticateExternalResult> AuthenticateExternal(AuthenticateExternalRequest authenticateExternalRequest, CancellationToken clt);
    Task<DeleteResponse> DeleteReportActionSharedConfiguration(Guid reportActionSharedId, Guid customerId, CancellationToken clt);
}