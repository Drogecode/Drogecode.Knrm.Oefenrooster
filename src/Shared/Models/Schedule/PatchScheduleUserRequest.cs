using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PatchScheduleUserRequest
{
    public Guid? TrainingId { get; set; }
    public Guid? FunctionId { get; set; }
    public PlanUser? User { get; set; }
}
