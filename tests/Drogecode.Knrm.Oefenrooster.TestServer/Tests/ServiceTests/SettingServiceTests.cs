﻿using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ServiceTests;

public class SettingServiceTests : BaseTest
{
    private IUserSettingService _userSettingService;
    private ICustomerSettingService _customerSettingService;
    public SettingServiceTests(IUserSettingService userSettingService,
        ICustomerSettingService customerSettingService,
        IDateTimeService dateTimeServiceMock,
        ScheduleController scheduleController,
        FunctionController functionController,
        UserController userController,
        HolidayController holidayController,
        TrainingTypesController trainingTypesController,
        CalendarItemController calendarItemController,
        PreComController preComController,
        VehicleController vehicleController) :
        base(dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, calendarItemController, preComController, vehicleController)
    {
        _userSettingService = userSettingService;
        _customerSettingService = customerSettingService;
    }

    [Fact]
    public async Task TrainingToCalendarTest()
    {
        var value = await _userSettingService.TrainingToCalendar(DefaultSettingsHelper.KnrmHuizenId, DefaultUserId);
        Assert.False(value);
        await _customerSettingService.Patch_TrainingToCalendar(DefaultSettingsHelper.KnrmHuizenId, true); value = await _userSettingService.TrainingToCalendar(DefaultSettingsHelper.KnrmHuizenId, DefaultUserId);
        Assert.True(value);
        await _userSettingService.Patch_TrainingToCalendar(DefaultSettingsHelper.KnrmHuizenId, DefaultUserId, false);
        value = await _userSettingService.TrainingToCalendar(DefaultSettingsHelper.KnrmHuizenId, DefaultUserId);
        Assert.False(value);
        value = await _customerSettingService.TrainingToCalendar(DefaultSettingsHelper.KnrmHuizenId);
        Assert.True(value);
    }
}
