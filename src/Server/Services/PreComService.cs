using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Helpers.JsonConverters;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using System.Diagnostics;
using System.Text.Json;
using Drogecode.Knrm.Oefenrooster.PreCom;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class PreComService : DrogeService, IPreComService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public PreComService(ILogger<PreComService> logger,
        DataContext database,
        IMemoryCache memoryCache,
        IDateTimeService dateTimeService,
        HttpClient httpClient,
        IConfiguration configuration) : base(logger, database, memoryCache, dateTimeService)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<PreComClient?> GetPreComClient()
    {
        var preComClient = new PreComClient(_httpClient, "drogecode", Logger);
        var preComUser = _configuration.GetValue<string>("PreCom:User");
        var preComPassword = _configuration.GetValue<string>("PreCom:Password");
        if (string.IsNullOrWhiteSpace(preComUser) || string.IsNullOrWhiteSpace(preComPassword))
        {
            preComUser = KeyVaultHelper.GetSecret("PreComUser", Logger)?.Value;
            preComPassword = KeyVaultHelper.GetSecret("PreComPassword", Logger)?.Value;
            if (string.IsNullOrWhiteSpace(preComUser) || string.IsNullOrWhiteSpace(preComPassword))
                return null;
        }

        await preComClient.Login(preComUser, preComPassword);

        return preComClient;
    }

    public async Task<MultiplePreComAlertsResponse> GetAllAlerts(Guid userId, Guid customerId, int take, int skip, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultiplePreComAlertsResponse { PreComAlerts = new List<PreComAlert>() };
        var fromDb = Database.PreComAlerts.Where(x => x.CustomerId == customerId && x.UserId == userId).OrderByDescending(x => x.SendTime);
        foreach (var alert in await fromDb.Skip(skip).Take(take).ToListAsync(clt))
        {
            result.PreComAlerts.Add(new PreComAlert
            {
                Id = alert.Id,
                Alert = alert.Alert,
                SendTime = alert.SendTime,
                Priority = alert.Priority,
                RawJson = alert.Raw,
            });
        }

        result.TotalCount = await fromDb.CountAsync(clt);
        result.Success = true;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public string AnalyzeAlert(Guid userId, Guid customerId, object? body, out DateTime timestamp, out int? priority)
    {
        var jsonSerializerOptions = new JsonSerializerOptions()
        {
            Converters = { new PreComConverter() }
        };
        var ser = JsonSerializer.Serialize(body);
        var dataIos = JsonSerializer.Deserialize<NotificationDataBase>(ser, jsonSerializerOptions);
        var dataAndroid = JsonSerializer.Deserialize<NotificationDataAndroid>(ser);
        Logger.LogInformation($"alert is '{dataIos?._alert}'");
        var alert = dataIos?._alert ?? dataAndroid?.data?.message;
        timestamp = DateTime.SpecifyKind(dataIos?._data?.actionData?.Timestamp ?? DateTime.MinValue, DateTimeKind.Utc);
        if (dataIos is NotificationDataTestWebhookObject)
        {
            NotificationDataTestWebhookObject? testWebhookData = (NotificationDataTestWebhookObject)dataIos;
            alert ??= testWebhookData?.message;
            if (timestamp == DateTime.MinValue && testWebhookData?.messageData?.sentTime is not null)
            {
                timestamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                timestamp = timestamp.AddMilliseconds(testWebhookData.messageData.sentTime);
            }
        }

        if (dataAndroid?.data?.messageData is not null)
        {
            timestamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            timestamp = timestamp.AddMilliseconds(dataAndroid.sentTime);
        }

        alert ??= "No alert found by hui.nu webhook";
        if (timestamp.Equals(DateTime.MinValue))
            timestamp = DateTimeService.Now();

        var prioParsed = int.TryParse(dataIos?._data?.priority, out var prio); // Android does not have prio in JSON.
        priority = prioParsed ? prio : null;
        return alert;
    }

    public async Task<bool> WriteAlertToDb(Guid userId, Guid customerId, DateTime? sendTime, string alert, int? priority, string raw, string? ip)
    {
        var cacheKey = $"PreComAlert-{userId}-{customerId}-{ip}-{raw.GetHashCode()}";
        MemoryCache.TryGetValue(cacheKey, out bool? cached);
        if (cached is true)
        {
            Logger.LogInformation("Alert already exists in db for user {UserId} and customer {CustomerId}", userId, customerId);
            return false;
        }

        Database.PreComAlerts.Add(new DbPreComAlert
        {
            UserId = userId,
            CustomerId = customerId,
            Alert = alert,
            Priority = priority,
            Raw = raw,
            SendTime = sendTime,
            Ip = ip,
        });
        await Database.SaveChangesAsync();
        MemoryCache.Set(cacheKey, true, TimeSpan.FromMinutes(1));
        return true;
    }

    public async Task<bool> PatchAlertToDb(DbPreComAlert alert)
    {
        var dbPreComAlert = await Database.PreComAlerts.FirstOrDefaultAsync(x => x.Id == alert.Id);
        if (dbPreComAlert is null) return false;
        dbPreComAlert.Alert = alert.Alert;
        dbPreComAlert.Priority = alert.Priority;
        dbPreComAlert.SendTime = alert.SendTime;
        Database.PreComAlerts.Update(dbPreComAlert);
        return await Database.SaveChangesAsync() > 0;
    }

    public async Task<PutPreComForwardResponse> PutForward(PreComForward forward, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PutPreComForwardResponse();
        if (await Database.PreComForwards.CountAsync(x => x.CustomerId == customerId && x.UserId == userId && x.DeletedBy == null, cancellationToken: clt) > 5)
        {
            sw.Stop();
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return result;
        }

        forward.Id = Guid.NewGuid();
        forward.CreatedOn = DateTime.UtcNow;
        forward.CreatedBy = userId;
        if (!string.IsNullOrEmpty(forward.ForwardUrl))
        {
            await Database.PreComForwards.AddAsync(forward.ToDb(customerId, userId), clt);
            result.Success = await Database.SaveChangesAsync(clt) > 0;
        }

        if (result.Success)
        {
            result.NewId = forward.Id;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<PatchPreComForwardResponse> PatchForward(PreComForward forward, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PatchPreComForwardResponse();

        var dbForward = await Database.PreComForwards.FirstOrDefaultAsync(x => x.Id == forward.Id && x.CustomerId == customerId && x.UserId == userId && x.DeletedBy == null, clt);
        if (dbForward is not null && !string.IsNullOrEmpty(forward.ForwardUrl))
        {
            dbForward.ForwardUrl = forward.ForwardUrl;
            Database.Update(dbForward);
            result.Success = await Database.SaveChangesAsync(clt) > 0;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<MultiplePreComForwardsResponse> GetAllForwards(int take, int skip, Guid userId, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultiplePreComForwardsResponse
        {
            PreComForwards = new List<PreComForward>()
        };
        var dbForwards = Database.PreComForwards.Where(x => x.CustomerId == customerId && x.UserId == userId && x.DeletedBy == null);
        result.TotalCount = await dbForwards.CountAsync(clt);
        foreach (var forward in await dbForwards.OrderBy(x => x.CreatedOn).Skip(skip).Take(take).ToListAsync(clt))
        {
            result.PreComForwards.Add(forward.ToPreComForward());
        }

        result.Success = true;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<PreComForward?> GetForward(Guid forwardId, Guid customerId, CancellationToken clt)
    {
        var dbForward = await Database.PreComForwards.FirstOrDefaultAsync(x => x.Id == forwardId && x.CustomerId == customerId, clt);
        return dbForward?.ToPreComForward();
    }

    public async Task<DeleteResponse> DeleteDuplicates()
    {
        var sw = Stopwatch.StartNew();
        var range = 1000;
        var deleted = 0;
        var saved = new List<Guid>();
        var result = new DeleteResponse()
        {
            Success = true
        };
        for (var i = 0; i < 10000; i++)
        {
            var selection = Database.PreComAlerts.Skip(i * range).Take(range).Select(x => x).OrderBy(x => x.SendTime).ToList();
            if (selection.Count == 0)
                break;
            foreach (var alert in selection)
            {
                saved.Add(alert.Id);
                var duplicates = Database.PreComAlerts.Where(x => x.UserId == alert.UserId && x.CustomerId == alert.CustomerId && x.Id != alert.Id && x.Raw == alert.Raw).ToList();
                if (duplicates.Count <= 0 || duplicates.Any(x => saved.Contains(x.Id)))
                    continue;
                deleted += duplicates.Count;
                Database.PreComAlerts.RemoveRange(duplicates);
            }
        }

        var savedResult = await Database.SaveChangesAsync();
        if (savedResult > 0)
        {
            result.Success = true;
            Logger.LogInformation("Deleted `{byCounter}` `{bySave}` duplicates", deleted, savedResult);
        }
        else
        {
            result.Success = false;
            Logger.LogInformation("Nothing deleted `{byCounter}`", deleted);
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}