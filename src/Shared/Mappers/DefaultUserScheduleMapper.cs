using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Mappers;

public static class DefaultUserScheduleMapper
{
    public static PatchDefaultUserSchedule ToPatchDefaultUserSchedule(this DefaultUserSchedule defaultUserSchedule)
    {
        return new PatchDefaultUserSchedule
        {
            UserDefaultAvailableId = defaultUserSchedule.UserDefaultAvailableId,
            Available = defaultUserSchedule.Available,
            ValidFromUser = defaultUserSchedule.ValidFromUser,
            ValidUntilUser = defaultUserSchedule.ValidUntilUser,
            Assigned = defaultUserSchedule.Assigned,
        };
    }
}
