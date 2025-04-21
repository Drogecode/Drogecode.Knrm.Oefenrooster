using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Models.UserPreCom;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Setting;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserSettingService : IUserSettingService
{
    private readonly ILogger<UserSettingService> _logger;
    private readonly DataContext _database;
    private readonly ICustomerSettingService _customerSettingService;

    public UserSettingService(ILogger<UserSettingService> logger, DataContext database, ICustomerSettingService customerSettingService)
    {
        _logger = logger;
        _database = database;
        _customerSettingService = customerSettingService;
    }

    public async Task<SettingBoolResponse> GetBoolUserSetting(Guid customerId, Guid userId, SettingName setting)
    {
        var result = await GetUserSetting(customerId, userId, setting);
        if (result is null)
            return await _customerSettingService.GetBoolCustomerSetting(customerId, setting);
        return new SettingBoolResponse() { Value = SettingNames.StringToBool(result) };
    }

    public async Task<SettingStringResponse> GetStringUserSetting(Guid customerId, Guid userId, SettingName setting)
    {
        if (setting == SettingName.TimeZone)
        {
            throw new DrogeCodeConfigurationException("Use CustomerSettingService.GetTimeZone(Guid customerId) for TimeZone");
        }

        var result = await GetUserSetting(customerId, userId, setting);
        if (result is null)
            return await _customerSettingService.GetStringCustomerSetting(customerId, setting, string.Empty);
        return new SettingStringResponse() { Value = result };
    }

    public async Task PatchBoolSetting(Guid customerId, Guid userId, SettingName setting, bool value)
    {
        await PatchUserSetting(customerId, userId, setting, value ? "true" : "false");
    }

    public async Task PatchStringSetting(Guid customerId, Guid userId, SettingName setting, string value)
    {
        await PatchUserSetting(customerId, userId, setting, value);
    }

    public async Task<List<UserPreComIdAndValue>> GetAllPreComIdAndValue(Guid customerId, SettingName setting, CancellationToken clt)
    {
        var result = await _database.UserSettings
            .Where(x => x.CustomerId == customerId && x.Name == setting)
            .Include(x => x.User)
            .Select(x => new UserPreComIdAndValue
            {
                UserPreComId = x.User.PreComId,
                Value = SettingNames.StringToBool(x.Value ?? ""),
            })
            .ToListAsync(clt);
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