using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

namespace Drogecode.Knrm.Oefenrooster.Shared.Mappers;

public static class DefaultUserScheduleMapper
{
    public static PatchDefaultUserSchedule ToPatchDefaultUserSchedule(this DefaultUserSchedule defaultUserSchedule)
    {
        return new PatchDefaultUserSchedule
        {
            UserDefaultAvailableId = defaultUserSchedule.UserDefaultAvailableId,
            GroupId = defaultUserSchedule.GroupId,
            Availability = defaultUserSchedule.Availability,
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
                Availability = item.Availability,
                ValidFromUser = item.ValidFromUser,
                ValidUntilUser = item.ValidUntilUser,
                Assigned = item.Assigned,
            });
        }
        return clone;
    }
}
