namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Menu;

public class DrogeMenu
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public char? AddLoginHint { get; set; }
    public bool IsGroup  { get; set; }
    public bool TargetBlank { get; set; }
    public int Order { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Url { get; set; }
}