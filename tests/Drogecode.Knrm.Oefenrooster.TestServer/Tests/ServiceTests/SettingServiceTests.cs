using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ServiceTests;

public class SettingServiceTests : BaseTest
{
    private readonly IUserSettingService _userSettingService;
    private readonly ICustomerSettingService _customerSettingService;
    public SettingServiceTests(
        IUserSettingService userSettingService,
        ICustomerSettingService customerSettingService,
        TestService testService) :
        base(testService)
    {
        _userSettingService = userSettingService;
        _customerSettingService = customerSettingService;
    }

    [Fact]
    public async Task TrainingToCalendarTest()
    {
        var value = await _userSettingService.TrainingToCalendar(DefaultSettingsHelper.KnrmHuizenId, Tester.DefaultUserId);
        Assert.False(value);
        await _customerSettingService.PatchBoolSetting(DefaultSettingsHelper.KnrmHuizenId, SettingName.TrainingToCalendar, true); value = await _userSettingService.TrainingToCalendar(DefaultSettingsHelper.KnrmHuizenId, Tester.DefaultUserId);
        Assert.True(value);
        await _userSettingService.Patch_TrainingToCalendar(DefaultSettingsHelper.KnrmHuizenId, Tester.DefaultUserId, false);
        value = await _userSettingService.TrainingToCalendar(DefaultSettingsHelper.KnrmHuizenId, Tester.DefaultUserId);
        Assert.False(value);
        value = (await _customerSettingService.GetBoolCustomerSetting(DefaultSettingsHelper.KnrmHuizenId, SettingName.TrainingToCalendar)).Value;
        Assert.True(value);
    }
}
