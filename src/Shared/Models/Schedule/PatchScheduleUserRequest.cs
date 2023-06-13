using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PatchAssignedUserRequest
{
    public Guid? TrainingId { get; set; }
    public PlanUser? User { get; set; }
    public TrainingAdvance? Training { get; set; }
}
