using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PlannedTraining : TrainingAdvance
{
    public List<PlanUser> PlanUsers { get; set; } = new List<PlanUser>();
    public bool IsCreated { get; set; }
}
