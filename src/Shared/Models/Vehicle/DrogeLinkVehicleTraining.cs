﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;

public class DrogeLinkVehicleTraining
{
    public Guid? Id { get; set; }
    public Guid RoosterTrainingId { get; set; }
    public Guid VehicleId { get; set; }
    public bool IsSelected { get; set; }
}
