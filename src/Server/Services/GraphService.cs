using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph.Models;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class GraphService : IGraphService
{
    private readonly ILogger<GraphService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;

    private const string SP_USERS = "SPUsrs_{0}";
    private const string SP_ACTIONS = "usrSPAct_{0}";
    private const string SP_ACTIONS_EXP = "usrSPActEx_{0}";
    private const string SP_TRAININGS = "usrSPTrai_{0}";
    private const string SP_TRAININGS_EXP = "usrSPTraiEx_{0}";
    public GraphService(
        ILogger<GraphService> logger,
        IMemoryCache memoryCache,
        IConfiguration configuration)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _configuration = configuration;
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

    public async Task<MultipleSharePointActionsResponse> GetListActionsUser(List<string> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var keyExp = string.Format(SP_ACTIONS_EXP, customerId);
        var keyActions = string.Format(SP_ACTIONS, customerId);
        var cacheOptions = new MemoryCacheEntryOptions();
        _memoryCache.TryGetValue<UpdatedCheck>(keyExp, out var lastupdated);
        if (lastupdated is null)
            lastupdated = new UpdatedCheck();
        if (lastupdated.NextCheck.CompareTo(DateTime.UtcNow) < 0)
        {
            var newLastUpdated = await GraphHelper.ListActionLastUpdate(customerId);
            if (!newLastUpdated.Equals(DateTime.MinValue) && !newLastUpdated.Equals(lastupdated))
            {
                _memoryCache.Remove(keyActions);
                lastupdated.LastUpdated = newLastUpdated;
                _logger.LogInformation("There are changes in the action list");
            }
            lastupdated.NextCheck = DateTime.UtcNow.AddMinutes(5);
            cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(120));
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(240));
            _ = _memoryCache.Set(keyExp, lastupdated, cacheOptions);
        }
        _memoryCache.TryGetValue<List<SharePointAction>>(keyActions, out var sharePointActions);
        if (sharePointActions == null)
        {
            var spUsers = await GetAllSharePointUsers(customerId, clt);
            sharePointActions = await GraphHelper.GetListActions(customerId, spUsers);
            cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(30));
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(120));
            _ = _memoryCache.Set(keyActions, sharePointActions, cacheOptions);
        }
        var listWhere = sharePointActions?.Where(x => x.Users.Any(y => users.All(x=> string.Compare(y.Name, x) == 0)) );
        var sharePointActionsUser = new MultipleSharePointActionsResponse
        {
            SharePointActions = listWhere?.Skip(skip).Take(count).ToList(),
            TotalCount = listWhere?.Count() ?? -1
        };
        sw.Stop();
        sharePointActionsUser.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return sharePointActionsUser;
    }

    public async Task<MultipleSharePointTrainingsResponse> GetListTrainingUser(List<string> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var keyExp = string.Format(SP_TRAININGS_EXP, customerId);
        var keyTrainings = string.Format(SP_TRAININGS, customerId);
        var cacheOptions = new MemoryCacheEntryOptions();
        _memoryCache.TryGetValue<UpdatedCheck>(keyExp, out var lastupdated);
        if (lastupdated is null)
            lastupdated = new UpdatedCheck();
        if (lastupdated.NextCheck.CompareTo(DateTime.UtcNow) < 0)
        {
            var newLastUpdated = await GraphHelper.ListTrainingLastUpdate(customerId);
            if (!newLastUpdated.Equals(DateTime.MinValue) && !newLastUpdated.Equals(lastupdated))
            {
                _memoryCache.Remove(keyTrainings);
                lastupdated.LastUpdated = newLastUpdated;
                _logger.LogInformation("There are changes in the training list");
            }
            lastupdated.NextCheck = DateTime.UtcNow.AddMinutes(5);
            cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(120));
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(240));
            _ = _memoryCache.Set(keyExp, lastupdated, cacheOptions);
        }
        _memoryCache.TryGetValue<List<SharePointTraining>>(keyTrainings, out var sharePointTrainings);
        if (sharePointTrainings is null)
        {
            var spUsers = await GetAllSharePointUsers(customerId, clt);
            sharePointTrainings = await GraphHelper.GetListTraining(customerId, spUsers);
            cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(30));
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(120));
            _ = _memoryCache.Set(keyTrainings, sharePointTrainings, cacheOptions);
        }
        var listWhere = sharePointTrainings?.Where(x => x.Users.Any(y => users.All(x => string.Compare(y.Name, x) == 0)));
        var sharePointTrainingsUser = new MultipleSharePointTrainingsResponse
        {
            SharePointTrainings = listWhere?.Skip(skip).Take(count).ToList(),
            TotalCount = listWhere?.Count() ?? -1
        };
        sw.Stop();
        sharePointTrainingsUser.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return sharePointTrainingsUser;
    }

    private async Task<List<SharePointUser>> GetAllSharePointUsers(Guid customerId, CancellationToken clt)
    {

        _memoryCache.TryGetValue<List<SharePointUser>>(string.Format(SP_USERS, customerId), out var sharePointUsers);
        if (sharePointUsers == null)
        {
            sharePointUsers = await GraphHelper.FindSharePointUsers();
            _ = _memoryCache.Set(string.Format(SP_USERS, customerId), sharePointUsers, DateTimeOffset.UtcNow.AddMinutes(30));
        }
        return sharePointUsers;
    }

    public async Task<Event> AddToCalendar(Guid userId, string description, DateTime dateStart, DateTime dateEnd)
    {
        var result = await GraphHelper.AddToCalendar(userId, description, dateStart, dateEnd);
        return result;
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
