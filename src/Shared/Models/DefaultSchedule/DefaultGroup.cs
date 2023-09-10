using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

public class DefaultGroup
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public bool IsDefault { get; set; } = false;
}
