using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("UserLastCalendarUpdate")]
public class DbUserLastCalendarUpdate
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    public DateTime LastUpdate { get; set; }
    
    public DbCustomers Customer { get; set; }
    public DbUsers? User { get; set; }
}