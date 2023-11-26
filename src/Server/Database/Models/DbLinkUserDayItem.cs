using Microsoft.Graph.Models.Security;
using Microsoft.Graph.Models;
using Drogecode.Knrm.Oefenrooster.Database.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models
{
    public class DbLinkUserDayItem
    {
        public Guid UserForeignKey { get; set; }
        public Guid DayItemForeignKey { get; set; }
        public DbUsers User { get; set; } = null!;
        public DbRoosterItemDay DayItem { get; set; } = null!;
    }
}
