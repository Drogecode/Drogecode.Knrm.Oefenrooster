using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using System.Diagnostics;
using System.Linq;

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
        var fromDb = _database.PreComAlerts.Where(x => x.CustomerId == customerId && x.UserId == userId).OrderByDescending(x=>x.SendTime);
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

    public async Task WriteAlertToDb(Guid userId, Guid customerId, DateTime? sendTime, string alert, int? priority, string raw, string? ip)
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
        await _database.SaveChangesAsync();
    }
}
