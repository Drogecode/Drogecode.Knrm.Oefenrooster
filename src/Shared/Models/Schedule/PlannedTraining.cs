using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PlannedTraining
{
    public Guid? TrainingId { get; set; }
    public Guid? DefaultId { get; set; }
    public DateOnly Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<PlanUser> PlanUsers { get; set; } = new List<PlanUser>();
    public bool Updated { get; set; }
    public bool IsCreated { get; set; }
}
