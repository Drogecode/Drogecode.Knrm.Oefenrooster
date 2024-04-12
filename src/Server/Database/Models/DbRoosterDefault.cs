using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterDefault")]
public class DbRoosterDefault
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? RoosterTrainingTypeId { get; set; }
    public List<Guid>? VehicleIds { get; set; }
    public string? Name { get; set; }
    public DayOfWeek WeekDay { get; set; }
    public TimeOnly TimeStart { get; set; }
    public TimeOnly TimeEnd { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    public string TimeZone { get; set; } = string.Empty;
    public bool CountToTrainingTarget { get; set; } = true;
    public bool? ShowTime { get; set; } = true;
    public int Order { get; set; }

    public DbCustomers Customer { get; set; }
    public DbRoosterTrainingType? RoosterTrainingType { get; set; }
    public ICollection<DbRoosterTraining>? RoosterTrainings { get; set; }
    public ICollection<DbUserDefaultAvailable>? UserDefaultAvailables { get; set; }
}
