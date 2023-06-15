using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IPreComService
{
    Task<MultiplePreComAlertsResponse> GetAllAlerts(Guid customerId);
    Task WriteAlertToDb(Guid customerId, Guid? notificationId, DateTime? sendTime, string alert, int? priority, string raw);
}
