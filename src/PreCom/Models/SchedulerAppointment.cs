using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.PreCom.Models;

public class SchedulerAppointment
{
    public DateTime Start { get; set; }
    public TimeSpan Duration { get; set; }
}
