using Drogecode.Knrm.Oefenrooster.Database.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("UserDefaultGroup")]
public class DbUserDefaultGroup
{
    [Key] public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CustomerId { get; set; }
    public string Name { get; set; } = "";
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }

    public DbCustomers Customer { get; set; }
    public DbUsers User { get; set; }
    public ICollection<DbUserDefaultAvailable>? UserDefaultAvailables { get; set; }
}
