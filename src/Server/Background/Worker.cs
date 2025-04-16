using Drogecode.Knrm.Oefenrooster.PreCom;
using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Server.Models.UserPreCom;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Background;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _memoryCache;
    private readonly IDateTimeService _dateTimeService;
    private CancellationToken _clt;

    private const string NEXT_USER_SYNC = "all_usr_sync";
    private int _errorCount = 0;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration, IMemoryCache memoryCache, IDateTimeService dateTimeService)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _memoryCache = memoryCache;
        _dateTimeService = dateTimeService;
    }

    protected override async Task ExecuteAsync(CancellationToken clt)
    {
        _clt = clt;
        _memoryCache.Set(NEXT_USER_SYNC, DateTime.SpecifyKind(DateTime.Today.AddDays(1).AddHours(1), DateTimeKind.Utc)); // do not run on startup
        int count = 0;
        while (!_clt.IsCancellationRequested && _configuration.GetValue<bool>("Drogecode:RunBackgroundService"))
        {
            try
            {
                var sleep = 60 * (_errorCount * 3 + 1);
                for (int i = 0; i < sleep; i++)
                {
                    if (_clt.IsCancellationRequested) return; // run once every second.
#if DEBUG
                    await Task.Delay(100, clt);
#else
                    await Task.Delay(1000, clt);
#endif
                }

                using var scope = _scopeFactory.CreateScope();
                var graphService = scope.ServiceProvider.GetRequiredService<IGraphService>();
                graphService.InitializeGraph();
                var successfully = await SyncSharePoint(scope, graphService, clt);
                if (count % 15 == 6) // Every 15 runs, but not directly after restart.
                    successfully = await SyncPreComAvailability(scope, clt) && successfully;
                count++;
                if (count > 10000)
                    count = 0;
                if (successfully && _errorCount > 0)
                {
                    _errorCount--;
                    _logger.LogInformation("No error in worker, decreasing counter with one `{errorCount}`", _errorCount);
                }
                else if (!successfully)
                {
                    _errorCount++;
                    _logger.LogWarning("Error in worker, increasing counter with one `{errorCount}`", _errorCount);
                }
            }
            catch (Exception ex)
            {
                _errorCount++;
                _logger.LogError(ex, "Error `{errorCount}` in worker", _errorCount);
            }

            if (_clt.IsCancellationRequested) return;
        }
    }

    private async Task<bool> SyncSharePoint(IServiceScope scope, IGraphService graphService, CancellationToken clt)
    {
        var result = true;
        result = await SyncSharePointActions(graphService) && result;
        result = await SyncSharePointUsers(scope, graphService) && result;
        result = await SyncCalendarEvents(scope, graphService, clt) && result;
        return result;
    }

    private async Task<bool> SyncSharePointActions(IGraphService graphService)
    {
        var result = true;
        result = (await RunBackgroundTask(async () => await graphService.SyncSharePointActions(DefaultSettingsHelper.KnrmHuizenId, _clt), "SyncSharePointActions", _clt) && result);
        result = (await RunBackgroundTask(async () => await graphService.SyncSharePointTrainings(DefaultSettingsHelper.KnrmHuizenId, _clt), "SyncSharePointTrainings", _clt) && result);
        return result;
    }

    private async Task<bool> SyncSharePointUsers(IServiceScope scope, IGraphService graphService)
    {
        var nextSync = _memoryCache.Get<DateTime?>(NEXT_USER_SYNC);
        if (nextSync is not null && nextSync.Value.CompareTo(DateTime.UtcNow) > 0)
            return true;

        var userControllerLogger = scope.ServiceProvider.GetRequiredService<ILogger<UserController>>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var userRoleService = scope.ServiceProvider.GetRequiredService<IUserRoleService>();
        var linkUserRoleService = scope.ServiceProvider.GetRequiredService<ILinkUserRoleService>();
        var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
        var functionService = scope.ServiceProvider.GetRequiredService<IFunctionService>();
        var refreshHub = scope.ServiceProvider.GetRequiredService<RefreshHub>();
        var userController = new UserController(userControllerLogger, userService, userRoleService, linkUserRoleService, auditService, graphService, functionService, refreshHub);
        await userController.InternalSyncAllUsers(DefaultSettingsHelper.SystemUser, DefaultSettingsHelper.KnrmHuizenId, _clt);

        _clt.ThrowIfCancellationRequested();
        _memoryCache.Set(NEXT_USER_SYNC, DateTime.SpecifyKind(DateTime.Today.AddDays(1).AddHours(1), DateTimeKind.Utc));
        return true;
    }

    private async Task<bool> SyncCalendarEvents(IServiceScope scope, IGraphService graphService, CancellationToken clt)
    {
        var userLastCalendarUpdateService = scope.ServiceProvider.GetRequiredService<IUserLastCalendarUpdateService>();
        var scheduleService = scope.ServiceProvider.GetRequiredService<IScheduleService>();
        var usersToUpdate = await userLastCalendarUpdateService.GetLastUpdateUsers(clt);
        foreach (var user in usersToUpdate)
        {
            var availables = await scheduleService.GetTrainingsThatRequireCalendarUpdate(user.UserId, user.CustomerId);
            foreach (var ava in availables)
            {
                // ToDo: Sync with calendar
            }
        }

        return true;
    }

    private async Task<bool> SyncPreComAvailability(IServiceScope scope, CancellationToken clt)
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
        var userIdsWithNull = await userSettingService.GetAllSyncPreComWithCalendarSetting(DefaultSettingsHelper.KnrmHuizenId, clt); //new List<int> { 37398 };
        if (userIdsWithNull.Count == 0)
            return true;
        var itemsSynced = 0;
        for (var i = 0; i < 5; i++)
        {
            itemsSynced += await LoopSyncPreComAvailability(userIdsWithNull, date.AddDays(i), preComWorker, userService, customerSettingService, userPreComEventService, clt);
        }

        _logger.LogInformation("Synced `{items}` from PreCom to outlook", itemsSynced);
        return true;
    }

    private async Task<int> LoopSyncPreComAvailability(List<SyncPreComWithCalendarSetting> userIdsWithNull, DateTime date, AvailabilityForUser preComWorker, IUserService userService,
        ICustomerSettingService customerSettingService, IUserPreComEventService userPreComEventService, CancellationToken clt)
    {
        var userIds = userIdsWithNull.Where(x=>x is { Value: true, UserPreComId: not null }).Select(x=>x.UserPreComId!.Value).ToList();
        var preComAvailability = await preComWorker.Get(userIds, date);
        if (preComAvailability?.Users is null)
            return 0;
        var itemsSynced = 0;
        foreach (var userSyncPreComWithCalendarSetting in userIdsWithNull.Where(x=> x.UserPreComId is not null))
        {
            var user = preComAvailability.Users.FirstOrDefault(x=>x.UserId == userSyncPreComWithCalendarSetting.UserPreComId!.Value);
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

    private async Task<bool> RunBackgroundTask<TRet>(Func<Task<TRet>> function, string name, CancellationToken clt)
    {
        try
        {
            await function();
            clt.ThrowIfCancellationRequested();
            return true;
        }
        catch (Exception ex)
        {
            _errorCount++;
            _logger.LogError(ex, "Error in background service `{name}` {errorCount}`", name, _errorCount);
            clt.ThrowIfCancellationRequested();
            return false;
        }
    }
}