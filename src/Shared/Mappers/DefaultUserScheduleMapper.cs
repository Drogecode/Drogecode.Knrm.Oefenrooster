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
            GroupId = defaultUserSchedule.GroupId,
            Available = defaultUserSchedule.Available,
            ValidFromUser = defaultUserSchedule.ValidFromUser,
            ValidUntilUser = defaultUserSchedule.ValidUntilUser,
            Assigned = defaultUserSchedule.Assigned,
        };
    }

    public static List<DefaultUserSchedule> DeepCloon(this List<DefaultUserSchedule> original)
    {
        List<DefaultUserSchedule> clone = new List<DefaultUserSchedule>();
        foreach(var item in original)
        {
            clone.Add(new DefaultUserSchedule
            {
                UserDefaultAvailableId = item.UserDefaultAvailableId,
                Available = item.Available,
                ValidFromUser = item.ValidFromUser,
                ValidUntilUser = item.ValidUntilUser,
                Assigned = item.Assigned,
            });
        }
        return clone;
    }
}
