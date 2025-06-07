using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("Audit")]
public class DbAudit
{
    [Key] public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? CustomerId { get; set; }
    public AuditType AuditType { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? Note { get; set; }
    public Guid? ObjectKey { get; set; }
    [StringLength(50)] public string? ObjectName { get; set; }
    public DateTime Created { get; set; }

    public DbUsers User { get; set; }
    public DbCustomers? Customer { get; set; }
}
