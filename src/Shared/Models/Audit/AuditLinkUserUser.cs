using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
