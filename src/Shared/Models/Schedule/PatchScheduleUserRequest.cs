using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PatchAssignedUserRequest
{
    public Guid? TrainingId { get; set; }
    public PlanUser? User { get; set; }
    public TrainingAdvance? Training { get; set; }
    public AuditReason? AuditReason { get; set; }
}
