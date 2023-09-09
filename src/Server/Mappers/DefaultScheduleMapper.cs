using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class DefaultScheduleMapper
{
    public static DbRoosterDefault ToDbRoosterDefault(this DefaultSchedule defaultSchedule, Guid customerId, string timeZone = "")
    {
        return new DbRoosterDefault
        {
            Id = defaultSchedule.Id,
            RoosterTrainingTypeId = defaultSchedule.RoosterTrainingTypeId,
            WeekDay = defaultSchedule.WeekDay,
            TimeStart = defaultSchedule.TimeStart,
            TimeEnd = defaultSchedule.TimeEnd,
            ValidFrom = defaultSchedule.ValidFromDefault,
            ValidUntil = defaultSchedule.ValidUntilDefault,
            CountToTrainingTarget = defaultSchedule.CountToTrainingTarget,
            Order = defaultSchedule.Order,
            CustomerId = customerId,
            TimeZone = timeZone
        };
    }
    public static DefaultSchedule ToDefaultSchedule(this DbRoosterDefault defaultSchedule)
    {
        return new DefaultSchedule
        {
            Id = defaultSchedule.Id,
            RoosterTrainingTypeId = defaultSchedule.RoosterTrainingTypeId,
            WeekDay = defaultSchedule.WeekDay,
            TimeStart = defaultSchedule.TimeStart,
            TimeEnd = defaultSchedule.TimeEnd,
            ValidFromDefault = defaultSchedule.ValidFrom,
            ValidUntilDefault = defaultSchedule.ValidUntil,
            CountToTrainingTarget = defaultSchedule.CountToTrainingTarget,
            Order = defaultSchedule.Order,
        };
    }
}
