using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("PreComAlert")]
public class DbPreComAlert
{
    [Key] public Guid Id { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? NotificationId { get; set; }
    public string? Alert { get; set; }
    public string? Raw { get; set; }

    public DbCustomers? Customer { get; set; }
}
