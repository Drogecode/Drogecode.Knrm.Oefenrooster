using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class EditTraining : TrainingBase
{
    public Guid? Id { get; set; }
    public Guid? DefaultId { get; set; }
    public DateTime? Date { get; set; }
    public TimeSpan? TimeStart { get; set; }
    public TimeSpan? TimeEnd { get; set; }
    public bool IsNew { get; set; }
    public bool IsNewFromDefault { get; set; }
    public bool CountToTrainingTarget { get; set; }
    public bool IsPinned { get; set; }
    public bool ShowTime { get; set; } = false;
}
