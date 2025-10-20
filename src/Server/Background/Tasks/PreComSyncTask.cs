using Drogecode.Knrm.Oefenrooster.PreCom;
using Drogecode.Knrm.Oefenrooster.Server.Models.Background;
using Drogecode.Knrm.Oefenrooster.Server.Models.UserPreCom;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.Server.Background.Tasks;

public class PreComSyncTask(ILogger _logger, IDateTimeProvider dateTimeProvider)
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
        var preComWorker = new AvailabilityForUser(preComClient, _logger, dateTimeProvider);

        var date = DateTime.Today;
        var dayCount = await customerSettingService.GetIntCustomerSetting(DefaultSettingsHelper.KnrmHuizenId, SettingName.PreComDaysInFuture, 5, clt);
        var userIdsWithNull = await userSettingService.GetAllPreComIdAndValue(DefaultSettingsHelper.KnrmHuizenId, SettingName.SyncPreComWithCalendar, clt); //new List<int> { 37398 };
        if (userIdsWithNull.Count == 0)
            return true;
        var itemsSynced = 0;
        for (var i = 0; i < dayCount.Value; i++)
        {
            // Check future availability
            itemsSynced += await LoopSyncPreComAvailability(userIdsWithNull, date.AddDays(i), false, preComWorker, userService, customerSettingService, userPreComEventService, userSettingService,
                clt);
            await Task.Delay(100, clt); // 0.1 s Do not spam PreCom api
        }

        userIdsWithNull = await userSettingService.GetAllPreComIdAndValue(DefaultSettingsHelper.KnrmHuizenId, SettingName.SyncPreComDeleteOld, clt);
        if (userIdsWithNull.Count != 0)
        {
            // Delete old availability from Outlook
            var usersToDeleteOld = userIdsWithNull.Where(x => x is { UserPreComId: not null, Value: true }).ToList();
            itemsSynced += await LoopSyncPreComAvailability(usersToDeleteOld, date.AddDays(-7), true, preComWorker, userService, customerSettingService, userPreComEventService, userSettingService,
                clt);
        }

        _logger.LogInformation("Synced `{items}` from PreCom to outlook", itemsSynced);
        return true;
    }

    private async Task<int> LoopSyncPreComAvailability(List<UserPreComIdAndValue> userIdsWithNull, DateTime date, bool onlyDelete, AvailabilityForUser preComWorker, IUserService userService,
        ICustomerSettingService customerSettingService, IUserPreComEventService userPreComEventService, IUserSettingService userSettingService, CancellationToken clt)
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
            var drogeUser = await userService.GetUserByPreComId(userSyncPreComWithCalendarSetting.UserPreComId!.Value, DefaultSettingsHelper.KnrmHuizenId, clt);
            if (drogeUser is null)
            {
                _logger.LogWarning("No user found with PreCom id `{PreComId}`", userSyncPreComWithCalendarSetting.UserPreComId);
                continue;
            }

            var timeZone = await customerSettingService.GetTimeZone(drogeUser.CustomerId);
            var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var periods = new List<PreComPeriod>();
            DateTime? start = null;
            var isFullDay = true;
            if (user?.AvailabilitySets is not null)
            {
                foreach (var availabilitySet in user.AvailabilitySets.OrderBy(x => x.Start))
                {
                    if (start is null && availabilitySet.Available)
                    {
                        start = availabilitySet.Start;
                        continue;
                    }

                    if (availabilitySet.Available)
                    {
                        continue;
                    }

                    isFullDay = false;

                    if (start is null)
                    {
                        continue;
                    }

                    var startConverted = TimeZoneInfo.ConvertTimeToUtc(start.Value, zone);
                    var end = TimeZoneInfo.ConvertTimeToUtc(availabilitySet.Start, zone);
                    periods.Add(new PreComPeriod
                    {
                        Start = startConverted,
                        End = end,
                        Date = DateOnly.FromDateTime(date),
                        IsFullDay = false
                    });
                    start = null;
                }
            }

            if (start is not null)
            {
                var startConverted = TimeZoneInfo.ConvertTimeToUtc(start.Value, zone);
                var end = TimeZoneInfo.ConvertTimeToUtc(start.Value.AddDays(1).Date, zone);
                periods.Add(new PreComPeriod
                {
                    Start = startConverted,
                    End = end,
                    Date = DateOnly.FromDateTime(date),
                    IsFullDay = isFullDay
                });
            }

            clt.ThrowIfCancellationRequested();

            itemsSynced += await SyncWithUserCalendar(drogeUser, periods, DateOnly.FromDateTime(date), userPreComEventService, userSettingService, clt);
        }

        return itemsSynced;
    }

    private async Task<int> SyncWithUserCalendar(DrogeUser drogeUser, List<PreComPeriod> periods, DateOnly date, IUserPreComEventService userPreComEventService, IUserSettingService userSettingService,
        CancellationToken clt)
    {
        var userPreComEvents = await userPreComEventService.GetEventsForUserForDay(drogeUser.Id, drogeUser.CustomerId, date, clt);
        var syncWithExternal = await userSettingService.GetBoolUserSetting(drogeUser.CustomerId, drogeUser.Id, SettingName.SyncPreComWithExternal, true, clt);
        var notFound = new List<int>();
        var itemsSynced = 0;
        for (var i = 0; i < userPreComEvents.Count; i++)
        {
            var period = periods.FirstOrDefault(x => x.Start.CompareTo(userPreComEvents[i].Start) == 0 && x.End.CompareTo(userPreComEvents[i].End) == 0);
            if (period is not null && period.IsFullDay == userPreComEvents[i].IsFullDay && userPreComEvents[i].SyncWithExternal == syncWithExternal.Value)
            {
                periods.Remove(period);
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
            await userPreComEventService.AddEvent(drogeUser, period, date, syncWithExternal.Value, clt);
            itemsSynced++;
        }

        return itemsSynced;
    }
}