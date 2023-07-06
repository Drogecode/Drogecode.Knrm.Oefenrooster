using Drogecode.Knrm.Oefenrooster.Database.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;
[Table("UserFunctions")]
public class DbUserFunctions
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public int TrainingTarget { get; set; }
    public bool TrainingOnly { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }

    public DbCustomers Customer { get; set; }
    public ICollection<DbUsers>? Users { get; set; }
    public ICollection<DbRoosterAvailable>? RoosterAvailables { get; set; }
}
