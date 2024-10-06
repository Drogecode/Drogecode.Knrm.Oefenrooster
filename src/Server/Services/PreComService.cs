using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Drogecode.Knrm.Oefenrooster.Server.Helpers.JsonConverters;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class PreComService : IPreComService
{
    private readonly ILogger<PreComService> _logger;
    private readonly Database.DataContext _database;

    public PreComService(ILogger<PreComService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<MultiplePreComAlertsResponse> GetAllAlerts(Guid userId, Guid customerId, int take, int skip, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultiplePreComAlertsResponse { PreComAlerts = new List<PreComAlert>() };
        var fromDb = _database.PreComAlerts.Where(x => x.CustomerId == customerId && x.UserId == userId).OrderByDescending(x => x.SendTime);
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

    public string AnalyzeAlert(Guid userId, Guid customerId, object body, out DateTime timestamp, out int? priority)
    {
        var jsonSerializerOptions = new JsonSerializerOptions()
        {
            Converters = { new PreComConverter() }
        };
        var ser = JsonSerializer.Serialize(body);
        var dataIos = JsonSerializer.Deserialize<NotificationDataBase>(ser, jsonSerializerOptions);
        var dataAndroid = JsonSerializer.Deserialize<NotificationDataAndroid>(ser);
        _logger.LogInformation($"alert is '{dataIos?._alert}'");
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
            timestamp = DateTime.Now;

        var prioParsed = int.TryParse(dataIos?._data?.priority, out int prio); // Android does not have prio in json.
        priority = prioParsed ? prio : null;
        return alert;
    }

    public void WriteAlertToDb(Guid userId, Guid customerId, DateTime? sendTime, string alert, int? priority, string raw, string? ip)
    {
        _database.PreComAlerts.Add(new DbPreComAlert
        {
            UserId = userId,
            CustomerId = customerId,
            Alert = alert,
            Priority = priority,
            Raw = raw,
            SendTime = sendTime,
            Ip = ip,
        });
        _database.SaveChanges();
    }

    public async Task<bool> PatchAlertToDb(DbPreComAlert alert)
    {
        var dbPreComAlert = await _database.PreComAlerts.FirstOrDefaultAsync(x => x.Id == alert.Id);
        if (dbPreComAlert is null) return false;
        dbPreComAlert.Alert = alert.Alert;
        dbPreComAlert.Priority = alert.Priority;
        dbPreComAlert.SendTime = alert.SendTime;
        _database.PreComAlerts.Update(dbPreComAlert);
        return await _database.SaveChangesAsync() > 0;
    }

    public async Task<PutPreComForwardResponse> PutForward(PreComForward forward, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PutPreComForwardResponse();
        if (await _database.PreComForwards.CountAsync(x => x.CustomerId == customerId && x.UserId == userId && x.DeletedBy == null, cancellationToken: clt) > 5)
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
            await _database.PreComForwards.AddAsync(forward.ToDb(customerId, userId), clt);
            result.Success = await _database.SaveChangesAsync(clt) > 0;
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

        var dbForward = await _database.PreComForwards.FirstOrDefaultAsync(x => x.Id == forward.Id && x.CustomerId == customerId && x.UserId == userId && x.DeletedBy == null, clt);
        if (dbForward is not null && !string.IsNullOrEmpty(forward.ForwardUrl))
        {
            dbForward.ForwardUrl = forward.ForwardUrl;
            _database.Update(dbForward);
            result.Success = await _database.SaveChangesAsync(clt) > 0;
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
        var dbForwards = _database.PreComForwards.Where(x => x.CustomerId == customerId && x.UserId == userId && x.DeletedBy == null);
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
}