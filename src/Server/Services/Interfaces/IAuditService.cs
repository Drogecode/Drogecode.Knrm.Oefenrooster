using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IAuditService
{
   Task Log(Guid user, AuditType auditType, Guid customer, string? note = null, Guid? objectKey = null, string? objectName = null);
}
