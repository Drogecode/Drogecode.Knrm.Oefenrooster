using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("Licenses")]
public class DbLicenses
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Licenses License { get; set; }
    
    public DbCustomers Customer { get; set; }
}