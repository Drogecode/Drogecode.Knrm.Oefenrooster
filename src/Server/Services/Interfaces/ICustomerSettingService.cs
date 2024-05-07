namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ICustomerSettingService
{
    Task<bool> IosDarkLightCheck(Guid customerId);
    Task<bool> TrainingToCalendar(Guid customerId);
    Task Patch_TrainingToCalendar(Guid customerId, bool value);
}
