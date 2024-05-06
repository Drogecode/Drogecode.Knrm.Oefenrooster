using MudBlazor;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;

public class PlannerTrainingType : ICloneable
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string ColorLight { get; set; } = Colors.Gray.Lighten1;
    public string ColorDark { get; set; } = string.Empty;
    public string? TextColorLight { get; set; }
    public string? TextColorDark { get; set; }
    public int Order { get; set; } = -1;
    public bool CountToTrainingTarget { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }


    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
