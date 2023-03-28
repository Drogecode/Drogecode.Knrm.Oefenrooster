using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterTraining")]
public class DbRoosterTraining
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? RoosterDefaultId { get; set; }
    public Guid? RoosterTrainingTypeId { get; set; }
    public string? Name { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public bool CountToTrainingTarget { get; set; }
    public bool Pin { get; set; }

    public DbCustomers Customer { get; set; }
    public DbRoosterDefault? RoosterDefault { get; set; }
    public DbRoosterTrainingType? RoosterTrainingType { get; set; }
    public ICollection<DbRoosterAvailable>? RoosterAvailables { get; set; }
}
