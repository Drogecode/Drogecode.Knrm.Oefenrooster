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
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public MudColor ColorLight { get; set; } = Colors.Grey.Lighten1;
    public MudColor ColorDark { get; set; } = "rgba(255,255,255, 0.3)";
    public bool CountToTrainingTarget { get; set; }
}
