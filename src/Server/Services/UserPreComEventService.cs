using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Models.Background;
using Drogecode.Knrm.Oefenrooster.Server.Models.UserPreCom;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserPreComEventService(
    ILogger<CustomerService> logger,
    DataContext database,
    IMemoryCache memoryCache,
    IDateTimeProvider dateTimeProvider,
    IUserSettingService _userSettingService,
    IUserLinkedMailsService _userLinkedMailsService,
    IGraphService _graphService)
    : DrogeService(logger, database, memoryCache, dateTimeProvider), IUserPreComEventService
{
    public async Task<List<UserPreComEvent>> GetEventsForUserForDay(Guid userId, Guid customerId, DateOnly date, CancellationToken clt)
    {
        return await Database.UserPreComEvents
            .Where(x => x.UserId == userId && x.CustomerId == customerId && x.Date == date)
            .Select(x => x.ToUserPreComEvent())
            .AsNoTracking()
            .ToListAsync(clt);
    }

    public async Task<bool> RemoveEvent(DrogeUser drogeUser, UserPreComEvent userPreComEvent, CancellationToken clt)
    {
        if (userPreComEvent.CalendarEventId is null)
            return false;
        await _graphService.DeleteCalendarEvent(drogeUser.ExternalId, userPreComEvent.CalendarEventId, clt);
        var dbEvent = await Database.UserPreComEvents.FirstOrDefaultAsync(x => x.Id == userPreComEvent.Id);
        if (dbEvent is null)
            return false;
        Database.UserPreComEvents.Remove(dbEvent);
        return await SaveDb(clt) > 0;
    }

    public async Task<bool> AddEvent(DrogeUser drogeUser, PreComPeriod period, DateOnly date, bool syncWithExternal, CancellationToken clt)
    {
        var text = (await _userSettingService.GetStringUserSetting(drogeUser.CustomerId, drogeUser.Id, SettingName.PreComAvailableText, string.Empty, clt)).Value;
        if (string.IsNullOrWhiteSpace(text))
            text = "Piket";

        var start = period.Start;
        var end = period.End;
        if (period.IsFullDay)
        {
            start = period.Date.ToDateTime(new TimeOnly(0, 0, 0));
            end = period.Date.ToDateTime(new TimeOnly(0, 0, 0));
        }

        List<UserLinkedMail> attendees = [];
        if (syncWithExternal)
        {
            attendees = (await _userLinkedMailsService.AllUserLinkedMail(30, 0, drogeUser.Id, drogeUser.CustomerId, true, clt)).UserLinkedMails ?? [];
        }
        
        var newEvent = await _graphService.AddToCalendar(drogeUser.ExternalId, text, start, end, period.IsFullDay, FreeBusyStatus.Free, attendees);
        if (newEvent is null)
            return false;
        Database.UserPreComEvents.Add(new DbUserPreComEvent
        {
            Id = Guid.CreateVersion7(),
            CalendarEventId = newEvent.Id,
            UserId = drogeUser.Id,
            CustomerId = drogeUser.CustomerId,
            Start = period.Start,
            End = period.End,
            Date = date,
            Text = text,
            IsFullDay = period.IsFullDay,
            SyncWithExternal = syncWithExternal
        });
        return await SaveDb(clt) > 0;
    }
}