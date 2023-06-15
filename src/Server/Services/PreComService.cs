using Drogecode.Knrm.Oefenrooster.Server.Database.Models;

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

    public async Task WriteAlertToDb(Guid customerId, Guid? notificationId, string alert, string raw)
    {
        _database.PreComAlerts.Add(new DbPreComAlert { 
            CustomerId = customerId,
            NotificationId = notificationId,
            Alert = alert,
            Raw = raw
        });
    }
}
