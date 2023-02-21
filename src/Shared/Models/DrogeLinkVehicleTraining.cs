using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models;

public class DrogeLinkVehicleTraining
{
    public Guid RoosterTrainingId { get; set; }
    public Guid Vehicle { get; set; }
    public bool IsSelected { get; set; }
}
