using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("Menu")]
public class DbMenu
{
    [Key] public Guid Id { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? ParentId { get; set; }
    public char? AddLoginHint { get; set; }
    public bool IsGroup  { get; set; }
    public bool TargetBlank { get; set; }
    public int Order { get; set; }
    [StringLength(30)] public string Text { get; set; } = string.Empty;
    [StringLength(150)] public string? Url { get; set; }

    public DbCustomers? Customer { get; set; }
    public DbMenu? Parent { get; set; }
    public List<DbMenu>? Children { get; set; }
}