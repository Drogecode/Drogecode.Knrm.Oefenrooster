using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("LinkVehicleTraining")]
public class DbLinkVehicleTraining
{
    [Key] public Guid Id { get; set; }
    public Guid RoosterTrainingId { get; set; }
    public Guid Vehicle { get; set; }
    public Guid CustomerId { get; set; }
    public bool IsSelected { get; set; }

    public DbCustomers Customer { get; set; }
}
