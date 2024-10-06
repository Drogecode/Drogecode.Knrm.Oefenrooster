using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
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

        _logger.LogTrace("Start ClientSecret: {ClientSecret}", settings.ClientSecret?[..3] ?? "Is null");
        GraphHelper.InitializeGraphForAppOnlyAuth(settings);
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
            .Where(x => x.CustomerId == customerId)
            .Include(x => x.Users)
            .ToListAsync(clt);

        var saveCount = 0;
        var changeCount = 0;
        foreach (var action in sharePointActions)
        {
            if (clt.IsCancellationRequested)
                return false;
            var dbAction = dbActions.FirstOrDefault(x => x.Id == action.Id);
            if (dbAction is null)
            {
                dbAction = action.ToDbReportAction(customerId);
                await _database.ReportActions.AddAsync(dbAction, clt);
                saveCount++;
            }
            else if (dbAction.LastUpdated != action.LastUpdated)
            {
                dbAction.UpdateDbReportAction(action, customerId);
                _database.ReportActions.Update(dbAction);
                if (dbAction.Users is not null)
                {
                    foreach (var user in dbAction.Users)
                    {
                        if (user.IsNew)
                            _database.ReportUsers.Add(user);
                        else
                            _database.ReportUsers.Update(user);
                    }
                }

                saveCount++;
            }

            if (saveCount < 50) continue;
            changeCount += await _database.SaveChangesAsync(clt);
            saveCount = 0;
        }

        changeCount += await _database.SaveChangesAsync(clt);
        if (changeCount > 0)
            _logger.LogInformation("SharePoint actions synced (count {changeCount})", changeCount);
        else
            _logger.LogTrace("SharePoint actions synced none");
        return changeCount > 0;
    }

    public async Task<bool> SyncSharePointTrainings(Guid customerId, CancellationToken clt)
    {
        var keyTrainings = string.Format(SP_TRAININGS, customerId);
        await ShouldUpdateCacheSharePointTrainings(customerId, keyTrainings);
        _memoryCache.TryGetValue<List<SharePointTraining>>(keyTrainings, out var sharePointTrainings);
        sharePointTrainings ??= await GetSharePointTrainings(customerId, keyTrainings, clt);

        if (sharePointTrainings == null || clt.IsCancellationRequested)
            return false;
        var dbTrainings = await _database.ReportTrainings
            .Where(x => x.CustomerId == customerId)
            .Include(x => x.Users)
            .ToListAsync(clt);

        var saveCount = 0;
        var changeCount = 0;
        foreach (var training in sharePointTrainings)
        {
            if (clt.IsCancellationRequested)
                return false;
            var dbTraining = dbTrainings.FirstOrDefault(x => x.Id == training.Id);
            if (dbTraining is null)
            {
                dbTraining = training.ToDbReportTraining(customerId);
                await _database.ReportTrainings.AddAsync(dbTraining, clt);
                saveCount++;
            }
            else if (dbTraining.LastUpdated != training.LastUpdated)
            {
                dbTraining.UpdateDbReportTraining(training, customerId);
                _database.ReportTrainings.Update(dbTraining);
                if (dbTraining.Users is not null)
                {
                    foreach (var user in dbTraining.Users)
                    {
                        if (user.IsNew)
                            _database.ReportUsers.Add(user);
                        else
                            _database.ReportUsers.Update(user);
                    }
                }

                saveCount++;
            }

            if (saveCount < 50) continue;
            changeCount += await _database.SaveChangesAsync(clt);
            saveCount = 0;
        }

        changeCount += await _database.SaveChangesAsync(clt);
        if (changeCount > 0)
            _logger.LogInformation("SharePoint training synced (count {changeCount})", changeCount);
        else
            _logger.LogTrace("SharePoint training synced none");
        return changeCount > 0;
    }

    public async Task<MultipleSharePointActionsResponse> GetListActionsUser(List<Guid?> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
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
                _logger.LogTrace("There are changes in the action list");
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

    public async Task<MultipleSharePointTrainingsResponse> GetListTrainingUser(List<Guid?> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
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
                _logger.LogTrace("There are changes in the training list");
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
            var dbUsers = await _database.Users.Where(x => x.CustomerId == customerId && x.DeletedOn == null).Select(x => new { x.Id, x.SharePointID, x.Name }).ToListAsync();
            foreach (var sharePointUser in sharePointUsers)
            {
                var dbUser = dbUsers.FirstOrDefault(x => x.SharePointID == sharePointUser.SharePointID);
                if (dbUser is null)
                {
                    dbUser = dbUsers.FirstOrDefault(x => x.Name == sharePointUser.Name);
                    if (dbUser is not null)
                        await _database.Users.Where(x => x.Id == dbUser.Id && x.CustomerId == customerId && x.DeletedOn == null)
                            .ExecuteUpdateAsync(x => x.SetProperty(p => p.SharePointID, sharePointUser.SharePointID), clt);
                }

                if (dbUser is not null)
                    sharePointUser.DrogeCodeId = dbUser.Id;
            }

            _ = _memoryCache.Set(string.Format(SP_USERS, customerId), sharePointUsers, DateTimeOffset.UtcNow.AddMinutes(30));
        }

        return sharePointUsers;
    }

    public async Task<Event?> AddToCalendar(Guid userId, string description, DateTime dateStart, DateTime dateEnd, bool isAllDay)
    {
        var result = await GraphHelper.AddToCalendar(userId, description, dateStart, dateEnd, isAllDay, _logger);
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

    public async Task<GetHistoricalResponse> SyncHistorical(Guid customerId, CancellationToken clt)
    {
        clt = CancellationToken.None; // Ignore CancellationToken
        var sw = Stopwatch.StartNew();
        var spUsers = await GetAllSharePointUsers(customerId, clt);

        var l2006 = new Guid("fbc090f0-b144-47b3-bede-f962b59c02c2");
        var l2007 = new Guid("58cbd576-4e47-4302-9210-952d67a728e6");
        var l2008 = new Guid("10aec7c3-d7d2-401b-a348-a712cff1d967");
        var l2009 = new Guid("872e8292-7a4a-42fc-adeb-1eeeaf63ddfa");
        var l2010 = new Guid("315d7b40-0d0f-4735-a090-3e57d8db372a");
        var l2011 = new Guid("a46548b3-e776-424b-af12-69bddc253437");
        var l2012 = new Guid("47c03884-8171-425f-8312-59f8fbf6947c");
        var l2013 = new Guid("18fb5d8c-4279-4baf-bb40-ed2e2f389001");
        var l2014 = new Guid("e8ba9706-2980-41a7-b4f2-592f6a78506d");
        var l2015 = new Guid("2d62b89e-52c8-4454-9559-5e50e4d8c3c7");
        var l2016 = new Guid("f88f4d9d-2553-451c-9df4-1a90d05cc11d");
        var l2017 = new Guid("d955d4be-59ea-4bab-bcc9-872d0c64e193");
        var l2018 = new Guid("ab2ac7f9-c829-4dcf-8583-3da3062979f6");
        var l2019 = new Guid("cd7548a1-c4ab-4477-bd38-37e3780bd79b");
        var l2020 = new Guid("4225c7ff-b29f-452c-8eaf-be59ff62b6ea");
        var l2021 = new Guid("0ef18f42-ea1b-4215-b110-fc914c01ecfe");

        var response = new GetHistoricalResponse();
        var changeCount = 0;
        foreach (var listId in new List<Guid> { l2021, l2020, l2019, l2018, l2017, l2016, l2015, l2014, l2013, l2012, l2011, l2010, l2009, l2008, l2007, l2006 })
        {
            var fromSharePoint = await GraphHelper.GetHistorical(customerId, spUsers, listId, clt);
            if (fromSharePoint.Actions is not null && !clt.IsCancellationRequested)
            {
                changeCount += await SyncHistoricalActions(customerId, clt, fromSharePoint.Actions);
            }

            if (fromSharePoint.Trainings is not null && !clt.IsCancellationRequested)
            {
                changeCount += await SyncHistoricalTrainings(customerId, clt, fromSharePoint.Trainings);
            }

            if (_database.ChangeTracker.HasChanges())
                changeCount += await _database.SaveChangesAsync(clt);
            _logger.LogInformation("SharePoint historical `{listId}` synced (count {changeCount})", listId, changeCount);
        }

        _logger.LogInformation("SharePoint historical synced (count {changeCount})", changeCount);
        sw.Stop();
        response.Success = changeCount > 0;
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }

    public async Task SendMail(Guid? userId, string emailAddress, string subject, string body, CancellationToken clt)
    {
        if (userId is null) return;
        await GraphHelper.SendMail(userId.Value, emailAddress, subject, body, clt);
    }

    private async Task<int> SyncHistoricalActions(Guid customerId, CancellationToken clt, List<SharePointAction> actions)
    {
        var saveCount = 0;
        var changeCount = 0;
        var dbActions = await _database.ReportActions
            .Where(x => x.CustomerId == customerId)
            .Include(x => x.Users)
            .ToListAsync(clt);
        foreach (var action in actions)
        {
            if (clt.IsCancellationRequested) return changeCount;
            var dbAction = dbActions.FirstOrDefault(x => x.Id == action.Id);
            if (dbAction is null)
            {
                dbAction = action.ToDbReportAction(customerId);
                await _database.ReportActions.AddAsync(dbAction, clt);
                saveCount++;
            }
            else if (true || dbAction.LastUpdated != action.LastUpdated) // Always update while debugging
            {
                dbAction.UpdateDbReportAction(action, customerId);
                _database.ReportActions.Update(dbAction);
                if (dbAction.Users is not null)
                {
                    foreach (var user in dbAction.Users)
                    {
                        if (user.IsNew)
                            _database.ReportUsers.Add(user);
                        else
                            _database.ReportUsers.Update(user);
                    }
                }

                saveCount++;
            }

            if (saveCount < 50) continue;
            if (_database.ChangeTracker.HasChanges())
                changeCount += await _database.SaveChangesAsync(clt);
            saveCount = 0;
        }

        return changeCount;
    }

    private async Task<int> SyncHistoricalTrainings(Guid customerId, CancellationToken clt, List<SharePointTraining> trainings)
    {
        var saveCount = 0;
        var changeCount = 0;
        var dbTrainings = await _database.ReportTrainings
            .Where(x => x.CustomerId == customerId)
            .Include(x => x.Users)
            .ToListAsync(clt);
        foreach (var training in trainings)
        {
            if (clt.IsCancellationRequested) return changeCount;
            var dbTraining = dbTrainings.FirstOrDefault(x => x.Id == training.Id);
            if (dbTraining is null)
            {
                dbTraining = training.ToDbReportTraining(customerId);
                await _database.ReportTrainings.AddAsync(dbTraining, clt);
                saveCount++;
            }
            else if (true || dbTraining.LastUpdated != training.LastUpdated) // Always update while debugging
            {
                dbTraining.UpdateDbReportTraining(training, customerId);
                _database.ReportTrainings.Update(dbTraining);
                if (dbTraining.Users is not null)
                {
                    foreach (var user in dbTraining.Users)
                    {
                        if (user.IsNew)
                            _database.ReportUsers.Add(user);
                        else
                            _database.ReportUsers.Update(user);
                    }
                }

                saveCount++;
            }

            if (saveCount < 50) continue;
            if (_database.ChangeTracker.HasChanges())
                changeCount += await _database.SaveChangesAsync(clt);
            saveCount = 0;
        }

        return changeCount;
    }

    private class UpdatedCheck
    {
        public DateTime LastUpdated { get; set; }
        public DateTime NextCheck { get; set; }
    }
}