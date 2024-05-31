using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("LinkExchange")]
public class DbLinkExchange
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid VehicleId { get; set; }
    [StringLength(200)] public string? CalendarEventId { get; set; }
    public bool IsSet { get; set; }

    public DbCustomers Customer { get; set; }
    public ICollection<DbRoosterAvailable>? RoosterAvailables { get; set; }
}
