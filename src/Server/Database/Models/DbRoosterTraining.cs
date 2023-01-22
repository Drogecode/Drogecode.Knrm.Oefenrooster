using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterTraining")]
public class DbRoosterTraining
{
    [Key]
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateOnly Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public DbCustomers Customer { get; set; }
    public ICollection<DbRoosterAvailable>? RoosterAvailables{ get; set; }
}
