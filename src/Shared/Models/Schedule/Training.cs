using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class Training : TrainingAdvance
{
    public Availabilty? Availabilty { get; set; }
    public AvailabilitySetBy SetBy { get; set; }
    public bool Assigned { get; set; }
}
