using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

public abstract class TrainingBase
{
    public string? Name { get; set; }
    public virtual Guid? RoosterTrainingTypeId { get; set; }
}
