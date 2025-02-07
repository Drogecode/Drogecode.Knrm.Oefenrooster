using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IAuditService
{
    Task Log(Guid user, AuditType auditType, Guid customer, string? note = null, Guid? objectKey = null, string? objectName = null);
    Task<GetTrainingAuditResponse> GetTrainingAudit(Guid customerId, Guid userId, int count, int skip, Guid trainingId, CancellationToken clt);
}
