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
    public DateTime Date { get; set; }
    /// <summary>
    /// Time from start in minutes.
    /// </summary>
    public int Duration { get; set; }

    public DbCustomers Customer { get; set; }
    public DbRoosterDefault? RoosterDefault { get; set; }
    public ICollection<DbRoosterAvailable>? RoosterAvailables { get; set; }
}
