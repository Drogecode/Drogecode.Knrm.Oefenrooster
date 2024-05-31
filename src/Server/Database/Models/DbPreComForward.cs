using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("PreComForward")]
public class DbPreComForward
{
    [Key] public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? CustomerId { get; set; }
    [StringLength(150)] public string ForwardUrl { get; set; }
    
    public DbUsers? User { get; set; }
    public DbCustomers? Customer { get; set; }
}