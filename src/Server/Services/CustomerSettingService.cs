using Drogecode.Knrm.Oefenrooster.Server.Helpers;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class CustomerSettingService : ICustomerSettingService
{
    private readonly ILogger<CustomerSettingService> _logger;
    private readonly Database.DataContext _database;
    public CustomerSettingService(ILogger<CustomerSettingService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<bool> TrainingToCalendar(Guid customerId)
    {
        var result = await GetCustomerSetting(customerId, SettingNames.TRAINING_TO_CALENDAR, "false");
        return SettingNames.StringToBool(result);
    }

    public async Task Patch_TrainingToCalendar(Guid customerId, bool value)
    {
        await PatchCustomerSetting(customerId, SettingNames.TRAINING_TO_CALENDAR, value ? "true" : "false");
    }

    private async Task<string> GetCustomerSetting(Guid customerId, string setting, string def)
    {
        var result = await _database.CustomerSettings.Where(x => x.CustomerId == customerId && x.Setting == setting).FirstOrDefaultAsync();
        if (result?.Value is null) return def;
        return result.Value;
    }

    private async Task PatchCustomerSetting(Guid customerId, string setting, string value)
    {
        var result = await _database.CustomerSettings.Where(x => x.CustomerId == customerId  && x.Setting == setting).FirstOrDefaultAsync();
        if (result is null)
            _database.CustomerSettings.Add(new Database.Models.DbCustomerSettings
            {
                CustomerId = customerId,
                Setting = setting,
                Value = value
            });
        else
        {
            result.Value = value;
            _database.CustomerSettings.Update(result);
        }
        _database.SaveChanges();
    }
}
