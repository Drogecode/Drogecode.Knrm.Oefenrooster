using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;

public class AuditAssignedUser
{
    public Guid? UserId { get; set; }
    public bool? Assigned { get; set; }
    public Availabilty? Availabilty { get; set; }
    public AvailabilitySetBy? SetBy { get; set; }
}
