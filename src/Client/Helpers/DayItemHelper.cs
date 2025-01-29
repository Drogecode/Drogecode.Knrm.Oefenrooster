using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Client.Helpers;

public static class DayItemHelper
{
    public static Color DayItemTypeColor(CalendarItemType type)
    {
        return type switch
        {
            CalendarItemType.Custom => Color.Default,
            CalendarItemType.PersonPublic => Color.Info,
            CalendarItemType.Default => Color.Primary,
            CalendarItemType.SpecialDate => Color.Tertiary,
            _ => Color.Secondary,
        };
    }
    public static string GetColor(Color color) => $"var(--mud-palette-{color.ToDescriptionString()})";
}
