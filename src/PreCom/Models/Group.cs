using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.PreCom.Models;

public class Group
{
    public long GroupID { get; set; }
    public string Code { get; set; }
    public string Label { get; set; }
    public TimeSpan TimeCalculation { get; set; }
    public Dictionary<DateTime, Dictionary<string, bool?>> SchedulerDays { get; set; }
    public ServiceFuntion[] ServiceFuntions { get; set; }
}
