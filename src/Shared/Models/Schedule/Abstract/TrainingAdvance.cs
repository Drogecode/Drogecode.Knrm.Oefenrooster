using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

public class TrainingAdvance : TrainingBase
{
    public Guid? TrainingId { get; set; }
    public Guid? DefaultId { get; set; }
    public DateTime Date { get; set; }
    /// <summary>
    /// Time from start in minutes.
    /// </summary>
    public int Duration { get; set; }
    public bool Updated { get; set; }
}
