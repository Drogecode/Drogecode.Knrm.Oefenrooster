using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("UserSettings")]
public class DbUserSettings
{
    [Key] public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CustomerId { get; set; }
    [StringLength(30)] public string? Setting { get; set; }
    [StringLength(30)] public string? Value { get; set; }

    public DbCustomers Customer { get; set; }
    public DbUsers User { get; set; }
}
