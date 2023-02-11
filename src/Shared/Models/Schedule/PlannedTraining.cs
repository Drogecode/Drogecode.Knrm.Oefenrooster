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
    public string? Name { get; set; }
    public DateTime Date { get; set; }
    /// <summary>
    /// Time from start in minutes.
    /// </summary>
    public int Duration { get; set; }
    public List<PlanUser> PlanUsers { get; set; } = new List<PlanUser>();
    public bool Updated { get; set; }
    public bool IsCreated { get; set; }
}
