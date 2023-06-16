﻿using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;

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

    public async Task<MultiplePreComAlertsResponse> GetAllAlerts(Guid customerId)
    {
        var result = new MultiplePreComAlertsResponse { PreComAlerts = new List<PreComAlert>() };
        var fromDb = _database.PreComAlerts.Where(x => x.CustomerId == customerId);
        foreach (var alert in fromDb)
        {
            result.PreComAlerts.Add(new PreComAlert
            {
                Id = alert.Id,
                NotificationId = alert.NotificationId,
                Alert = alert.Alert,
                SendTime = alert.SendTime,
            });
        }
        return result;
    }

    public async Task WriteAlertToDb(Guid customerId, Guid? notificationId, DateTime? sendTime, string alert, int? priority, string raw)
    {
        _database.PreComAlerts.Add(new DbPreComAlert
        {
            CustomerId = customerId,
            NotificationId = notificationId,
            Alert = alert,
            Priority = priority,
            Raw = raw,
            SendTime = sendTime
        });
        _database.SaveChanges();
    }
}