namespace Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

public class DefaultGroup : ICloneable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public bool IsDefault { get; set; } = false;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
