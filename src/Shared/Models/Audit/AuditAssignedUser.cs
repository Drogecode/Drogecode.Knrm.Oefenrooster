using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;

public class AuditAssignedUser
{
    public Guid? UserId { get; set; }
    public Guid? VehicleId { get; set; }
    public Guid? FunctionId { get; set; }
    public bool? Assigned { get; set; }
    public Availability? Availability { get; set; }
    public AvailabilitySetBy? SetBy { get; set; }
    public AuditReason? AuditReason { get; set; }
}

public enum AuditReason
{
    None = 0,
    Assigned = 1,
    ChangeAvailability = 2,
    ChangeVehicle = 3,
    ChangedFunction = 4,
}