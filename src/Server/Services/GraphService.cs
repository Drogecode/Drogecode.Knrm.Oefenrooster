using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class GraphService : IGraphService
{
    private readonly ILogger<GraphService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;

    private const string USER_SP_TRAININGS = "usrSPTrai_{0}";
    private const string USER_SP_ACTIONS = "usrSPAct_{0}";
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

    public async Task<List<SharePointTraining>> GetListTrainingUser(string userName, Guid userId, int count, Guid customerId, CancellationToken clt)
    {
        _memoryCache.TryGetValue<List<SharePointTraining>>(string.Format(USER_SP_TRAININGS, userId), out var sharePointTrainings);
        if (sharePointTrainings == null)
        {
             sharePointTrainings = await GraphHelper.GetListTraining(userName, userId, customerId);
            _ = _memoryCache.Set(string.Format(USER_SP_TRAININGS, userId), sharePointTrainings, DateTimeOffset.UtcNow.AddMinutes(5));
        }
        return sharePointTrainings.Take(count).ToList();
    }

    public async Task<List<SharePointAction>> GetListActionsUser(string userName, Guid userId, int count, Guid customerId, CancellationToken clt)
    {
        _memoryCache.TryGetValue<List<SharePointAction>>(string.Format(USER_SP_ACTIONS, userId), out var sharePointTrainings);
        if (sharePointTrainings == null)
        {
             sharePointTrainings = await GraphHelper.GetListActions(userName, userId, customerId);
            _ = _memoryCache.Set(string.Format(USER_SP_ACTIONS, userId), sharePointTrainings, DateTimeOffset.UtcNow.AddMinutes(5));
        }
        return sharePointTrainings.Take(count).ToList();
    }
}
