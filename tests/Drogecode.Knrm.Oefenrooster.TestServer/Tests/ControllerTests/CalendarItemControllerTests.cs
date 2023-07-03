using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class CalendarItemControllerTests : BaseTest
{
    public CalendarItemControllerTests(
        ScheduleController scheduleController,
        FunctionController functionController,
        UserController userController,
        HolidayController holidayController,
        TrainingTypesController trainingTypesController,
        CalendarItemController calendarItemController) :
        base(scheduleController, userController, functionController, holidayController, trainingTypesController, calendarItemController)
    {
    }

    [Fact]
    public async Task PutMonthItem()
    {
        var body = new RoosterItemMonth
        {
            Text = "PutDayItem",
            Type = Shared.Enums.CalendarItemType.Custom,
            Month = short.Parse(DateTime.Today.Month.ToString())
        };
        var result = await CalendarItemController.PutMonthItem(body);
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
    }

    [Fact]
    public async Task PutDayItem()
    {
        var body = new RoosterItemDay
        {
            Text = "PutDayItem",
            Type = Shared.Enums.CalendarItemType.Custom,
            IsFullDay = true,
            DateStart = DateTime.Today.AddDays(7),
        };
        var result = await CalendarItemController.PutDayItem(body);
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
    }
}
