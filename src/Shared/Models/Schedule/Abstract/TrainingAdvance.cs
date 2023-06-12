using Drogecode.Knrm.Oefenrooster.Shared.Enums;
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
    public Guid? VehicleId { get; set; }
    public Guid? PlannedFunctionId { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public bool CountToTrainingTarget { get; set; }
    public bool Updated { get; set; }
    public bool Pin { get; set; }
}
