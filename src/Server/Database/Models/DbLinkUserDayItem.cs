﻿using Microsoft.Graph.Models.Security;
using Microsoft.Graph.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models
{
    public class DbLinkUserDayItem
    {
        public Guid UserId { get; set; }
        public Guid DayItemId { get; set; }
        public string? CalendarEventId { get; set; }
        public DbUsers User { get; set; } = null!;
        public DbRoosterItemDay DayItem { get; set; } = null!;
    }
}
