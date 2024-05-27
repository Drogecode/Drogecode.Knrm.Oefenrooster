using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Drogecode.Knrm.Oefenrooster.Server.Helpers.JsonConverters;

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

        result.TotalCount = fromDb.Count();
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
        var dbPreComAlert = _database.PreComAlerts.FirstOrDefault(x => x.Id == alert.Id);
        if (dbPreComAlert is null) return false;
        dbPreComAlert.Alert = alert.Alert;
        dbPreComAlert.Priority = alert.Priority;
        dbPreComAlert.SendTime = alert.SendTime;
        _database.PreComAlerts.Update(dbPreComAlert);
       return await _database.SaveChangesAsync() > 0;
    }
}