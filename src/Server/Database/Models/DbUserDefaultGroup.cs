using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("UserDefaultGroup")]
public class DbUserDefaultGroup
{
    [Key] public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CustomerId { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_TRAINING_GROUP_NAME)] public string Name { get; set; } = "";
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public bool IsDefault { get; set; } = false;

    public DbCustomers Customer { get; set; }
    public DbUsers User { get; set; }
    public ICollection<DbUserDefaultAvailable>? UserDefaultAvailables { get; set; }
}
