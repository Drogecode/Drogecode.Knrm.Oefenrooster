using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;

public sealed class RoosterItemDay : ICloneable
{
    public Guid Id { get; set; }
    public List<RoosterItemDayLinkedUsers>? LinkedUsers { get; set; }
    public DateTime? DateStart { get; set; }
    public DateTime? DateEnd { get; set; }
    public bool IsFullDay { get; set; }
    public CalendarItemType Type { get; set; }
    public string Text { get; set; } = string.Empty;

    public object Clone()
    {
        return MemberwiseClone();
    }
}
