﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;

public sealed class GetMultipleDayItemResponse : BaseMultipleResponse
{
    public List<RoosterItemDay>? DayItems { get; set; }
}
