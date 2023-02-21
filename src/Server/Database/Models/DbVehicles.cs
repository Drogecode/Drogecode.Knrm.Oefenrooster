using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("Vehicle")]
public class DbVehicles
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public int Order { get; set; }
    public bool Default { get; set; }
    public bool Active { get; set; }
    public DateTime? DeletedOn { get; set; }

    public DbCustomers Customer { get; set; }
    public ICollection<DbRoosterAvailable>? RoosterAvailables { get; set; }
}
