using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;

public class MultipleVehicleTrainingLinkResponse : BaseResponse
{
    public List<DrogeLinkVehicleTraining>? DrogeLinkVehicleTrainingLinks { get; set; }
}
