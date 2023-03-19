using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterTraining")]
public class DbRoosterTraining
{
    [Key]
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? RoosterDefaultId { get; set; }
    public string? Name { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public TrainingType TrainingType { get; set; } = TrainingType.Default;
    public bool CountToTrainingTarget { get; set; }

    public DbCustomers Customer { get; set; }
    public DbRoosterDefault? RoosterDefault { get; set; }
    public ICollection<DbRoosterAvailable>? RoosterAvailables { get; set; }
}
