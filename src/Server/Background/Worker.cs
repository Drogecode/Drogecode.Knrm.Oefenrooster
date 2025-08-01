using Drogecode.Knrm.Oefenrooster.Server.Background.Tasks;
using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Managers.Interfaces;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Background;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _memoryCache;
    private readonly IDateTimeProvider _dateTimeProvider;
    private CancellationToken _clt;

    private const string NEXT_USER_SYNC = "all_usr_sync";
    private int _errorCount = 0;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration, IMemoryCache memoryCache, IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _memoryCache = memoryCache;
        _dateTimeProvider = dateTimeProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken clt)
    {
        _clt = clt;
        _memoryCache.Set(NEXT_USER_SYNC, DateTime.SpecifyKind(DateTime.Today.AddDays(1).AddHours(1), DateTimeKind.Utc)); // do not run on startup
        var count = 0;
        while (!_clt.IsCancellationRequested && _configuration.GetValue<bool>("Drogecode:RunBackgroundService"))
        {
            try
            {
                var sleep = 60 * (_errorCount * 3 + 1);
                for (var i = 0; i < sleep; i++)
                {
                    if (_clt.IsCancellationRequested) return; // run once every second.
#if DEBUG
                    await Task.Delay(100, clt);
#else
                    await Task.Delay(1000, clt);
#endif
                }

                using var scope = _scopeFactory.CreateScope();
                var authenticationManager = scope.ServiceProvider.GetRequiredService<IAuthenticationManager>();
                var authService = authenticationManager.GetAuthenticationService();
                var tenantId = authService.GetTenantId();
                
                var graphService = scope.ServiceProvider.GetRequiredService<IGraphService>();
                graphService.InitializeGraph();
                
                var successfully = await SyncSharePoint(scope, graphService, tenantId, clt);
                if (tenantId.Equals(DefaultSettingsHelper.KnrmHuizenId.ToString()) && count % 15 == 6) // Every 15 runs, but not directly after restart.
                {
                    var preComSyncJob = new PreComSyncTask(_logger, _dateTimeProvider);
                    successfully &= await preComSyncJob.SyncPreComAvailability(scope, clt);
                }

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

    private async Task<bool> SyncSharePoint(IServiceScope scope, IGraphService graphService, string tenantId, CancellationToken clt)
    {
        var result = true;
        if (tenantId.Equals(DefaultSettingsHelper.KnrmHuizenId.ToString()))
        {
            result &= await SyncSharePointReports(graphService);
        }

        result &= await SyncSharePointUsers(scope, graphService);
        result &= await SyncCalendarEvents(scope, graphService, clt);
        return result;
    }

    private async Task<bool> SyncSharePointReports(IGraphService graphService)
    {
        var result = true;
        result &= (await RunBackgroundTask(async () => await graphService.SyncSharePointActions(DefaultSettingsHelper.KnrmHuizenId, _clt), "SyncSharePointActions", _clt));
        result &= (await RunBackgroundTask(async () => await graphService.SyncSharePointTrainings(DefaultSettingsHelper.KnrmHuizenId, _clt), "SyncSharePointTrainings", _clt));
        return result;
    }

    private async Task<bool> SyncSharePointUsers(IServiceScope scope, IGraphService graphService)
    {
        var nextSync = _memoryCache.Get<DateTime?>(NEXT_USER_SYNC);
        if (nextSync is not null && nextSync.Value.CompareTo(DateTime.UtcNow) > 0)
            return true;

        // Get scoped objects
        var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();

        // Get the managers
        var authenticationManager = scope.ServiceProvider.GetRequiredService<IAuthenticationManager>();
        var userSyncManager = scope.ServiceProvider.GetRequiredService<IUserSyncManager>();

        // Get tenant details
        var authService = authenticationManager.GetAuthenticationService();
        var customersInTenant = await customerService.GetByTenantId(authService.GetTenantId(), _clt);

        var response = true;
        foreach (var customer in customersInTenant)
        {
            try
            {
                _clt.ThrowIfCancellationRequested();
                _logger.LogInformation("Syncing users for customer `{customerId}`", customer.Id);
                await userSyncManager.SyncAllUsers(DefaultSettingsHelper.SystemUser, customer.Id, _clt);
                await Task.Delay(100, _clt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing {customer}", customer.Id);
                response = false;
            }
        }

        _memoryCache.Set(NEXT_USER_SYNC, DateTime.SpecifyKind(DateTime.Today.AddDays(1).AddHours(1), DateTimeKind.Utc));
        return response;
    }

    private async Task<bool> SyncCalendarEvents(IServiceScope scope, IGraphService graphService, CancellationToken clt)
    {
        var minutesInThePast = 10;
#if DEBUG
        minutesInThePast = 1;
#endif
        var userLastCalendarUpdateService = scope.ServiceProvider.GetRequiredService<IUserLastCalendarUpdateService>();
        var scheduleService = scope.ServiceProvider.GetRequiredService<IScheduleService>();
        var scheduleController = scope.ServiceProvider.GetRequiredService<ScheduleController>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var functionService = scope.ServiceProvider.GetRequiredService<IFunctionService>();
        var usersToUpdate = await userLastCalendarUpdateService.GetLastUpdateUsers(minutesInThePast, 60, clt);
        foreach (var user in usersToUpdate)
        {
            var availabilities = await scheduleService.GetTrainingsThatRequireCalendarUpdate(user.UserId, user.CustomerId);
            foreach (var ava in availabilities)
            {
                var thisUser = await userService.GetUserById(ava.CustomerId, ava.UserId, true, clt);
                if (thisUser is null)
                {
                    _logger.LogWarning("User `{userId}` in customer `{customer}` not found", ava.UserId, ava.CustomerId);
                    continue;
                }

                DrogeFunction? function = null;
                if (ava.UserFunctionId is not null && thisUser.UserFunctionId is not null && thisUser.UserFunctionId != ava.UserFunctionId)
                {
                    function = await functionService.GetById(ava.CustomerId, ava.UserFunctionId.Value, clt);
                }

                await scheduleController.ToOutlookCalendar(ava.UserId, thisUser.ExternalId, ava.TrainingId, ava.Assigned, ava.Training.ToPlannedTraining(), user.UserId, ava.CustomerId,
                    ava.Id, ava.CalendarEventId, function?.Name, true, clt);
                await Task.Delay(100, clt); // 0.1 s Do not spam outlook
            }
        }

        return true;
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
            _logger.LogError(ex, "Error in background service `{name}` {errorCount}`", name, _errorCount);
            clt.ThrowIfCancellationRequested();
            return false;
        }
    }
}