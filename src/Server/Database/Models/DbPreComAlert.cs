using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("PreComAlert")]
public class DbPreComAlert
{
    [Key] public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? CustomerId { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? Alert { get; set; }
    /// <summary>
    /// Only to complete the object, not all fields are always returned.
    /// </summary>
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? Raw { get; set; }
    [StringLength(50)] public string? Ip { get; set; }
    public DateTime? SendTime { get; set; }
    public int? Priority { get; set; }

    public DbUsers? User { get; set; }
    public DbCustomers? Customer { get; set; }
}
