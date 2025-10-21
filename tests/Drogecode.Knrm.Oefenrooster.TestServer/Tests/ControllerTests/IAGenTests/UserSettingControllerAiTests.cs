using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Setting;
using Microsoft.AspNetCore.Mvc;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests.IAGenTests;

public class UserSettingControllerAiTests : BaseTest
{
    public UserSettingControllerAiTests(TestService testService) : base(testService)
    {
    }

    #region GetStringSetting Tests

    [Fact]
    public async Task GetStringSetting_CalendarPrefix_ReturnsDefaultEmpty()
    {
        var result = await Tester.UserSettingController.GetStringSetting(SettingName.CalendarPrefix);
        
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
        result.Value.Value.Should().Be(string.Empty);
    }

    [Fact]
    public async Task GetStringSetting_PreComAvailableText_ReturnsDefaultEmpty()
    {
        var result = await Tester.UserSettingController.GetStringSetting(SettingName.PreComAvailableText);
        
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
        result.Value.Value.Should().Be(string.Empty);
    }

    [Fact]
    public async Task GetStringSetting_NonStringSettingName_ReturnsBadRequest()
    {
        var result = await Tester.UserSettingController.GetStringSetting(SettingName.TrainingToCalendar);
        
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region GetBoolSetting Tests

    [Fact]
    public async Task GetBoolSetting_TrainingToCalendar_ReturnsDefaultFalse()
    {
        var result = await Tester.UserSettingController.GetBoolSetting(SettingName.TrainingToCalendar);
        
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
        result.Value.Value.Should().BeFalse();
    }

    [Fact]
    public async Task GetBoolSetting_SyncPreComWithCalendar_ReturnsDefaultFalse()
    {
        var result = await Tester.UserSettingController.GetBoolSetting(SettingName.SyncPreComWithCalendar);
        
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
        result.Value.Value.Should().BeFalse();
    }

    [Fact]
    public async Task GetBoolSetting_SyncPreComDeleteOld_ReturnsDefaultFalse()
    {
        var result = await Tester.UserSettingController.GetBoolSetting(SettingName.SyncPreComDeleteOld);
        
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
        result.Value.Value.Should().BeFalse();
    }

    [Fact]
    public async Task GetBoolSetting_DelaySyncingTrainingToOutlook_ReturnsDefaultFalse()
    {
        var result = await Tester.UserSettingController.GetBoolSetting(SettingName.DelaySyncingTrainingToOutlook);
        
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
        result.Value.Value.Should().BeFalse();
    }

    [Fact]
    public async Task GetBoolSetting_SyncPreComWithExternal_ReturnsDefaultTrue()
    {
        var result = await Tester.UserSettingController.GetBoolSetting(SettingName.SyncPreComWithExternal);
        
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
        result.Value.Value.Should().BeTrue();
    }

    [Fact]
    public async Task GetBoolSetting_NonBoolSettingName_ReturnsBadRequest()
    {
        var result = await Tester.UserSettingController.GetBoolSetting(SettingName.CalendarPrefix);
        
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region PatchStringSetting Tests

    [Fact]
    public async Task PatchStringSetting_CalendarPrefix_UpdatesSuccessfully()
    {
        const string NEW_VALUE = "MyPrefix_";
        var body = new PatchSettingStringRequest(SettingName.CalendarPrefix, NEW_VALUE);

        var patchResult = await Tester.UserSettingController.PatchStringSetting(body);
        
        patchResult.Should().BeOfType<OkResult>();

        var getResult = await Tester.UserSettingController.GetStringSetting(SettingName.CalendarPrefix);
        Assert.NotNull(getResult?.Value);
        getResult.Value.Value.Should().Be(NEW_VALUE);
    }

    [Fact]
    public async Task PatchStringSetting_PreComAvailableText_UpdatesSuccessfully()
    {
        const string NEW_VALUE = "Available for duty";
        var body = new PatchSettingStringRequest(SettingName.PreComAvailableText, NEW_VALUE);

        var patchResult = await Tester.UserSettingController.PatchStringSetting(body);
        
        patchResult.Should().BeOfType<OkResult>();

        var getResult = await Tester.UserSettingController.GetStringSetting(SettingName.PreComAvailableText);
        Assert.NotNull(getResult?.Value);
        getResult.Value.Value.Should().Be(NEW_VALUE);
    }

    [Fact]
    public async Task PatchStringSetting_EmptyValue_UpdatesSuccessfully()
    {
        var body = new PatchSettingStringRequest(SettingName.CalendarPrefix, string.Empty);

        var patchResult = await Tester.UserSettingController.PatchStringSetting(body);
        
        patchResult.Should().BeOfType<OkResult>();

        var getResult = await Tester.UserSettingController.GetStringSetting(SettingName.CalendarPrefix);
        Assert.NotNull(getResult?.Value);
        getResult.Value.Value.Should().Be(string.Empty);
    }

    [Fact]
    public async Task PatchStringSetting_NullValue_UpdatesSuccessfully()
    {
        var body = new PatchSettingStringRequest(SettingName.CalendarPrefix, null);

        var patchResult = await Tester.UserSettingController.PatchStringSetting(body);
        
        patchResult.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task PatchStringSetting_MultipleUpdates_LastValuePersists()
    {
        const string VALUE1 = "First";
        const string VALUE2 = "Second";

        var body1 = new PatchSettingStringRequest(SettingName.CalendarPrefix, VALUE1);
        await Tester.UserSettingController.PatchStringSetting(body1);

        var body2 = new PatchSettingStringRequest(SettingName.CalendarPrefix, VALUE2);
        await Tester.UserSettingController.PatchStringSetting(body2);

        var getResult = await Tester.UserSettingController.GetStringSetting(SettingName.CalendarPrefix);
        Assert.NotNull(getResult?.Value);
        getResult.Value.Value.Should().Be(VALUE2);
    }

    #endregion

    #region PatchBoolSetting Tests

    [Fact]
    public async Task PatchBoolSetting_TrainingToCalendar_UpdatesSuccessfully()
    {
        var body = new PatchSettingBoolRequest(SettingName.TrainingToCalendar, true);

        var patchResult = await Tester.UserSettingController.PatchBoolSetting(body);
        
        patchResult.Should().BeOfType<OkResult>();

        var getResult = await Tester.UserSettingController.GetBoolSetting(SettingName.TrainingToCalendar);
        Assert.NotNull(getResult?.Value);
        getResult.Value.Value.Should().BeTrue();
    }

    [Fact]
    public async Task PatchBoolSetting_SyncPreComWithCalendar_UpdatesSuccessfully()
    {
        var body = new PatchSettingBoolRequest(SettingName.SyncPreComWithCalendar, true);

        var patchResult = await Tester.UserSettingController.PatchBoolSetting(body);
        
        patchResult.Should().BeOfType<OkResult>();

        var getResult = await Tester.UserSettingController.GetBoolSetting(SettingName.SyncPreComWithCalendar);
        Assert.NotNull(getResult?.Value);
        getResult.Value.Value.Should().BeTrue();
    }

    [Fact]
    public async Task PatchBoolSetting_DelaySyncingTrainingToOutlook_UpdatesSuccessfully()
    {
        var body = new PatchSettingBoolRequest(SettingName.DelaySyncingTrainingToOutlook, true);

        var patchResult = await Tester.UserSettingController.PatchBoolSetting(body);
        
        patchResult.Should().BeOfType<OkResult>();

        var getResult = await Tester.UserSettingController.GetBoolSetting(SettingName.DelaySyncingTrainingToOutlook);
        Assert.NotNull(getResult?.Value);
        getResult.Value.Value.Should().BeTrue();
    }

    [Fact]
    public async Task PatchBoolSetting_SyncPreComWithExternal_UpdatesFromTrueToFalse()
    {
        // Default is true for this setting
        var getBeforePatch = await Tester.UserSettingController.GetBoolSetting(SettingName.SyncPreComWithExternal);
        getBeforePatch.Value!.Value.Should().BeTrue();

        var body = new PatchSettingBoolRequest(SettingName.SyncPreComWithExternal, false);

        var patchResult = await Tester.UserSettingController.PatchBoolSetting(body);
        
        patchResult.Should().BeOfType<OkResult>();

        var getResult = await Tester.UserSettingController.GetBoolSetting(SettingName.SyncPreComWithExternal);
        Assert.NotNull(getResult?.Value);
        getResult.Value.Value.Should().BeFalse();
    }

    [Fact]
    public async Task PatchBoolSetting_ToggleValue_UpdatesCorrectly()
    {
        var body = new PatchSettingBoolRequest(SettingName.TrainingToCalendar, true);

        await Tester.UserSettingController.PatchBoolSetting(body);

        var getResult1 = await Tester.UserSettingController.GetBoolSetting(SettingName.TrainingToCalendar);
        getResult1.Value!.Value.Should().BeTrue();

        body.Value = false;
        await Tester.UserSettingController.PatchBoolSetting(body);

        var getResult2 = await Tester.UserSettingController.GetBoolSetting(SettingName.TrainingToCalendar);
        getResult2.Value!.Value.Should().BeFalse();
    }

    [Fact]
    public async Task PatchBoolSetting_MultipleSettings_UpdatesIndependently()
    {
        var body1 = new PatchSettingBoolRequest(SettingName.TrainingToCalendar, true);
        var body2 = new PatchSettingBoolRequest(SettingName.SyncPreComWithCalendar, false);

        await Tester.UserSettingController.PatchBoolSetting(body1);
        await Tester.UserSettingController.PatchBoolSetting(body2);

        var getResult1 = await Tester.UserSettingController.GetBoolSetting(SettingName.TrainingToCalendar);
        var getResult2 = await Tester.UserSettingController.GetBoolSetting(SettingName.SyncPreComWithCalendar);

        getResult1.Value!.Value.Should().BeTrue();
        getResult2.Value!.Value.Should().BeFalse();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task UserSettings_CompleteWorkflow_AllSettingsWork()
    {
        // Set all string settings
        await Tester.UserSettingController.PatchStringSetting(new PatchSettingStringRequest(SettingName.CalendarPrefix, "TEST_"));

        await Tester.UserSettingController.PatchStringSetting(new PatchSettingStringRequest(SettingName.PreComAvailableText,  "On Duty"));

        // Set all bool settings
        await Tester.UserSettingController.PatchBoolSetting(new PatchSettingBoolRequest(SettingName.TrainingToCalendar,  true));
        await Tester.UserSettingController.PatchBoolSetting(new PatchSettingBoolRequest(SettingName.SyncPreComWithCalendar,  true));

        // Verify all settings
        var calendarPrefix = await Tester.UserSettingController.GetStringSetting(SettingName.CalendarPrefix);
        var preComText = await Tester.UserSettingController.GetStringSetting(SettingName.PreComAvailableText);
        var trainingToCalendar = await Tester.UserSettingController.GetBoolSetting(SettingName.TrainingToCalendar);
        var syncPreCom = await Tester.UserSettingController.GetBoolSetting(SettingName.SyncPreComWithCalendar);

        calendarPrefix.Value!.Value.Should().Be("TEST_");
        preComText.Value!.Value.Should().Be("On Duty");
        trainingToCalendar.Value!.Value.Should().BeTrue();
        syncPreCom.Value!.Value.Should().BeTrue();
    }

    #endregion
}
