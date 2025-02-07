using System.ComponentModel.DataAnnotations;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models
{
    public class DbLinkUserDayItem
    {
        public Guid UserId { get; set; }
        public Guid DayItemId { get; set; }
        [StringLength(200)] public string? CalendarEventId { get; set; }
        public DbUsers User { get; set; } = null!;
        public DbRoosterItemDay DayItem { get; set; } = null!;
    }
}
