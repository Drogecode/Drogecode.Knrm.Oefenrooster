using MudBlazor;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PlannerTrainingType
{
    private static MudColor d = "rgba(255,255,255, 0.3)";
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string ColorLight { get; set; } = Colors.Grey.Lighten1;
    public string ColorDark { get; set; } = d.ToString();
    public bool CountToTrainingTarget { get; set; }
    public bool IsDefault { get; set; }
}
