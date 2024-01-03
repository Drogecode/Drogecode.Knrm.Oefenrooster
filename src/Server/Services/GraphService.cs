using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph.Models;
using MudBlazor.Services;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class GraphService : IGraphService
{
    private readonly ILogger<GraphService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;
    private readonly DataContext _database;

    private const string SP_USERS = "SPUsrs_{0}";
    private const string SP_ACTIONS = "usrSPAct_{0}";
    private const string SP_ACTIONS_EXP = "usrSPActEx_{0}";
    private const string SP_TRAININGS = "usrSPTrai_{0}";
    private const string SP_TRAININGS_EXP = "usrSPTraiEx_{0}";
    public GraphService(
        ILogger<GraphService> logger,
        IMemoryCache memoryCache,
        IConfiguration configuration,
        DataContext dataContext)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _configuration = configuration;
        _database = dataContext;
    }
    public void InitializeGraph(Settings? settings = null)
    {
        if (settings == null)
        {
            settings = Settings.LoadSettings(_configuration);
        }
        _logger.LogInformation($"start ClientSecret: {settings?.ClientSecret?[..3] ?? "Is null"}");
        GraphHelper.InitializeGraphForAppOnlyAuth(settings);
    }

    public async Task<string> GetAccessTokenAsync()
    {
        try
        {
            var appOnlyToken = await GraphHelper.GetAppOnlyTokenAsync();
            Console.WriteLine($"App-only token: {appOnlyToken}");
            return appOnlyToken;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting app-only access token: {ex.Message}");
            return null;
        }
    }

    public async Task<UserCollectionResponse?> ListUsersAsync()
    {
        try
        {
            var userPage = await GraphHelper.GetUsersAsync();

            if (userPage?.Value == null)
            {
                Console.WriteLine("No results returned.");
                return null;
            }

            // If NextPageRequest is not null, there are more users
            // available on the server
            // Access the next page like:
            // var nextPageRequest = new UsersRequestBuilder(userPage.OdataNextLink, _appClient.RequestAdapter);
            // var nextPage = await nextPageRequest.GetAsync();
            var moreAvailable = !string.IsNullOrEmpty(userPage.OdataNextLink);

            Console.WriteLine($"\nMore users available? {moreAvailable}");
            return userPage;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting users: {ex.Message}");
            return null;
        }
    }

    public async Task<DirectoryObjectCollectionResponse?> GetGroupForUser(string userId)
    {
        try
        {
            var result = await GraphHelper.TaskGetGroupForUser(userId);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user by ID: {ex.Message}");
            return null;
        }
    }

    public async Task<UserCollectionResponse> NextUsersPage(UserCollectionResponse users)
    {
        return await GraphHelper.NextUsersPage(users);
    }

    public async Task GetLists()
    {
        await GraphHelper.GetLists();
    }

    public async Task<bool> SyncSharePointActions(Guid customerId, CancellationToken clt)
    {
        var keyActions = string.Format(SP_ACTIONS, customerId);
        var update = await ShouldUpdateCacheSharePointActions(customerId, keyActions);
        if (!update)
            return false;
        _memoryCache.TryGetValue<List<SharePointAction>>(keyActions, out var sharePointActions);
        sharePointActions ??= await GetSharePointActions(customerId, keyActions, clt);
        if (sharePointActions == null || clt.IsCancellationRequested)
            return false;
        var dbActions = await _database.ReportActions
            .Where(x=>x.CustomerId == customerId)
            .Include(x => x.Users).ToListAsync();

        foreach (var action in sharePointActions)
        {
            if (clt.IsCancellationRequested) 
                return false;
            var dbAction = dbActions.FirstOrDefault(x=>x.Id == action.Id);
            if (dbAction is null)
            {
                dbAction = action.ToDbDefaultSchedule(customerId);
                await _database.ReportActions.AddAsync(dbAction, clt);
            }
            else if (dbAction.LastUpdated != action.LastUpdated)
            {
                dbAction.UpdateDbDefaultSchedule(action, customerId);
            }
        }
        var changeCount = await _database.SaveChangesAsync(clt);
        _logger.LogInformation("SharePoint actions synced (count {changeCount})",changeCount);
        return changeCount > 0;
    }
    public async Task<bool> SyncSharePointTrainings(Guid customerId, CancellationToken clt)
    {
        var keyTrainings = string.Format(SP_TRAININGS, customerId);
        await ShouldUpdateCacheSharePointTrainings(customerId, keyTrainings);
        _memoryCache.TryGetValue<List<SharePointTraining>>(keyTrainings, out var sharePointTrainings);
        sharePointTrainings ??= await GetSharePointTrainings(customerId, keyTrainings, clt);
        return true;
    }

    public async Task<MultipleSharePointActionsResponse> GetListActionsUser(List<Guid> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var keyActions = string.Format(SP_ACTIONS, customerId);
        await ShouldUpdateCacheSharePointActions(customerId, keyActions);
        _memoryCache.TryGetValue<List<SharePointAction>>(keyActions, out var sharePointActions);
        sharePointActions ??= await GetSharePointActions(customerId, keyActions, clt);
        var listWhere = sharePointActions?.Where(x => x.Users.Count(y => users.Contains(y.DrogeCodeId)) == users.Count());
        var sharePointActionsUser = new MultipleSharePointActionsResponse
        {
            SharePointActions = listWhere?.Skip(skip).Take(count).ToList(),
            TotalCount = listWhere?.Count() ?? -1
        };
        sw.Stop();
        sharePointActionsUser.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return sharePointActionsUser;
    }

    private async Task<bool> ShouldUpdateCacheSharePointActions(Guid customerId, string keyActions)
    {
        var keyExp = string.Format(SP_ACTIONS_EXP, customerId);
        var cacheOptions = new MemoryCacheEntryOptions();
        _memoryCache.TryGetValue<UpdatedCheck>(keyExp, out var lastupdated);
        if (lastupdated is null)
        {
            _logger.LogInformation("Lastupdated is null for action list");
            lastupdated = new UpdatedCheck();
        }
        if (lastupdated.NextCheck.CompareTo(DateTime.UtcNow) < 0)
        {
            var newLastUpdated = await GraphHelper.ListActionLastUpdate(customerId);
            if (!newLastUpdated.Equals(DateTime.MinValue) && !newLastUpdated.Equals(lastupdated.LastUpdated))
            {
                _memoryCache.Remove(keyActions);
                lastupdated.LastUpdated = newLastUpdated;
                _logger.LogInformation("There are changes in the action list");
            }
            lastupdated.NextCheck = DateTime.UtcNow.AddMinutes(5);
            cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(120));
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(240));
            _ = _memoryCache.Set(keyExp, lastupdated, cacheOptions);
            return true;
        }
        else
            return false;
    }

    private async Task<List<SharePointAction>?> GetSharePointActions(Guid customerId, string keyActions, CancellationToken clt)
    {
        var cacheOptions = new MemoryCacheEntryOptions();
        var spUsers = await GetAllSharePointUsers(customerId, clt);
        var sharePointActions = await GraphHelper.GetListActions(customerId, spUsers);
        cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(30));
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(120));
        _ = _memoryCache.Set(keyActions, sharePointActions, cacheOptions);
        return sharePointActions;
    }

    public async Task<MultipleSharePointTrainingsResponse> GetListTrainingUser(List<Guid> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var keyTrainings = string.Format(SP_TRAININGS, customerId);
        await ShouldUpdateCacheSharePointTrainings(customerId, keyTrainings);
        _memoryCache.TryGetValue<List<SharePointTraining>>(keyTrainings, out var sharePointTrainings);
        sharePointTrainings ??= await GetSharePointTrainings(customerId, keyTrainings, clt);
        var listWhere = sharePointTrainings?.Where(x => x.Users.Count(y => users.Contains(y.DrogeCodeId)) == users.Count());
        var sharePointTrainingsUser = new MultipleSharePointTrainingsResponse
        {
            SharePointTrainings = listWhere?.Skip(skip).Take(count).ToList(),
            TotalCount = listWhere?.Count() ?? -1
        };
        sw.Stop();
        sharePointTrainingsUser.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return sharePointTrainingsUser;
    }

    private async Task<bool> ShouldUpdateCacheSharePointTrainings(Guid customerId, string keyTrainings)
    {
        var keyExp = string.Format(SP_TRAININGS_EXP, customerId);
        var cacheOptions = new MemoryCacheEntryOptions();
        _memoryCache.TryGetValue<UpdatedCheck>(keyExp, out var lastupdated);
        if (lastupdated is null)
        {
            _logger.LogInformation("Lastupdated is null for training list");
            lastupdated = new UpdatedCheck();
        }
        if (lastupdated.NextCheck.CompareTo(DateTime.UtcNow) < 0)
        {
            var newLastUpdated = await GraphHelper.ListTrainingLastUpdate(customerId);
            if (!newLastUpdated.Equals(DateTime.MinValue) && !newLastUpdated.Equals(lastupdated.LastUpdated))
            {
                _memoryCache.Remove(keyTrainings);
                lastupdated.LastUpdated = newLastUpdated;
                _logger.LogInformation("There are changes in the training list");
            }
            lastupdated.NextCheck = DateTime.UtcNow.AddMinutes(5);
            cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(120));
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(240));
            _ = _memoryCache.Set(keyExp, lastupdated, cacheOptions);
            return true;
        }
        else
            return false;
    }

    private async Task<List<SharePointTraining>?> GetSharePointTrainings(Guid customerId, string keyTrainings, CancellationToken clt)
    {
        var cacheOptions = new MemoryCacheEntryOptions();
        var spUsers = await GetAllSharePointUsers(customerId, clt);
        var sharePointTrainings = await GraphHelper.GetListTraining(customerId, spUsers);
        cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(30));
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(120));
        _ = _memoryCache.Set(keyTrainings, sharePointTrainings, cacheOptions);
        return sharePointTrainings;
    }

    private async Task<List<SharePointUser>> GetAllSharePointUsers(Guid customerId, CancellationToken clt)
    {

        _memoryCache.TryGetValue<List<SharePointUser>>(string.Format(SP_USERS, customerId), out var sharePointUsers);
        if (sharePointUsers == null)
        {
            sharePointUsers = await GraphHelper.FindSharePointUsers();
            var dbUsers = await _database.Users.Where(x=> x.CustomerId == customerId).Select(x => new { x.Id, x.SharePointID, x.Name }).ToListAsync();
            foreach (var sharePointUser in sharePointUsers)
            {
                var dbUser = dbUsers.FirstOrDefault(x => x.SharePointID == sharePointUser.SharePointID);
                if (dbUser is null)
                    dbUser = dbUsers.FirstOrDefault(x => x.Name == sharePointUser.Name);
                if (dbUser is not null)
                    sharePointUser.DrogeCodeId = dbUser.Id;

            }
            _ = _memoryCache.Set(string.Format(SP_USERS, customerId), sharePointUsers, DateTimeOffset.UtcNow.AddMinutes(30));
        }
        return sharePointUsers;
    }

    public async Task<Event> AddToCalendar(Guid userId, string description, DateTime dateStart, DateTime dateEnd, bool isAllDay)
    {
        var result = await GraphHelper.AddToCalendar(userId, description, dateStart, dateEnd, isAllDay);
        return result;
    }

    public async Task PatchCalender(Guid userId, string eventId, string description, DateTime dateStart, DateTime dateEnd, bool isAllDay)
    {
        await GraphHelper.PatchCalender(userId, eventId, description, dateStart, dateEnd, isAllDay);
    }

    public async Task DeleteCalendarEvent(Guid? userId, string calendarEventId, CancellationToken clt)
    {
        await GraphHelper.DeleteCalendarEvent(userId, calendarEventId, clt);
    }

    private class UpdatedCheck
    {
        public DateTime LastUpdated { get; set; }
        public DateTime NextCheck { get; set; }
    }
}
