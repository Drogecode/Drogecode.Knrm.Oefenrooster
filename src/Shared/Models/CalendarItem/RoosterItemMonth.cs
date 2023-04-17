using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;

public class RoosterItemMonth
{
    [Key] public Guid Id { get; set; }
    public short Month { get; set; }
    /// <summary>
    /// null for recurring
    /// </summary>
    public short? Year { get; set; }
    public CalendarItemType Type { get; set; }
    public Severity Severity { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
}
