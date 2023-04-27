using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterItemDay")]
public class DbRoosterItemDay : RoosterItemDay
{
    public Guid CustomerId { get; set; }

    public DbCustomers Customer { get; set; }
}
