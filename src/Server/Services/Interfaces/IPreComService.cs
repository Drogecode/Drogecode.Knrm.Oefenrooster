namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IPreComService
{
    Task WriteAlertToDb(Guid customerId, Guid? notificationId, string alert, string raw);
}
