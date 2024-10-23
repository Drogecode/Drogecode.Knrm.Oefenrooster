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
        var result = await GetUserSetting(customerId, userId, SettingNames.TRAINING_TO_CALENDAR);
        if (result is null)
            return await _customerSettingService.TrainingToCalendar(customerId);
        else
            return SettingNames.StringToBool(result);
    }

    public async Task Patch_TrainingToCalendar(Guid customerId, Guid userId, bool value)
    {
        await PatchUserSetting(customerId, userId, SettingNames.TRAINING_TO_CALENDAR, value ? "true" : "false");
    }

    public async Task<string> TrainingCalenderPrefix(Guid customerId, Guid userId)
    {
        var result = await GetUserSetting(customerId, userId, SettingNames.TRAINING_CALENDER_PREFIX);
        if (result is null)
            return await _customerSettingService.TrainingCalenderPrefix(customerId);
        return result;
    }

    private async Task<string?> GetUserSetting(Guid customerId, Guid userId, string setting)
    {
        var result = await _database.UserSettings.Where(x => x.CustomerId == customerId && x.UserId == userId && x.Setting == setting).FirstOrDefaultAsync();
        return result?.Value;
    }

    private async Task PatchUserSetting(Guid customerId, Guid userId, string setting, string value)
    {
        var result = await _database.UserSettings.Where(x => x.CustomerId == customerId && x.UserId == userId && x.Setting == setting).FirstOrDefaultAsync();
        if (result is null)
            _database.UserSettings.Add(new Database.Models.DbUserSettings
            {
                CustomerId = customerId,
                UserId = userId,
                Setting = setting,
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