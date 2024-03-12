using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IPreComService
{
    Task<MultiplePreComAlertsResponse> GetAllAlerts(Guid userId, Guid customerId, int take, int skip, CancellationToken clt);
    Task WriteAlertToDb(Guid userId, Guid customerId, Guid? notificationId, DateTime? sendTime, string alert, int? priority, string raw);
}
