using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class ScheduleForUserResponse
{
    public List<Training> Trainings { get; set; } = new List<Training>();
}
