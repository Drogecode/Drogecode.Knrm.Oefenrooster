using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;

public class Holiday : ICloneable
{
    public string? Description { get; set; }
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Availabilty? Availabilty { get; set; }
    public Availability? Availability { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
