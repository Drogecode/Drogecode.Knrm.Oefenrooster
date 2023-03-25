using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterTrainingType")]
public class DbRoosterTrainingType
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string? Name { get; set; }
    public string? ColorLight { get; set; }
    public string? ColorDark { get; set; }
    public bool CountToTrainingTarget { get; set; }
    public bool IsDefault { get; set; }

    public DbCustomers Customer { get; set; }
    public ICollection<DbRoosterTraining>? RoosterTrainings { get; set; }
    public ICollection<DbRoosterDefault>? RoosterDefaults { get; set; }
}
