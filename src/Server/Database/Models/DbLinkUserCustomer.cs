using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("LinkUserCustomer")]
public class DbLinkUserCustomer
{
    [Key] public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid GlobalUserId { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; }
    public int Order { get; set; }
    public Guid LinkedBy { get; set; }
    public DateTime LinkedOn { get; set; }
    
    public DbUsers User { get; set; }
    public DbCustomers Customer { get; set; }
    public DbUsersGlobal LinkedUser { get; set; }
    
}