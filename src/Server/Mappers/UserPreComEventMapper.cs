using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Models.UserPreCom;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class UserPreComEventMapper
{
    public static UserPreComEvent ToUserPreComEvent(this DbUserPreComEvent dbUserPreComEvent)
    {
        return new UserPreComEvent()
        {
            Id = dbUserPreComEvent.Id,
            UserId = dbUserPreComEvent.UserId,
            CalendarEventId = dbUserPreComEvent.CalendarEventId,
            Start = dbUserPreComEvent.Start,
            End = dbUserPreComEvent.End,
            IsFullDay = dbUserPreComEvent.IsFullDay,
            SyncWithExternal = dbUserPreComEvent.SyncWithExternal,
        };
    }
}