﻿using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ServiceTests;

public class SettingServiceTests : BaseTest
{
    private IUserSettingService _userSettingService;
    private ICustomerSettingService _customerSettingService;
    public SettingServiceTests(
        DataContext dataContext,
        IUserSettingService userSettingService,
        ICustomerSettingService customerSettingService,
        IDateTimeService dateTimeServiceMock,
        ScheduleController scheduleController,
        FunctionController functionController,
        UserController userController,
        HolidayController holidayController,
        TrainingTypesController trainingTypesController,
        DayItemController dayItemController,
        MonthItemController monthItemController,
        PreComController preComController,
        VehicleController vehicleController,
        DefaultScheduleController defaultScheduleController,
        ReportActionController reportActionController,
        ReportTrainingController reportTrainingController,
        UserRoleController userRoleController,
        UserLinkedMailsController userLinkedMailsController,
        ReportActionSharedController reportActionSharedController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, dayItemController, monthItemController,
            preComController, vehicleController, defaultScheduleController, reportActionController, reportTrainingController, userRoleController, userLinkedMailsController, reportActionSharedController)
    {
        _userSettingService = userSettingService;
        _customerSettingService = customerSettingService;
    }

    [Fact]
    public async Task TrainingToCalendarTest()
    {
        var value = await _userSettingService.TrainingToCalendar(DefaultSettingsHelper.KnrmHuizenId, DefaultUserId);
        Assert.False(value);
        await _customerSettingService.PatchBoolSetting(DefaultSettingsHelper.KnrmHuizenId, SettingName.TrainingToCalendar, true); value = await _userSettingService.TrainingToCalendar(DefaultSettingsHelper.KnrmHuizenId, DefaultUserId);
        Assert.True(value);
        await _userSettingService.Patch_TrainingToCalendar(DefaultSettingsHelper.KnrmHuizenId, DefaultUserId, false);
        value = await _userSettingService.TrainingToCalendar(DefaultSettingsHelper.KnrmHuizenId, DefaultUserId);
        Assert.False(value);
        value = (await _customerSettingService.GetBoolCustomerSetting(DefaultSettingsHelper.KnrmHuizenId, SettingName.TrainingToCalendar)).Value;
        Assert.True(value);
    }
}
