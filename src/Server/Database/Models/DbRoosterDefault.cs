using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterDefault")]
public class DbRoosterDefault
{
    [Key]
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DayOfWeek WeekDay { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public DbCustomers? Customer { get; set; }
    public ICollection<DbRoosterTraining>? RoosterTrainings { get; set; }
}
