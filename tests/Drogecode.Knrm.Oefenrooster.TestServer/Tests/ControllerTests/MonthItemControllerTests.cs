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
    public async Task GetAllMonth()
    {
        var new1 = await AddCalendarMonthItem("GetAllMonth_1");
        var new2 = await AddCalendarMonthItem("GetAllMonth_2", short.Parse(DateTime.Today.AddMonths(1).Month.ToString()));
        var new3 = await AddCalendarMonthItem("GetAllMonth_3", short.Parse(DateTime.Today.Month.ToString()), short.Parse(DateTime.Today.AddYears(1).Year.ToString()));
        var result = await MonthItemController.GetItems(DateTime.Today.Year, DateTime.Today.Month);
        Assert.NotNull(result?.Value?.MonthItems);
        result.Value.MonthItems.Should().Contain(x => x.Id == DefaultCalendarMonthItem);
        result.Value.MonthItems.Should().Contain(x => x.Id == new1);
        result.Value.MonthItems.Should().NotContain(x => x.Id == new2);
        result.Value.MonthItems.Should().NotContain(x => x.Id == new3);
    }
}
