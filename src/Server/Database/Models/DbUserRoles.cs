using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("UserRoles")]
public class DbUserRoles
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_ROLE_EXTERNAL_ID)] public string? ExternalId { get; set; }
    [StringLength(50)] public string? Name { get; set; }
    [StringLength(1000)] public string? Accesses { get; set; }// ToDo: Convert to enum list
    public int Order { get; set; }

    public DbCustomers Customer { get; set; }
    public ICollection<DbLinkUserRole>? LinkUserRoles { get; set; }
}
