using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class NewTraining : TrainingBase
{
    public DateTime? Date { get; set; }
    public TimeSpan? TimeStart { get; set; }
    public TimeSpan? TimeEnd { get; set; }
    public override TrainingType TrainingType { get; set; }
}
