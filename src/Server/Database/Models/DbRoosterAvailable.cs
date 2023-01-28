using Drogecode.Knrm.Oefenrooster.Database.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterAvailable")]
public class DbRoosterAvailable
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid TrainingId { get; set; }
    public DateOnly Date { get; set; }
    public Availabilty? Available { get; set; }
    public bool Assigned { get; set; }

    public DbCustomers? Customer { get; set; }
    public DbUsers? User { get; set; }
    public DbRoosterTraining? Training { get; set; }
}
