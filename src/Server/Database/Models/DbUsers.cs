using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Database.Models;

[Table("Users")]
public class DbUsers
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? UserFunctionId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }

    public DbCustomers Customer { get; set; }
    public DbUserFunctions? UserFunction{ get; set; }
    public ICollection<DbRoosterAvailable>? RoosterAvailables { get; set; }
    public ICollection<DbAudit>? Audits { get; set; }
}
