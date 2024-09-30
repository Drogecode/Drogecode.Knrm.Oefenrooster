using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Enums;

public enum ItemUpdated
{
    None = 0,
    FutureTrainings = 1,
    AllUsers = 2,
    AllVehicles = 3,
    AllTrainingTypes = 4,
    AllFunctions = 5,
    AllFutureDayItems = 6,
    DayItemDashboard = 7,
    PinnedDashboard = 8,
    FutureHolidays = 9,
}
