using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;

public sealed class AuditLinkUserUser
{
    public Guid UserA { get; set; }
    public Guid UserB { get; set; }
    public bool Add { get; set; }
    public UserUserLinkType LinkType { get; set; }
    public bool Success { get; set; }
    public long ElapsedMilliseconds { get; set; } = -1;
}
