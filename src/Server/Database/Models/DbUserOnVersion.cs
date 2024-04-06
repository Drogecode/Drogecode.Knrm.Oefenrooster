using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("UserOnVersion")]
public class DbUserOnVersion
{
    [Key] public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CustomerId { get; set; }
    public string Version { get; set; }
    public DateTime LastSeenOnThisVersion { get; set; }

    public DbCustomers Customer { get; set; }
    public DbUsers User { get; set; }
}