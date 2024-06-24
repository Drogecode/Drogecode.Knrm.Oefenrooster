using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using MudBlazor;
using ZXing;

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
        DefaultScheduleController defaultScheduleController,
        ReportActionController reportActionController,
        ReportTrainingController reportTrainingController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, dayItemController, monthItemController,
            preComController, vehicleController, defaultScheduleController, reportActionController, reportTrainingController)
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
    public async Task GetMonthItemById()
    {
        var item = await MonthItemController.ById(DefaultCalendarMonthItem);
        Assert.NotNull(item?.Value?.MonthItem);
        item.Value.MonthItem.Text.Should().Be("xUnit month item");
        item.Value.MonthItem.Id.Should().Be(DefaultCalendarMonthItem);
        item.Value.MonthItem.Month.Should().Be(3);
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
        result.Value.MonthItems.Should().NotContain(x => x.Id == DefaultCalendarMonthItem);
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
        var new1 = await AddCalendarMonthItem("GetAllMonthItems_1");
        var new2 = await AddCalendarMonthItem("GetAllMonthItems_2", short.Parse(compareDate.AddMonths(1).Month.ToString()));
        var new3 = await AddCalendarMonthItem("GetAllMonthItems_3", short.Parse(compareDate.AddMonths(-1).Month.ToString()));
        var new4 = await AddCalendarMonthItem("GetAllMonthItems_4", short.Parse(compareDate.AddMonths(1).Month.ToString()), short.Parse(compareDate.Year.ToString()));
        var new5 = await AddCalendarMonthItem("GetAllMonthItems_5", short.Parse(compareDate.AddMonths(-1).Month.ToString()), short.Parse(compareDate.Year.ToString()));
        var new6 = await AddCalendarMonthItem("GetAllMonthItems_6", short.Parse(compareDate.Month.ToString()), short.Parse(compareDate.AddYears(1).Year.ToString()));
        var new7 = await AddCalendarMonthItem("GetAllMonthItems_7", (short.Parse((compareDate.AddMonths(-1).Month).ToString())), short.Parse(compareDate.AddMonths(-1).AddYears(1).Year.ToString()));
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
        var new1 = await AddCalendarMonthItem("GetFutureMonthItems_1");
        var new2 = await AddCalendarMonthItem("GetFutureMonthItems_2", short.Parse(compareDate.AddMonths(1).Month.ToString()));
        var new3 = await AddCalendarMonthItem("GetFutureMonthItems_3", short.Parse(compareDate.AddMonths(-1).Month.ToString()));
        var new4 = await AddCalendarMonthItem("GetFutureMonthItems_4", short.Parse(compareDate.AddMonths(1).Month.ToString()), short.Parse(compareDate.Year.ToString()));
        var new5 = await AddCalendarMonthItem("GetFutureMonthItems_5", short.Parse(compareDate.AddMonths(-1).Month.ToString()), short.Parse(compareDate.Year.ToString()));
        var new6 = await AddCalendarMonthItem("GetFutureMonthItems_6", short.Parse(compareDate.Month.ToString()), short.Parse(compareDate.AddYears(1).Year.ToString()));
        var new7 = await AddCalendarMonthItem("GetFutureMonthItems_7", (short.Parse((compareDate.AddMonths(-1).Month).ToString())), short.Parse(compareDate.AddMonths(-1).AddYears(1).Year.ToString()));
        var result = await MonthItemController.GetAllItems(100, 0, false);
        Assert.NotNull(result?.Value?.MonthItems);
        result.Value.MonthItems.Should().NotContain(x => x.Id == DefaultCalendarMonthItem);
        result.Value.MonthItems.Should().Contain(x => x.Id == new1);
        result.Value.MonthItems.Should().Contain(x => x.Id == new2);
        result.Value.MonthItems.Should().Contain(x => x.Id == new3);
        result.Value.MonthItems.Should().Contain(x => x.Id == new4);
        result.Value.MonthItems.Should().NotContain(x => x.Id == new5);
        result.Value.MonthItems.Should().Contain(x => x.Id == new6);
        result.Value.MonthItems.Should().Contain(x => x.Id == new7);
    }

    [Fact]
    public async Task PatchMonthItem()
    {
        var new1 = await AddCalendarMonthItem("PatchMonthItem_1");
        var item = (await MonthItemController.ById(new1)).Value!.MonthItem;
        item!.Text = "Patched by xUnit";
        item.Year = 2022;
        item.Month = 5;
        item.Order = 69;
        item.Severity = Severity.Warning;
        var patchResult = await MonthItemController.PatchItem(item);
        Assert.NotNull(patchResult.Value?.Success);
        Assert.True(patchResult.Value.Success);
        var itemPatched = (await MonthItemController.ById(new1)).Value!.MonthItem;
        Assert.NotNull(itemPatched?.Text);
        itemPatched.Text.Should().Be("Patched by xUnit");
        itemPatched.Year.Should().Be(2022);
        itemPatched.Month.Should().Be(5);
        itemPatched.Order.Should().Be(69);
        itemPatched.Severity.Should().Be(Severity.Warning);
    }

    [Fact]
    public async Task DeleteMonthItem()
    {
        var new1 = await AddCalendarMonthItem("DeleteMonthItem_1");
        var item = await MonthItemController.ById(new1);
        Assert.NotNull(item?.Value?.MonthItem);
        var delete = await MonthItemController.DeleteMonthItem(new1);
        Assert.True(delete.Value);
        item = await MonthItemController.ById(new1);
        Assert.Null(item?.Value?.MonthItem);
    }
}
