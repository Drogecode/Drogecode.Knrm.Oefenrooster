using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterDefault")]
public class DbRoosterDefault
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? RoosterTrainingTypeId { get; set; }
    public DayOfWeek WeekDay { get; set; }
    public TimeOnly TimeStart { get; set; }
    public TimeOnly TimeEnd { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    public bool CountToTrainingTarget { get; set; } = true;

    public DbCustomers Customer { get; set; }
    public DbRoosterTrainingType? RoosterTrainingType { get; set; }
    public ICollection<DbRoosterTraining>? RoosterTrainings { get; set; }
}
