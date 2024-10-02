using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("UserLinkedMails")]
public class DbUserLinkedMails
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    [StringLength(150)] public string? Email { get; set; }
    [StringLength(11)] public string? ActivateKey { get; set; }
    public DateTime? ActivateRequestedOn { get; set; }
    public int ActivationFailedAttempts { get; set; }
    public bool IsActive { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
    
    public DbCustomers Customer { get; set; }
    public DbUsers? User { get; set; }
}