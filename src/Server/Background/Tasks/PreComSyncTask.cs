using Drogecode.Knrm.Oefenrooster.PreCom;
using Drogecode.Knrm.Oefenrooster.Server.Models.UserPreCom;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.Server.Background.Jobs;

public class PreComSyncTask(ILogger _logger, IDateTimeService _dateTimeService)
{
    public async Task<bool> SyncPreComAvailability(IServiceScope scope, CancellationToken clt)
    {
        var preComService = scope.ServiceProvider.GetRequiredService<IPreComService>();
        var userPreComEventService = scope.ServiceProvider.GetRequiredService<IUserPreComEventService>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var customerSettingService = scope.ServiceProvider.GetRequiredService<ICustomerSettingService>();
        var userSettingService = scope.ServiceProvider.GetRequiredService<IUserSettingService>();
        var preComClient = await preComService.GetPreComClient();
        if (preComClient is null)
            return true;
        var preComWorker = new AvailabilityForUser(preComClient, _logger, _dateTimeService);

        var date = DateTime.Today;
        var userIdsWithNull = await userSettingService.GetAllPreComIdAndValue(DefaultSettingsHelper.KnrmHuizenId, SettingName.SyncPreComWithCalendar, clt); //new List<int> { 37398 };
        if (userIdsWithNull.Count == 0)
            return true;
        var itemsSynced = 0;
        for (var i = 0; i < 5; i++)
        {
            // Check future availability
            itemsSynced += await LoopSyncPreComAvailability(userIdsWithNull, date.AddDays(i), false, preComWorker, userService, customerSettingService, userPreComEventService, clt);
        }
        userIdsWithNull = await userSettingService.GetAllPreComIdAndValue(DefaultSettingsHelper.KnrmHuizenId, SettingName.SyncPreComDeleteOld, clt);
        if (userIdsWithNull.Count != 0)
        {
            // Delete old availability from outlook
            var usersToDeleteOld = userIdsWithNull.Where(x => x is { UserPreComId: not null, Value: true }).ToList();
            itemsSynced += await LoopSyncPreComAvailability(usersToDeleteOld, date.AddDays(-7), true, preComWorker, userService, customerSettingService, userPreComEventService, clt);
        }
        _logger.LogInformation("Synced `{items}` from PreCom to outlook", itemsSynced);
        return true;
    }

    private async Task<int> LoopSyncPreComAvailability(List<UserPreComIdAndValue> userIdsWithNull, DateTime date, bool onlyDelete, AvailabilityForUser preComWorker, IUserService userService,
        ICustomerSettingService customerSettingService, IUserPreComEventService userPreComEventService, CancellationToken clt)
    {
        GetResponse? preComAvailability = null;
        if (!onlyDelete)
        {
            var userIds = userIdsWithNull.Where(x => x is { Value: true, UserPreComId: not null }).Select(x => x.UserPreComId!.Value).ToList();
            preComAvailability = await preComWorker.Get(userIds, date);
        }

        var itemsSynced = 0;
        foreach (var userSyncPreComWithCalendarSetting in userIdsWithNull.Where(x => x.UserPreComId is not null))
        {
            var user = preComAvailability?.Users?.FirstOrDefault(x => x.UserId == userSyncPreComWithCalendarSetting.UserPreComId!.Value);
            var drogeUser = await userService.GetUserByPreComId(userSyncPreComWithCalendarSetting.UserPreComId!.Value, clt);
            if (drogeUser is null)
            {
                _logger.LogWarning("No user found with PreCom id `{PreComId}`", userSyncPreComWithCalendarSetting.UserPreComId);
                continue;
            }

            var timeZone = await customerSettingService.GetTimeZone(drogeUser.CustomerId);
            var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var periods = new Dictionary<DateTime, DateTime>();
            DateTime? start = null;
            if (user?.AvailabilitySets is not null)
            {
                foreach (var availabilitySet in user.AvailabilitySets.OrderBy(x => x.Start))
                {
                    if (start is null && availabilitySet.Available)
                    {
                        start = availabilitySet.Start;
                        continue;
                    }

                    if (availabilitySet.Available || start is null)
                    {
                        continue;
                    }

                    var startConverted = TimeZoneInfo.ConvertTimeToUtc(start.Value, zone);
                    var end = TimeZoneInfo.ConvertTimeToUtc(availabilitySet.Start, zone);
                    periods.Add(startConverted, end);
                    start = null;
                }
            }

            if (start is not null)
            {
                var startConverted = TimeZoneInfo.ConvertTimeToUtc(start.Value, zone);
                var end = TimeZoneInfo.ConvertTimeToUtc(start.Value.AddDays(1).Date, zone);
                periods.Add(startConverted, end);
            }

            clt.ThrowIfCancellationRequested();

            itemsSynced += await SyncWithUserCalendar(drogeUser, periods, DateOnly.FromDateTime(date), userPreComEventService, clt);
        }

        return itemsSynced;
    }

    private async Task<int> SyncWithUserCalendar(DrogeUser drogeUser, Dictionary<DateTime, DateTime> periods, DateOnly date, IUserPreComEventService userPreComEventService, CancellationToken clt)
    {
        var userPreComEvents = await userPreComEventService.GetEventsForUserForDay(drogeUser.Id, drogeUser.CustomerId, date, clt);
        var notFound = new List<int>();
        var itemsSynced = 0;
        for (var i = 0; i < userPreComEvents.Count; i++)
        {
            if (periods.Any(x => x.Key.CompareTo(userPreComEvents[i].Start) == 0 && x.Value.CompareTo(userPreComEvents[i].End) == 0))
            {
                periods.Remove(userPreComEvents[i].Start);
                continue;
            }

            notFound.Add(i);
        }

        foreach (var indexToRemove in notFound)
        {
            await userPreComEventService.RemoveEvent(drogeUser, userPreComEvents[indexToRemove], clt);
            itemsSynced++;
        }

        foreach (var period in periods)
        {
            await userPreComEventService.AddEvent(drogeUser, period.Key, period.Value, date, clt);
            itemsSynced++;
        }

        return itemsSynced;
    }
}