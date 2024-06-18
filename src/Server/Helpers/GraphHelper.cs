using Azure.Core;
using Azure.Identity;
using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Sites.Item.Lists.Item.Items;
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
                var training = new SharePointTraining { Users = new List<SharePointUser>() };
                GetUser(users, det, "SchipperLookupId", SharePointRole.Schipper, training, 0);
                GetUser(users, det, "Opstapper_x0020_1LookupId", SharePointRole.Opstapper, training, 1);
                GetUser(users, det, "Opstapper_x0020_2LookupId", SharePointRole.Opstapper, training,2);
                GetUser(users, det, "Opstapper_x0020_3LookupId", SharePointRole.Opstapper, training,3);
                GetUser(users, det, "Opstapper_x0020_4LookupId", SharePointRole.Opstapper, training,4);
                GetUser(users, det, "Opstapper_x0020_5LookupId", SharePointRole.Opstapper, training,5);
                GetUser(users, det, "Opstapper_x0020_6LookupId", SharePointRole.Opstapper, training,6);
                GetUser(users, det, "Opstapper_x0020_7LookupId", SharePointRole.Opstapper, training,7);
                GetUser(users, det, "Opstapper_x0020_8LookupId", SharePointRole.Opstapper, training,8);
                GetUser(users, det, "Opstapper_x0020_9LookupId", SharePointRole.Opstapper, training,9);
                training.Start = AdditionalDataToDateTime(det, "Aanvang_x0020_O_x0026_O_x0020__x");
                ;
                training.Commencement = AdditionalDataToDateTime(det, "Aanvang");
                training.End = AdditionalDataToDateTime(det, "Einde");
                ;
                training.Title = AdditionalDataToString(det, "LinkTitle");
                training.Description = AdditionalDataToString(det, "Bijzonderheden_x0020_Oproep");
                training.Boat = AdditionalDataToString(det, "Bo_x0028_o_x0029_t_x0028_en_x002");
                training.Type = AdditionalDataToString(det, "Soort");
                trainings.Add(training);
            }

            if (overigeItems.OdataNextLink != null)
                overigeItems = await NextListPage(overigeItems);
            else break;
        }

        return trainings.OrderByDescending(x => x.Commencement).ToList();
    }

    internal async static Task<List<SharePointAction>> GetListActions(Guid customerId, List<SharePointUser> users)
    {
        if (_appClient is null || customerId != DefaultSettingsHelper.KnrmHuizenId) return new List<SharePointAction>();

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
                if (det?.Fields?.AdditionalData is null || det.ETag is null) continue;
                var action = new SharePointAction { Users = new List<SharePointUser>() };
                action.Id = new Guid(det.ETag!.Split('\"', ',')[1]);
                if (det.LastModifiedDateTime is not null)
                    action.LastUpdated = DateTime.SpecifyKind(det.LastModifiedDateTime.Value.DateTime, DateTimeKind.Utc);
                GetUser(users, det, "SchipperLookupId", SharePointRole.Schipper, action, 0);
                GetUser(users, det, "Opstapper_x0020_1LookupId", SharePointRole.Opstapper, action, 1);
                GetUser(users, det, "Opstapper_x0020_2LookupId", SharePointRole.Opstapper, action, 2);
                GetUser(users, det, "Opstapper_x0020_3LookupId", SharePointRole.Opstapper, action, 3);
                GetUser(users, det, "Opstapper_x0020_4LookupId", SharePointRole.Opstapper, action, 4);
                GetUser(users, det, "Opstapper_x0020_5LookupId", SharePointRole.Opstapper, action, 5);
                GetUser(users, det, "Opstapper_x0020_6LookupId", SharePointRole.Opstapper, action, 6);
                GetUser(users, det, "Opstapper_x0020_7LookupId", SharePointRole.Opstapper, action, 7);
                GetUser(users, det, "Opstapper_x0020_8LookupId", SharePointRole.Opstapper, action, 8);
                GetUser(users, det, "Opstapper_x0020_9LookupId", SharePointRole.Opstapper, action, 9);
                action.Number = AdditionalDataToDouble(det, "Actie_x0020_nummer");
                action.Date = AdditionalDataToDateTime(det, "Datum");
                ;
                action.Start = AdditionalDataToDateTime(det, "Oproep_x0020__x0028_uren_x0029_");
                ;
                action.Commencement = AdditionalDataToDateTime(det, "Aanvang");
                action.Departure = AdditionalDataToDateTime(det, "Vertrek_x0020__x0028_uren_x0029_");
                action.End = AdditionalDataToDateTime(det, "Einde");
                ;
                action.Title = AdditionalDataToString(det, "LinkTitle");
                action.ShortDescription = AdditionalDataToString(det, "Korte_x0020_Omschrijving");
                action.Description = AdditionalDataToString(det, "Bijzonderheden_x0020_Oproep");
                action.Prio = AdditionalDataToString(det, "Priotiteit_x0020_Alarmering");
                action.Type = AdditionalDataToString(det, "Soort");
                action.Request = AdditionalDataToString(det, "Oproep");
                action.ForTheBenefitOf = AdditionalDataToString(det, "Ten_x0020_behoeve_x0020_van");
                action.Causes = AdditionalDataToString(det, "Oorzaken");
                action.Implications = AdditionalDataToString(det, "Gevolgen");
                action.Area = AdditionalDataToString(det, "Gebied");
                action.Area = AdditionalDataToString(det, "Gebied");
                action.WindDirection = AdditionalDataToString(det, "Windrichting");
                action.WindPower = AdditionalDataToInt(det, "Windkracht_x0020__x0028_Beaufort");
                action.WaterTemperature = AdditionalDataToDouble(det, "Temperatuur_x0020_Water");
                action.GolfHight = AdditionalDataToDouble(det, "Golf_x0020_Hoogte");
                action.Sight = AdditionalDataToInt(det, "Zicht");
                action.WeatherCondition = AdditionalDataToString(det, "Weersgesteldheid");
                action.CallMadeBy = AdditionalDataToString(det, "Oproep_x0020_gedaan_x0020_door");
                action.CountSailors = AdditionalDataToDouble(det, "Aantal_x0020_Opvarenden");
                action.CountSaved = AdditionalDataToDouble(det, "Aantal_x0020_geredden");
                action.CountAnimals = AdditionalDataToDouble(det, "Aantal_x0020_dieren");
                action.Boat = AdditionalDataToString(det, "Bo_x0028_o_x0029_t_x0028_en_x002");
                action.FunctioningMaterial = AdditionalDataToString(det, "Functioneren_x0020_materieel");
                action.ProblemsWithWeed = AdditionalDataToString(det, "Problemen_x0020_met_x0020_fontei");
                action.Completedby = AdditionalDataToString(det, "afgehandeld");
                actions.Add(action);
            }

            if (overigeItems.OdataNextLink != null)
                overigeItems = await NextListPage(overigeItems);
            else break;
        }

        return actions.OrderByDescending(x => x.Start).ToList();
    }

    private static string? AdditionalDataToString(ListItem det, string key)
    {
        return det.Fields!.AdditionalData.ContainsKey(key) ? det.Fields.AdditionalData[key]!.ToString() : "";
    }

    private static DateTime AdditionalDataToDateTime(ListItem det, string key)
    {
        var dateTime = det.Fields!.AdditionalData.ContainsKey(key) ? (DateTime)det.Fields.AdditionalData[key] : DateTime.MinValue;
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }

    private static int? AdditionalDataToInt(ListItem det, string key)
    {
        int? result = null;
        if (det.Fields!.AdditionalData.ContainsKey(key))
        {
            if (int.TryParse(det.Fields!.AdditionalData[key]!.ToString(), out var parsed))
                result = parsed;
        }

        return result;
    }

    private static double? AdditionalDataToDouble(ListItem det, string key)
    {
        double? result = null;
        if (det.Fields!.AdditionalData.ContainsKey(key))
        {
            if (double.TryParse(det.Fields!.AdditionalData[key]!.ToString(), out var parsed))
                result = parsed;
        }

        return result;
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

    private static void GetUser(List<SharePointUser> users, ListItem det, string key, SharePointRole role, SharePointListBase listBase, int order)
    {
        var sharePointId = det.Fields?.AdditionalData.ContainsKey(key) == true ? det.Fields.AdditionalData[key]?.ToString() : "";
        if (string.IsNullOrEmpty(sharePointId)) return;
        var user = (SharePointUser?)users.FirstOrDefault(x => x.SharePointID == sharePointId)?.Clone();
        if (user is null) return;
        user.Role = role;
        user.Order = order;
        listBase.Users!.Add(user);
    }

    public static async Task<Event?> AddToCalendar(Guid userId, string description, DateTime dateStart, DateTime dateEnd, bool isAllDay, ILogger logger)
    {
        try
        {
            Event body = GenerateCalendarBody(description, dateStart, dateEnd, isAllDay);
            var result = await _appClient.Users[userId.ToString()].Events
                .PostAsync(body, (requestConfiguration) => { requestConfiguration.Headers.Add("Prefer", "outlook.timezone=\"Pacific Standard Time\""); });

            var fromGet = await _appClient.Users[userId.ToString()].Events[result?.Id].GetAsync();
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in AddToCalendar");
        }

        return null;
    }

    public static async Task PatchCalender(Guid userId, string eventId, string description, DateTime dateStart, DateTime dateEnd, bool isAllDay)
    {
        try
        {
            var fromGet = await _appClient.Users[userId.ToString()].Events[eventId].GetAsync();
            if (fromGet is not null)
            {
                Event body = GenerateCalendarBody(description, dateStart, dateEnd, isAllDay);
                fromGet.Subject = body.Subject;
                fromGet.Body = body.Body;
                fromGet.Start = body.Start;
                fromGet.End = body.End;
                fromGet.IsAllDay = isAllDay;
                fromGet.AdditionalData = new Dictionary<string, object>(); // https://github.com/microsoftgraph/msgraph-sdk-dotnet/issues/2471
                var patch = await _appClient.Users[userId.ToString()].Events[eventId].PatchAsync(fromGet);
            }

            return;
        }
        catch (Microsoft.Graph.Models.ODataErrors.ODataError ex)
        {
            if (ex.ResponseStatusCode != 404)
                throw;
        }
    }

    private static Event GenerateCalendarBody(string description, DateTime dateStart, DateTime dateEnd, bool isAllDay)
    {
        var even = new Event()
        {
            Subject = description,
            Body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = description,
            },
        };
        if (isAllDay)
        {
            even.IsAllDay = true;
            even.Start = new DateTimeTimeZone
            {
                DateTime = dateStart.ToString("yyyy-MM-ddT00:00:00"),
                TimeZone = TimeZoneInfo.Local.Id
            };
            even.End = new DateTimeTimeZone
            {
                DateTime = dateEnd.AddDays(1).ToString("yyyy-MM-ddT00:00:00"),
                TimeZone = TimeZoneInfo.Local.Id
            };
        }
        else
        {
            even.Start = new DateTimeTimeZone
            {
                DateTime = dateStart.ToString("o"),
                TimeZone = "UTC",
            };
            even.End = new DateTimeTimeZone
            {
                DateTime = dateEnd.ToString("o"),
                TimeZone = "UTC",
            };
        }

        return even;
    }

    internal static async Task DeleteCalendarEvent(Guid? userId, string calendarEventId, CancellationToken clt)
    {
        try
        {
            var eve = await _appClient.Users[userId.ToString()].Events[calendarEventId].GetAsync();
            if (eve is not null)
            {
                await _appClient.Users[userId.ToString()].Events[calendarEventId].DeleteAsync();
            }
        }
        catch (Microsoft.Graph.Models.ODataErrors.ODataError ex)
        {
            if (ex.ResponseStatusCode != 404)
                throw;
        }
    }
}