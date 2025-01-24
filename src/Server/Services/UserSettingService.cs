using Drogecode.Knrm.Oefenrooster.Server.Helpers;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserSettingService : IUserSettingService
{
    private readonly ILogger<UserSettingService> _logger;
    private readonly Database.DataContext _database;
    private readonly ICustomerSettingService _customerSettingService;

    public UserSettingService(ILogger<UserSettingService> logger, Database.DataContext database, ICustomerSettingService customerSettingService)
    {
        _logger = logger;
        _database = database;
        _customerSettingService = customerSettingService;
    }

    public async Task<bool> TrainingToCalendar(Guid customerId, Guid userId)
    {
        var result = await GetUserSetting(customerId, userId, SettingName.TrainingToCalendar);
        if (result is null)
            return (await _customerSettingService.GetBoolCustomerSetting(customerId, SettingName.TrainingToCalendar)).Value;
        else
            return SettingNames.StringToBool(result);
    }

    public async Task Patch_TrainingToCalendar(Guid customerId, Guid userId, bool value)
    {
        await PatchUserSetting(customerId, userId, SettingName.TrainingToCalendar, value ? "true" : "false");
    }

    public async Task<string> TrainingCalenderPrefix(Guid customerId, Guid userId)
    {
        var result = await GetUserSetting(customerId, userId, SettingName.CalendarPrefix);
        if (result is null)
            return (await _customerSettingService.GetStringCustomerSetting(customerId, SettingName.CalendarPrefix, string.Empty)).Value;
        return result;
    }

    private async Task<string?> GetUserSetting(Guid customerId, Guid userId, SettingName name)
    {
        var result = await _database.UserSettings.Where(x => x.CustomerId == customerId && x.UserId == userId && x.Name == name).FirstOrDefaultAsync();
        return result?.Value;
    }

    private async Task PatchUserSetting(Guid customerId, Guid userId, SettingName name, string value)
    {
        var result = await _database.UserSettings.Where(x => x.CustomerId == customerId && x.UserId == userId && x.Name == name).FirstOrDefaultAsync();
        if (result is null)
            _database.UserSettings.Add(new Database.Models.DbUserSettings
            {
                CustomerId = customerId,
                UserId = userId,
                Name = name,
                Value = value
            });
        else
        {
            result.Value = value;
            _database.UserSettings.Update(result);
        }

        await _database.SaveChangesAsync();
    }
}