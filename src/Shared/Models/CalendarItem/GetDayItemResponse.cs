﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem
{
    public class GetDayItemResponse : BaseResponse
    {
        public RoosterItemDay? DayItem { get; set; }
    }
}
