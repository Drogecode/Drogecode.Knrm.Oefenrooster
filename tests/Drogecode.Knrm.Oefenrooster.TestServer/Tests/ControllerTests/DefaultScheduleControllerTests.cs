using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class DefaultScheduleControllerTests : BaseTest
{
    public DefaultScheduleControllerTests(
        DataContext dataContext,
        IDateTimeService dateTimeServiceMock,
        ScheduleController scheduleController,
        UserController userController,
        FunctionController functionController,
        HolidayController holidayController,
        TrainingTypesController trainingTypesController,
        CalendarItemController calendarItemController,
        PreComController preComController,
        VehicleController vehicleController,
        DefaultScheduleController defaultScheduleController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, calendarItemController, preComController,
            vehicleController, defaultScheduleController)
    {
    }

    [Fact]
    public async Task PutTest()
    {
        var body = new DefaultSchedule
        {
            RoosterTrainingTypeId = DefaultTrainingType,
            WeekDay = DayOfWeek.Tuesday,
            TimeStart = new TimeOnly(11, 0),
            TimeEnd = new TimeOnly(14, 0),
            ValidFromDefault = DateTime.Today,
            ValidUntilDefault = DateTime.Today.AddDays(7),
            CountToTrainingTarget = false,
            Order = 55
        };
        var result = await DefaultScheduleController.PutDefaultSchedule(body);
        Assert.NotNull(result?.Value?.DefaultSchedule?.Id);
    }

    [Fact]
    public async Task PatchForUserTest()
    {
        var body = new PatchDefaultUserSchedule
        {
            DefaultId = DefaultDefaultSchedule,
            Available = Availabilty.Available,
            ValidFromUser = DateTime.Today,
            ValidUntilUser = DateTime.Today.AddDays(7),
        };
        var result = await DefaultScheduleController.PatchDefaultScheduleForUser(body);
        Assert.NotNull(result?.Value?.Patched?.UserDefaultAvailableId);
    }
}
