using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class OtherScheduleUserRequest
{
    public Guid? TrainingId { get; set; }
    public Guid? FunctionId { get; set; }
    public Guid? UserId { get; set; }
    public bool Assigned { get; set; }
}
