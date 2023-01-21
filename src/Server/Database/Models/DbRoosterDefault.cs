using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterDefault")]
public class DbRoosterDefault
{
    [Key]
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public short WeekDay { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
