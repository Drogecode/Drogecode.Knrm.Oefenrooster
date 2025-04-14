using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Setting;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class CustomerSettingService : ICustomerSettingService
{
    private readonly ILogger<CustomerSettingService> _logger;
    private readonly DataContext _database;

    public CustomerSettingService(ILogger<CustomerSettingService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<Customer> GetByTenantId(string tenantId, CancellationToken clt)
    {
        var customer = await _database.Customers.FirstOrDefaultAsync(x => x.TenantId == tenantId, clt);
        return new Customer { Id = customer?.Id ?? Guid.Empty };
    }

    public async Task<SettingBoolResponse> GetBoolCustomerSetting(Guid customerId, SettingName setting)
    {
        var response = new SettingBoolResponse();
        var result = await GetStringCustomerSetting(customerId, setting, "false");
        response.Value = SettingNames.StringToBool(result.Value);
        response.Success = result.Success;
        return response;
    }

    public async Task<string> GetTimeZone(Guid customerId)
    {
        var customer = await _database.Customers.FindAsync(customerId);
        if (customer is not null)
        {
            return customer.TimeZone;
        }

        return string.Empty;
    }

    public async Task PatchBoolSetting(Guid customerId, SettingName setting, bool value)
    {
        await PatchStringSetting(customerId, SettingName.TrainingToCalendar, value ? "true" : "false");
    }

    public async Task<SettingStringResponse> GetStringCustomerSetting(Guid customerId, SettingName setting, string def)
    {
        if (setting == SettingName.TimeZone)
        {
            throw new DrogeCodeConfigurationException("Use CustomerSettingService.GetTimeZone(Guid customerId) for TimeZone");
        }

        var response = new SettingStringResponse
        {
            Value = def
        };
        var result = await _database.CustomerSettings
            .Where(x => x.CustomerId == customerId && x.Name == setting)
            .AsNoTracking()
            .FirstOrDefaultAsync();
        if (result?.Value is null) return response;
        response.Value = result.Value;
        response.Success = true;
        return response;
    }

    public async Task<SettingListIntResponse> GetListIntCustomerSetting(Guid customerId, SettingName setting, List<int> def)
    {
        var response = new SettingListIntResponse
        {
            Value = def
        };
        var result = await _database.CustomerSettings
            .Where(x => x.CustomerId == customerId && x.Name == setting)
            .AsNoTracking()
            .FirstOrDefaultAsync();
        if (result?.Value is null) return response;
        var deserialized = System.Text.Json.JsonSerializer.Deserialize<List<int>>(result.Value);
        if (deserialized is null) return response;
        response.Value = deserialized;
        return response;
    }

    public async Task PatchStringSetting(Guid customerId, SettingName setting, string value)
    {
        var result = await _database.CustomerSettings.Where(x => x.CustomerId == customerId && x.Name == setting).FirstOrDefaultAsync();
        if (result is null)
            _database.CustomerSettings.Add(new Database.Models.DbCustomerSettings
            {
                CustomerId = customerId,
                Name = setting,
                Value = value
            });
        else
        {
            result.Value = value;
            _database.CustomerSettings.Update(result);
        }

        await _database.SaveChangesAsync();
    }
}