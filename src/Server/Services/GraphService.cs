using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class GraphService : IGraphService
{
    private readonly ILogger<GraphService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;

    private const string SP_USERS = "SPUsrs_{0}";
    private const string SP_ACTIONS = "usrSPAct_{0}";
    private const string SP_TRAININGS = "usrSPTrai_{0}";
    private const string USER_SP_ACTIONS = "usrSPAct_{0}_{1}_{2}";
    private const string USER_SP_TRAININGS = "usrSPTrai_{0}_{1}_{2}";
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

    public async Task<List<SharePointAction>> GetListActionsUser(string userName, Guid userId, int count, Guid customerId, CancellationToken clt)
    {
        _memoryCache.TryGetValue<List<SharePointAction>>(string.Format(USER_SP_ACTIONS, customerId, userId, count), out var sharePointActionsUser);
        if (sharePointActionsUser == null)
        {
            var cacheOptions = new MemoryCacheEntryOptions();
            _memoryCache.TryGetValue<List<SharePointAction>>(string.Format(SP_ACTIONS, customerId), out var sharePointActions);
            if (sharePointActions == null)
            {
                var users = await GetAllSharePointUsers(customerId, clt);
                sharePointActions = await GraphHelper.GetListActions(customerId, users);
                cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(120));
                _ = _memoryCache.Set(string.Format(SP_ACTIONS, customerId), sharePointActions, cacheOptions);
            }
            cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
            sharePointActionsUser = sharePointActions.Where(x => x.Users.Any(y => string.Compare(y.Name, userName) == 0)).Take(count).ToList();
            _ = _memoryCache.Set(string.Format(USER_SP_ACTIONS,customerId, userId, count), sharePointActionsUser, cacheOptions);
        }
        return sharePointActionsUser;
    }

    public async Task<List<SharePointTraining>> GetListTrainingUser(string userName, Guid userId, int count, Guid customerId, CancellationToken clt)
    {
        _memoryCache.TryGetValue<List<SharePointTraining>>(string.Format(USER_SP_TRAININGS, customerId, userId, count), out var sharePointTrainingsUser);
        if (sharePointTrainingsUser == null)
        {
            var cacheOptions = new MemoryCacheEntryOptions();
            _memoryCache.TryGetValue<List<SharePointTraining>>(string.Format(SP_TRAININGS, customerId), out var sharePointTrainings);
            if (sharePointTrainings == null)
            {
                var users = await GetAllSharePointUsers(customerId, clt);
                sharePointTrainings = await GraphHelper.GetListTraining(customerId, users);
                cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(120));
                _ = _memoryCache.Set(string.Format(SP_TRAININGS, customerId), sharePointTrainings, cacheOptions);
            }
            sharePointTrainingsUser = sharePointTrainings.Where(x => x.Users.Any(y => string.Compare(y.Name, userName) == 0)).Take(count).ToList();
            cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
            _ = _memoryCache.Set(string.Format(USER_SP_TRAININGS, customerId, userId, count), sharePointTrainingsUser, cacheOptions);
        }
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
}
