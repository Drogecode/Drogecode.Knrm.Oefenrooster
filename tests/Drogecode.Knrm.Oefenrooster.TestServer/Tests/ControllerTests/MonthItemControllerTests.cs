using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class MonthItemControllerTests : BaseTest
{
    public MonthItemControllerTests(
        DataContext dataContext,
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
        DefaultScheduleController defaultScheduleController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, dayItemController, monthItemController,
            preComController, vehicleController, defaultScheduleController)
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
        var result = await MonthItemController.PutItem(body);
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
    }

    [Fact]
    public async Task GetCurentMonth()
    {
        var new1 = await AddCalendarMonthItem("GetAllMonth_1");
        var new2 = await AddCalendarMonthItem("GetAllMonth_2", short.Parse(DateTime.Today.AddMonths(1).Month.ToString()));
        var new3 = await AddCalendarMonthItem("GetAllMonth_3", short.Parse(DateTime.Today.AddMonths(-1).Month.ToString()));
        var new4 = await AddCalendarMonthItem("GetAllMonth_4", short.Parse(DateTime.Today.AddMonths(1).Month.ToString()), short.Parse(DateTime.Today.Year.ToString()));
        var new5 = await AddCalendarMonthItem("GetAllMonth_5", short.Parse(DateTime.Today.AddMonths(-1).Month.ToString()), short.Parse(DateTime.Today.Year.ToString()));
        var new6 = await AddCalendarMonthItem("GetAllMonth_6", short.Parse(DateTime.Today.Month.ToString()), short.Parse(DateTime.Today.AddYears(1).Year.ToString()));
        var new7 = await AddCalendarMonthItem("GetAllMonth_7", (short.Parse((DateTime.Today.Month - 1).ToString())), short.Parse(DateTime.Today.AddYears(1).Year.ToString()));
        var result = await MonthItemController.GetItems(DateTime.Today.Year, DateTime.Today.Month);
        Assert.NotNull(result?.Value?.MonthItems);
        result.Value.MonthItems.Should().Contain(x => x.Id == DefaultCalendarMonthItem);
        result.Value.MonthItems.Should().Contain(x => x.Id == new1);
        result.Value.MonthItems.Should().NotContain(x => x.Id == new2);
        result.Value.MonthItems.Should().NotContain(x => x.Id == new3);
        result.Value.MonthItems.Should().NotContain(x => x.Id == new4);
        result.Value.MonthItems.Should().NotContain(x => x.Id == new5);
        result.Value.MonthItems.Should().NotContain(x => x.Id == new6);
        result.Value.MonthItems.Should().NotContain(x => x.Id == new7);
    }

    [Fact]
    public async Task GetAllMonthItems()
    {
        var compareDate = DateTime.Today;
        var new1 = await AddCalendarMonthItem("GetAllMonth_1");
        var new2 = await AddCalendarMonthItem("GetAllMonth_2", short.Parse(compareDate.AddMonths(1).Month.ToString()));
        var new3 = await AddCalendarMonthItem("GetAllMonth_3", short.Parse(compareDate.AddMonths(-1).Month.ToString()));
        var new4 = await AddCalendarMonthItem("GetAllMonth_4", short.Parse(compareDate.AddMonths(1).Month.ToString()), short.Parse(compareDate.Year.ToString()));
        var new5 = await AddCalendarMonthItem("GetAllMonth_5", short.Parse(compareDate.AddMonths(-1).Month.ToString()), short.Parse(compareDate.Year.ToString()));
        var new6 = await AddCalendarMonthItem("GetAllMonth_6", short.Parse(compareDate.Month.ToString()), short.Parse(compareDate.AddYears(1).Year.ToString()));
        var new7 = await AddCalendarMonthItem("GetAllMonth_7", (short.Parse((compareDate.AddMonths(-1).Month).ToString())), short.Parse(compareDate.AddMonths(-1).AddYears(1).Year.ToString()));
        var result = await MonthItemController.GetAllItems(100, 0, true);
        Assert.NotNull(result?.Value?.MonthItems);
        result.Value.MonthItems.Should().Contain(x => x.Id == DefaultCalendarMonthItem);
        result.Value.MonthItems.Should().Contain(x => x.Id == new1);
        result.Value.MonthItems.Should().Contain(x => x.Id == new2);
        result.Value.MonthItems.Should().Contain(x => x.Id == new3);
        result.Value.MonthItems.Should().Contain(x => x.Id == new4);
        result.Value.MonthItems.Should().Contain(x => x.Id == new5);
        result.Value.MonthItems.Should().Contain(x => x.Id == new6);
        result.Value.MonthItems.Should().Contain(x => x.Id == new7);
    }

    [Fact]
    public async Task GetFutureMonthItems()
    {
        var compareDate = DateTime.Today;
        var new1 = await AddCalendarMonthItem("GetAllMonth_1");
        var new2 = await AddCalendarMonthItem("GetAllMonth_2", short.Parse(compareDate.AddMonths(1).Month.ToString()));
        var new3 = await AddCalendarMonthItem("GetAllMonth_3", short.Parse(compareDate.AddMonths(-1).Month.ToString()));
        var new4 = await AddCalendarMonthItem("GetAllMonth_4", short.Parse(compareDate.AddMonths(1).Month.ToString()), short.Parse(compareDate.Year.ToString()));
        var new5 = await AddCalendarMonthItem("GetAllMonth_5", short.Parse(compareDate.AddMonths(-1).Month.ToString()), short.Parse(compareDate.Year.ToString()));
        var new6 = await AddCalendarMonthItem("GetAllMonth_6", short.Parse(compareDate.Month.ToString()), short.Parse(compareDate.AddYears(1).Year.ToString()));
        var new7 = await AddCalendarMonthItem("GetAllMonth_7", (short.Parse((compareDate.AddMonths(-1).Month).ToString())), short.Parse(compareDate.AddMonths(-1).AddYears(1).Year.ToString()));
        var result = await MonthItemController.GetAllItems(100, 0, false);
        Assert.NotNull(result?.Value?.MonthItems);
        result.Value.MonthItems.Should().Contain(x => x.Id == DefaultCalendarMonthItem);
        result.Value.MonthItems.Should().Contain(x => x.Id == new1);
        result.Value.MonthItems.Should().Contain(x => x.Id == new2);
        result.Value.MonthItems.Should().Contain(x => x.Id == new3);
        result.Value.MonthItems.Should().Contain(x => x.Id == new4);
        result.Value.MonthItems.Should().NotContain(x => x.Id == new5);
        result.Value.MonthItems.Should().Contain(x => x.Id == new6);
        result.Value.MonthItems.Should().Contain(x => x.Id == new7);
    }
}
