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
        CalendarItemController calendarItemController,
        PreComController preComController,
        VehicleController vehicleController) :
        base(scheduleController, userController, functionController, holidayController, trainingTypesController, calendarItemController, preComController, vehicleController)
    {
    }

    [Fact]
    public async Task PutMonthItem()
    {
        var body = new RoosterItemMonth
        {
            Text = "PutMonthItem",
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

    [Fact]
    public async Task GetAllMonth()
    {
        var new1 = await AddCalendarMonthItem("GetAllMonth_1");
        var new2 = await AddCalendarMonthItem("GetAllMonth_2", short.Parse(DateTime.Today.AddMonths(1).Month.ToString()));
        var new3 = await AddCalendarMonthItem("GetAllMonth_3", short.Parse(DateTime.Today.Month.ToString()), short.Parse(DateTime.Today.AddYears(1).Year.ToString()));
        var result = await CalendarItemController.GetMonthItems(DateTime.Today.Year, DateTime.Today.Month);
        Assert.NotNull(result?.Value?.MonthItems);
        result.Value.MonthItems.Should().Contain(x => x.Id == DefaultCalendarMonthItem);
        result.Value.MonthItems.Should().Contain(x => x.Id == new1);
        result.Value.MonthItems.Should().NotContain(x => x.Id == new2);
        result.Value.MonthItems.Should().NotContain(x => x.Id == new3);
    }

    [Fact]
    public async Task GetAllDay()
    {
        var new1 = await AddCalendarDayItem("GetAllDay_1", DateTime.Today);
        var new2 = await AddCalendarDayItem("GetAllDay_2", DateTime.Today.AddDays(20));
        var new3 = await AddCalendarDayItem("GetAllDay_3", DateTime.Today.AddDays(30));
        var new4 = await AddCalendarDayItem("GetAllDay_4", DateTime.Today.AddDays(-10));
        var new5 = await AddCalendarDayItem("GetAllDay_5", DateTime.Today.AddDays(-20));
        var from = DateTime.Today.AddDays(-10);
        var till = DateTime.Today.AddDays(20);
        var result = await CalendarItemController.GetDayItems(from.Year, from.Month, from.Day, till.Year, till.Month, till.Day);
        Assert.NotNull(result?.Value?.DayItems);
        result.Value.DayItems.Should().Contain(x => x.Id == DefaultCalendarDayItem);
        result.Value.DayItems.Should().Contain(x => x.Id == new1);
        result.Value.DayItems.Should().Contain(x => x.Id == new2);
        result.Value.DayItems.Should().NotContain(x => x.Id == new3);
        result.Value.DayItems.Should().Contain(x => x.Id == new4);
        result.Value.DayItems.Should().NotContain(x => x.Id == new5);
    }
}
