using Azure.Core;
using Azure.Identity;
using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users;

namespace Drogecode.Knrm.Oefenrooster.Server.Helpers;

public static class GraphHelper
{
    // Settings object
    private static Settings? _settings;
    // App-ony auth token credential
    private static ClientSecretCredential? _clientSecretCredential;
    // Client configured with app-only authentication
    private static GraphServiceClient? _appClient;

    private static List<SharePointUser>? _userList;

    ///consts for KNRM huizen, if used by other organizations should be moved to db.
    private const string KNRM_HUIZEN = "dorus1824.sharepoint.com,282e3a78-e28a-4db9-a30f-0244d23b05c9,411e2e34-56c5-4219-8624-30bd89032f48";
    private const string STARTPAGINA = "dorus1824.sharepoint.com,834aa582-3cac-4e12-96fd-3aecebd36e4b,e7847792-243f-4564-a8f8-1ca2793e5f98";
    private const string HRB = "dorus1824.sharepoint.com,02bec1eb-b5d1-4ace-b064-d21cd2986efc,eb674d8f-15ee-4b6f-9467-8015ed5231e7";
    private const string ID_USERS_KNRM = "5DA01E6F-41A5-4230-A0FB-7E7E0582037E";
    private const string ID_ACTION_REPORTS_KNRM_HUIZEN = "2a6cf2ae-964a-4a63-9229-dcb820924bd5";
    private const string ID_OTHER_REPORTS_KNRM_HUIZEN = "7fff5890-2100-406a-89d9-f07978bda321";

    public static void InitializeGraphForAppOnlyAuth(Settings? settings)
    {
        _settings = settings;

        // Ensure settings isn't null
        _ = settings ??
            throw new System.NullReferenceException("Settings cannot be null");

        _settings = settings;

        if (_clientSecretCredential == null)
        {
            if (string.IsNullOrEmpty(_settings.TenantId))
                throw new System.NullReferenceException("TenantId cannot be null");
            if (string.IsNullOrEmpty(_settings.ClientId))
                throw new System.NullReferenceException("ClientId cannot be null");
            if (string.IsNullOrEmpty(_settings.ClientSecret))
                throw new System.NullReferenceException("ClientSecret cannot be null");
            _clientSecretCredential = new ClientSecretCredential(
                _settings.TenantId, _settings.ClientId, _settings.ClientSecret);
        }

        if (_appClient == null)
        {
            _appClient = new GraphServiceClient(_clientSecretCredential,
                // Use the default scope, which will request the scopes
                // configured on the app registration
                new[] { "https://graph.microsoft.com/.default" });
        }
    }

    public static async Task<string> GetAppOnlyTokenAsync()
    {
        // Ensure credential isn't null
        _ = _clientSecretCredential ??
            throw new System.NullReferenceException("Graph has not been initialized for app-only auth");

        // Request token with given scopes
        var context = new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" });
        var response = await _clientSecretCredential.GetTokenAsync(context);
        return response.Token;
    }
    public static Task<UserCollectionResponse?> GetUsersAsync()
    {
        // Ensure client isn't null
        _ = _appClient ??
            throw new System.NullReferenceException("Graph has not been initialized for app-only auth");

        return _appClient.Users.GetAsync((config) =>
        {
            // Only request specific properties
            config.QueryParameters.Select = new[] { "displayName", "id", "mail" };
            // Get at most 25 results
            config.QueryParameters.Top = 25;
            // Sort by display name
            config.QueryParameters.Orderby = new[] { "displayName" };
        });
    }
    public static async Task<UserCollectionResponse> NextUsersPage(UserCollectionResponse userPage)
    {
        var nextPageRequest = new UsersRequestBuilder(userPage.OdataNextLink, _appClient.RequestAdapter);
        var nextPage = await nextPageRequest.GetAsync();
        return nextPage;
    }

    public static async Task<DirectoryObjectCollectionResponse?> TaskGetGroupForUser(string userId)
    {
        // Ensure client isn't null
        _ = _appClient ??
            throw new System.NullReferenceException("Graph has not been initialized for app-only auth");

        var result = await _appClient.Users[userId].TransitiveMemberOf.GetAsync();
        return result;
    }

    /// <summary>
    /// Work in progress
    /// </summary>
    /// <returns>Nothing yet</returns>
    public static async Task GetLists()
    {
        try
        {
            if (_appClient == null) return;
            var actieRaporten = await _appClient.Sites[STARTPAGINA].Lists[ID_ACTION_REPORTS_KNRM_HUIZEN].GetAsync();
            var overigeRaporten = await _appClient.Sites[STARTPAGINA].Lists[ID_OTHER_REPORTS_KNRM_HUIZEN].GetAsync();
            var actieColumns = await _appClient.Sites[STARTPAGINA].Lists[ID_ACTION_REPORTS_KNRM_HUIZEN].Columns.GetAsync();
            var actieItems = await _appClient.Sites[STARTPAGINA].Lists[ID_ACTION_REPORTS_KNRM_HUIZEN].Items.GetAsync();
            var overigeItems = await _appClient.Sites[STARTPAGINA].Lists[ID_OTHER_REPORTS_KNRM_HUIZEN].Items.GetAsync();

            foreach (var actie in actieItems.Value)
            {
                Console.WriteLine(actie.Name);
                var det = await _appClient.Sites[STARTPAGINA].Lists[ID_ACTION_REPORTS_KNRM_HUIZEN].Items[actie.Id].GetAsync();
                Console.WriteLine(det.Fields.AdditionalData.Count);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    internal async static Task<object> GetListTraining(string userName, Guid userId, Guid customerId)
    {
        if (_appClient == null || customerId != DefaultSettingsHelper.KnrmHuizenId) return null;
        var users = await FindSharePointUsers(userName);

        var spUser = users.FirstOrDefault(x => string.Compare(x.Name, userName) == 0);

        var overigeRaporten = await _appClient.Sites[STARTPAGINA].Lists[ID_OTHER_REPORTS_KNRM_HUIZEN].GetAsync();

        var overigeItems = await _appClient.Sites[STARTPAGINA].Lists[ID_OTHER_REPORTS_KNRM_HUIZEN].Items.GetAsync((requestConfiguration) =>
        {
            requestConfiguration.QueryParameters.Expand = new string[] { "fields" };
        });
        var listItems = await _appClient.Sites[$"{STARTPAGINA}"].Lists[$"{ID_OTHER_REPORTS_KNRM_HUIZEN}"].Items.GetAsync();
        foreach (var det in overigeItems.Value.OrderByDescending(x=>x.CreatedDateTime))
        {
            if (det?.Fields?.AdditionalData == null) continue;
            var schipperId = det.Fields.AdditionalData["SchipperLookupId"]?.ToString();
            if (string.Compare(schipperId, spUser.Id) == 0)
            {
                continue;
            }
            var op1 = det.Fields.AdditionalData["Opstapper_x0020_1LookupId"]?.ToString();
            if (string.Compare(op1, spUser.Id) == 0)
            {
                continue;
            }
            var op2 = det.Fields.AdditionalData["Opstapper_x0020_2LookupId"]?.ToString();
            if (string.Compare(op2, spUser.Id) == 0)
            {
                continue;
            }
            var op3 = det.Fields.AdditionalData["Opstapper_x0020_3LookupId"]?.ToString();
            if (string.Compare(op3, spUser.Id) == 0)
            {
                continue;
            }
            var op4 = det.Fields.AdditionalData["Opstapper_x0020_4LookupId"]?.ToString();
            if (string.Compare(op4, spUser.Id) == 0)
            {
                continue;
            }
            var op5 = det.Fields.AdditionalData["Opstapper_x0020_5LookupId"]?.ToString();
            if (string.Compare(op5, spUser.Id) == 0)
            {
                continue;
            }
        }
        return overigeItems;
    }

    private static async Task<List<SharePointUser>> FindSharePointUsers(string userName)
    {
        if (_userList != null && !_userList.Any(x => string.Compare(x.Name, userName) == 0))
            return _userList;
        var allUsers = await _appClient.Sites[STARTPAGINA].Lists[ID_USERS_KNRM].Items.GetAsync((requestConfiguration) =>
        {
            requestConfiguration.QueryParameters.Expand = new string[] { "fields" };
        });
        var spUsers = new List<SharePointUser>();
        foreach (var det in allUsers.Value)
        {
            spUsers.Add(new SharePointUser
            {
                Id = det.Id,
                Name = det.Fields.AdditionalData["Title"].ToString()
            });
        }
        _userList = spUsers;
        return _userList;
    }

    private class SharePointUser
    {
        public string Id;
        public string Name;
    }
}
