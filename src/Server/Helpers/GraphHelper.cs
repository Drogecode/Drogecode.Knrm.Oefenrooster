using Azure.Core;
using Azure.Identity;
using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Sites.Item.Lists.Item.Items;
using Microsoft.Graph.Users;
using System;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Helpers;

public static class GraphHelper
{
    // Settings object
    private static Settings? _settings;
    // App-ony auth token credential
    private static ClientSecretCredential? _clientSecretCredential;
    // Client configured with app-only authentication
    private static GraphServiceClient? _appClient;

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

        var result = _appClient.Users.GetAsync((config) =>
        {
            // Only request specific properties
            config.QueryParameters.Select = new[] { "displayName", "id", "mail" };
            // Get at most 25 results
            config.QueryParameters.Top = 25;
            // Sort by display name
            config.QueryParameters.Orderby = new[] { "displayName" };
        });
        return result;
    }
    public static async Task<UserCollectionResponse> NextUsersPage(UserCollectionResponse userPage)
    {
        var nextPageRequest = new UsersRequestBuilder(userPage.OdataNextLink, _appClient.RequestAdapter);
        var nextPage = await nextPageRequest.GetAsync();
        return nextPage;
    }
    public static async Task<ListItemCollectionResponse> NextListPage(ListItemCollectionResponse listPage)
    {
        var nextPageRequest = new ItemsRequestBuilder(listPage.OdataNextLink, _appClient.RequestAdapter);
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

    internal static async Task<List<SharePointUser>> FindSharePointUsers()
    {
        var allUsers = await _appClient.Sites[STARTPAGINA].Lists[ID_USERS_KNRM].Items.GetAsync((config) =>
        {
            config.QueryParameters.Expand = new string[] { "fields" };
            config.QueryParameters.Top = 25;
        });
        var spUsers = new List<SharePointUser>();
        while (allUsers?.Value != null)
        {
            foreach (var det in allUsers.Value)
            {
                if (det?.Fields?.AdditionalData == null || det.Id == null || det?.Fields?.AdditionalData?.ContainsKey("Title") != true) continue;
                spUsers.Add(new SharePointUser
                {
                    SharePointID = det.Id,
                    Name = det.Fields.AdditionalData["Title"].ToString()
                });
            }
            if (allUsers.OdataNextLink != null)
                allUsers = await NextListPage(allUsers);
            else break;
        }
        return spUsers;
    }

    internal async static Task<List<SharePointTraining>> GetListTraining(Guid customerId, List<SharePointUser> users)
    {
        if (_appClient == null || customerId != DefaultSettingsHelper.KnrmHuizenId) return null;

        var overigeRaporten = await _appClient.Sites[STARTPAGINA].Lists[ID_OTHER_REPORTS_KNRM_HUIZEN].GetAsync();

        var overigeItems = await _appClient.Sites[STARTPAGINA].Lists[ID_OTHER_REPORTS_KNRM_HUIZEN].Items.GetAsync((config) =>
        {
            config.QueryParameters.Expand = new string[] { "fields" };
            config.QueryParameters.Top = 25;
        });
        var trainings = new List<SharePointTraining>();
        while (overigeItems?.Value != null)
        {
            foreach (var det in overigeItems.Value)
            {
                if (det?.Fields?.AdditionalData == null) continue;
                var training = new SharePointTraining();
                GetUser(users, det, "SchipperLookupId", SharePointRole.Schipper, training);
                GetUser(users, det, "Opstapper_x0020_1LookupId", SharePointRole.Opstapper, training);
                GetUser(users, det, "Opstapper_x0020_2LookupId", SharePointRole.Opstapper, training);
                GetUser(users, det, "Opstapper_x0020_3LookupId", SharePointRole.Opstapper, training);
                GetUser(users, det, "Opstapper_x0020_4LookupId", SharePointRole.Opstapper, training);
                GetUser(users, det, "Opstapper_x0020_5LookupId", SharePointRole.Opstapper, training);
                var start = det.Fields.AdditionalData.ContainsKey("Aanvang_x0020_O_x0026_O_x0020__x") ? (DateTime)det.Fields.AdditionalData["Aanvang_x0020_O_x0026_O_x0020__x"] : DateTime.MinValue;
                start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
                training.Start = start;
                training.Title = det.Fields.AdditionalData.ContainsKey("LinkTitle") ? det.Fields.AdditionalData["LinkTitle"]!.ToString() : "";
                training.Description = det.Fields.AdditionalData.ContainsKey("Bijzonderheden_x0020_Oproep") ? det.Fields.AdditionalData["Bijzonderheden_x0020_Oproep"]!.ToString() : "";
                trainings.Add(training);
            }
            if (overigeItems.OdataNextLink != null)
                overigeItems = await NextListPage(overigeItems);
            else break;
        }
        return trainings.OrderByDescending(x => x.Start).ToList();
    }

    internal async static Task<List<SharePointAction>> GetListActions(Guid customerId, List<SharePointUser> users)
    {
        if (_appClient == null || customerId != DefaultSettingsHelper.KnrmHuizenId) return new List<SharePointAction>();

        var overigeRaporten = await _appClient.Sites[STARTPAGINA].Lists[ID_ACTION_REPORTS_KNRM_HUIZEN].GetAsync();

        var overigeItems = await _appClient.Sites[STARTPAGINA].Lists[ID_ACTION_REPORTS_KNRM_HUIZEN].Items.GetAsync((config) =>
        {
            config.QueryParameters.Expand = new string[] { "fields" };
            config.QueryParameters.Top = 25;
        });
        var actions = new List<SharePointAction>();
        while (overigeItems?.Value != null)
        {
            foreach (var det in overigeItems.Value)
            {
                if (det?.Fields?.AdditionalData == null) continue;
                var action = new SharePointAction();
                GetUser(users, det, "SchipperLookupId", SharePointRole.Schipper, action);
                GetUser(users, det, "Opstapper_x0020_1LookupId", SharePointRole.Opstapper, action);
                GetUser(users, det, "Opstapper_x0020_2LookupId", SharePointRole.Opstapper, action);
                GetUser(users, det, "Opstapper_x0020_3LookupId", SharePointRole.Opstapper, action);
                GetUser(users, det, "Opstapper_x0020_4LookupId", SharePointRole.Opstapper, action);
                GetUser(users, det, "Opstapper_x0020_5LookupId", SharePointRole.Opstapper, action);
                double number = -1;
                if (det.Fields.AdditionalData.ContainsKey("Actie_x0020_nummer"))
                    _ = double.TryParse(det.Fields.AdditionalData["Actie_x0020_nummer"].ToString(), out number);
                var start = det.Fields.AdditionalData.ContainsKey("Oproep_x0020__x0028_uren_x0029_") ? (DateTime)det.Fields.AdditionalData["Oproep_x0020__x0028_uren_x0029_"] : DateTime.MinValue;
                start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
                action.Number = number;
                action.Start = start;
                action.Title = det.Fields.AdditionalData.ContainsKey("LinkTitle") ? det.Fields.AdditionalData["LinkTitle"]!.ToString() : "";
                action.ShortDescription = det.Fields.AdditionalData.ContainsKey("Korte_x0020_Omschrijving") ? det.Fields.AdditionalData["Korte_x0020_Omschrijving"]!.ToString() : "";
                action.Description = det.Fields.AdditionalData.ContainsKey("Bijzonderheden_x0020_Oproep") ? det.Fields.AdditionalData["Bijzonderheden_x0020_Oproep"]!.ToString() : "";
                action.Prio = det.Fields.AdditionalData.ContainsKey("Priotiteit_x0020_Alarmering") ? det.Fields.AdditionalData["Priotiteit_x0020_Alarmering"]!.ToString() : "";
                actions.Add(action);
            }
            if (overigeItems.OdataNextLink != null)
                overigeItems = await NextListPage(overigeItems);
            else break;
        }
        return actions.OrderByDescending(x => x.Start).ToList();
    }

    internal static async Task<DateTime> ListTrainingLastUpdate(Guid customerId)
    {
        if (_appClient == null || customerId != DefaultSettingsHelper.KnrmHuizenId) return DateTime.MinValue;
        return await ListLastModified(STARTPAGINA, ID_OTHER_REPORTS_KNRM_HUIZEN);
    }

    internal static async Task<DateTime> ListActionLastUpdate(Guid customerId)
    {
        if (_appClient == null || customerId != DefaultSettingsHelper.KnrmHuizenId) return DateTime.MinValue;
        return await ListLastModified(STARTPAGINA, ID_ACTION_REPORTS_KNRM_HUIZEN);
    }

    private static async Task<DateTime> ListLastModified(string site, string idlist)
    {
        if (_appClient == null) return DateTime.MinValue;
        var list = await _appClient.Sites[site].Lists[idlist].GetAsync();
        if (list?.LastModifiedDateTime is null) return DateTime.MinValue;
        return list.LastModifiedDateTime.Value.DateTime;
    }

    private static void GetUser(List<SharePointUser> users, ListItem det, string key, SharePointRole role, SharePointListBase listBase)
    {
        var sharePointID = det.Fields.AdditionalData.ContainsKey(key) ? det.Fields.AdditionalData[key]?.ToString() : "";
        if (sharePointID == null) return;
        var user = users.FirstOrDefault(x => x.SharePointID == sharePointID);
        if (user == null) return;
        user.Role = role;
        listBase.Users.Add(user);
    }

    public static async Task<Event> AddToCalendar()
    {
        var body = new Event()
        {
            Subject = "Test from graph",
            Body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = "Does this time work for you?",
            },
            Start = new DateTimeTimeZone
            {
                DateTime = "2023-07-15T12:00:00",
                TimeZone = "UTC",
            },
            End = new DateTimeTimeZone
            {
                DateTime = "2023-07-15T14:00:00",
                TimeZone = "UTC",
            },
        };
        var result = await _appClient.Users[DefaultSettingsHelper.IdTaco.ToString()].Events.PostAsync(body, (requestConfiguration) =>
        {
            requestConfiguration.Headers.Add("Prefer", "outlook.timezone=\"Pacific Standard Time\"");
        });

        var fromGet = await _appClient.Users[DefaultSettingsHelper.IdTaco.ToString()].Events[result.Id].GetAsync();
        return result;
    }
}
