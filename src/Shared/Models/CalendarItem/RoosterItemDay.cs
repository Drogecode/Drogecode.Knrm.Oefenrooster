using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;

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
        return this.MemberwiseClone();
    }
}
