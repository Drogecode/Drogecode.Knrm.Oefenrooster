using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;
[Table("UserFunctions")]
public class DbUserFunctions
{
    [Key]
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string FunctionName { get; set; }

    public DbCustomers Customer { get; set; }
}
