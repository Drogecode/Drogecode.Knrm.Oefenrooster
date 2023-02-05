﻿using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PlanUser
{
    public Guid UserId { get; set; }
    public Guid? UserFunctionId { get; set; }
    public Availabilty? Availabilty { get; set; }
    public bool Assigned { get; set; }
    public string Name { get; set; } = "Some dude!";
}
