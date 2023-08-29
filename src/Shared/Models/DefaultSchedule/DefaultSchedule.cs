﻿using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

public class DefaultSchedule
{
    public Guid Id { get; set; }
    public Guid? RoosterTrainingTypeId { get; set; }
    public Guid? UserDefaultAvailableId { get; set; }
    public DayOfWeek WeekDay { get; set; }
    public Availabilty? Available { get; set; }
    public TimeOnly TimeStart { get; set; }
    public TimeOnly TimeEnd { get; set; }
    public DateTime ValidFromDefault { get; set; }
    public DateTime ValidUntilDefault { get; set; }
    public DateTime? ValidFromUser { get; set; }
    public DateTime? ValidUntilUser { get; set; }
    public bool CountToTrainingTarget { get; set; } = true;
    public bool Assigned { get; set; }
    public int Order { get; set; }
}
