namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IUserSettingService
{
    Task<bool> TrainingToCalendar(Guid customerId, Guid userId);
    Task Patch_TrainingToCalendar(Guid customerId, Guid userId, bool value);
    Task<string> TrainingCalenderPrefix(Guid customerId, Guid userId);
}
