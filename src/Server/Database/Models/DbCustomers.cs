using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("Customers")]
public class DbCustomers
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime Created { get; set; }
}
