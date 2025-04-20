using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Models.UserPreCom;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserPreComEventService(
    ILogger<CustomerService> logger,
    DataContext database,
    IMemoryCache memoryCache,
    IDateTimeService dateTimeService,
    IUserSettingService _userSettingService,
    IGraphService _graphService)
    : DrogeService(logger, database, memoryCache, dateTimeService), IUserPreComEventService
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

    public async Task<bool> AddEvent(DrogeUser drogeUser, DateTime start, DateTime end, DateOnly date, CancellationToken clt)
    {
        var text = (await _userSettingService.GetStringUserSetting(drogeUser.CustomerId, drogeUser.Id, SettingName.PreComAvailableText)).Value;
        if (string.IsNullOrWhiteSpace(text))
            text = "Piket";

        var newEvent = await _graphService.AddToCalendar(drogeUser.ExternalId, text, start, end, false, []);
        if (newEvent is null)
            return false;
        Database.UserPreComEvents.Add(new DbUserPreComEvent()
        {
            Id = Guid.CreateVersion7(),
            CalendarEventId = newEvent.Id,
            UserId = drogeUser.Id,
            CustomerId = drogeUser.CustomerId,
            Start = start,
            End = end,
            Date = date,
            Text = text
        });
        return await SaveDb(clt) > 0;
    }
}