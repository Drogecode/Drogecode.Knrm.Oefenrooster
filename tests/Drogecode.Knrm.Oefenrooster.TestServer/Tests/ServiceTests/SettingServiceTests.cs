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
        var value = await _userSettingService.GetBoolUserSetting(DefaultSettingsHelper.KnrmHuizenId, Tester.DefaultUserId, SettingName.TrainingToCalendar, false, CancellationToken.None);
        Assert.False(value.Value);
        await _customerSettingService.PatchBoolSetting(DefaultSettingsHelper.KnrmHuizenId, SettingName.TrainingToCalendar, true); 
        value = await _userSettingService.GetBoolUserSetting(DefaultSettingsHelper.KnrmHuizenId, Tester.DefaultUserId, SettingName.TrainingToCalendar, false, CancellationToken.None);
        Assert.True(value.Value);
        await _userSettingService.PatchBoolSetting(DefaultSettingsHelper.KnrmHuizenId, Tester.DefaultUserId, SettingName.TrainingToCalendar, false);
        value = await _userSettingService.GetBoolUserSetting(DefaultSettingsHelper.KnrmHuizenId, Tester.DefaultUserId, SettingName.TrainingToCalendar, false, CancellationToken.None);
        Assert.False(value.Value);
        value = (await _customerSettingService.GetBoolCustomerSetting(DefaultSettingsHelper.KnrmHuizenId, SettingName.TrainingToCalendar, false, CancellationToken.None));
        Assert.True(value.Value);
    }
}
