﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class ScheduleForAllResponse
{
    public List<Planner> Planners { get; set; } = new List<Planner>();
}
