using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class MultiplePlannerTrainingTypesResponse : BaseResponse
{
    public List<PlannerTrainingType>? PlannerTrainingTypes { get; set; }
}
