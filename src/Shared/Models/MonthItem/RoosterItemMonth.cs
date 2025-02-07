using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;

public class RoosterItemMonth : ICloneable
{
    [Key] public Guid Id { get; set; }
    public short Month { get; set; }
    /// <summary>
    /// null for recurring
    /// </summary>
    public short? Year { get; set; }
    public CalendarItemType Type { get; set; }
    public Severity Severity { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_MONTH_ITEM_TEXT)] public string Text { get; set; } = string.Empty;
    public int Order { get; set; }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
