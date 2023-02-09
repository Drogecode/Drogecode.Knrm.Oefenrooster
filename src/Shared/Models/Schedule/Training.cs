using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class Training
{
    public Guid? TrainingId { get; set; }
    public Guid? DefaultId { get; set; }
    public string? Name { get; set; }
    public DateTime Date { get; set; }
    /// <summary>
    /// Time from start in minutes.
    /// </summary>
    public int Duration { get; set; }
    public Availabilty? Availabilty { get; set; }
    public bool Assigned { get; set; }
    public bool Updated { get; set; }
}
