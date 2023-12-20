using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class CalendarItemControllerTests : BaseTest
{
    public CalendarItemControllerTests(
        DataContext dataContext,
        IDateTimeService dateTimeServiceMock,
        ScheduleController scheduleController,
        FunctionController functionController,
        UserController userController,
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
    public async Task GetDayItem()
    {
        var result = await CalendarItemController.GetDayItemById(DefaultCalendarDayItem);
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
        Assert.NotNull(result.Value.DayItem?.Id);
        result.Value.DayItem.Text.Should().Be(TRAINING_CALENDAR_DAY);
    }

    [Fact]
    public async Task PatchDayItem()
    {
        var patchedText = "PatchDayItem";

        var result = await CalendarItemController.GetDayItemById(DefaultCalendarDayItem);
        var oldDayItem = result.Value?.DayItem;
        Assert.NotNull(oldDayItem);
        oldDayItem.Text.Should().NotBe(patchedText);
        oldDayItem.Text = patchedText;

        var patched = await CalendarItemController.PatchDayItem(oldDayItem);
        Assert.NotNull(patched.Value);
        Assert.True(patched.Value.Success);

        var getAfterPatch = await CalendarItemController.GetDayItemById(DefaultCalendarDayItem);
        Assert.NotNull(getAfterPatch.Value?.DayItem);
        getAfterPatch.Value.DayItem.Text.Should().Be(patchedText);
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
        var new4 = await AddCalendarDayItem("GetAllDay_3", DateTime.Today.AddMonths(30));
        var new5 = await AddCalendarDayItem("GetAllDay_4", DateTime.Today.AddDays(-10));
        var new6 = await AddCalendarDayItem("GetAllDay_5", DateTime.Today.AddDays(-20));
        var new7 = await AddCalendarDayItem("GetAllDay_5", DateTime.Today.AddMonths(-20));
        var from = DateTime.Today.AddDays(-10);
        var till = DateTime.Today.AddDays(20);
        var result = await CalendarItemController.GetDayItems(from.Year, from.Month, from.Day, till.Year, till.Month, till.Day, Guid.Empty);
        Assert.NotNull(result?.Value?.DayItems);
        result.Value.DayItems.Should().Contain(x => x.Id == DefaultCalendarDayItem);
        result.Value.DayItems.Should().Contain(x => x.Id == new1);
        result.Value.DayItems.Should().Contain(x => x.Id == new2);
        result.Value.DayItems.Should().NotContain(x => x.Id == new3);
        result.Value.DayItems.Should().NotContain(x => x.Id == new4);
        result.Value.DayItems.Should().Contain(x => x.Id == new5);
        result.Value.DayItems.Should().NotContain(x => x.Id == new6);
        result.Value.DayItems.Should().NotContain(x => x.Id == new7);
    }

    [Fact]
    public async Task GetAllFutureDayItems()
    {
        var usr = await AddUser("GetAllFutureDayItems");
        var new1 = await AddCalendarDayItem("GetAllFutureDay_1", DateTime.Today);
        var new2 = await AddCalendarDayItem("GetAllFutureDay_2", DateTime.Today.AddDays(20), DefaultSettingsHelper.IdTaco);
        var new3 = await AddCalendarDayItem("GetAllFutureDay_3", DateTime.Today.AddDays(30));
        var new4 = await AddCalendarDayItem("GetAllFutureDay_4", DateTime.Today.AddMonths(30));
        var new5 = await AddCalendarDayItem("GetAllFutureDay_5", DateTime.Today.AddDays(-10), DefaultSettingsHelper.IdTaco);
        var new6 = await AddCalendarDayItem("GetAllFutureDay_6", DateTime.Today.AddDays(-20), usr);
        var new7 = await AddCalendarDayItem("GetAllFutureDay_7", DateTime.Today.AddMonths(-20), DefaultUserId);
        var result = await CalendarItemController.GetAllFutureDayItems(30, 0, true);
        Assert.NotNull(result?.Value?.DayItems);
        result.Value.DayItems.Should().Contain(x => x.Id == DefaultCalendarDayItem);
        result.Value.DayItems.Should().Contain(x => x.Id == new1);
        result.Value.DayItems.Should().Contain(x => x.Id == new2);
        result.Value.DayItems.Should().Contain(x => x.Id == new3);
        result.Value.DayItems.Should().Contain(x => x.Id == new4);
        result.Value.DayItems.Should().NotContain(x => x.Id == new5);
        result.Value.DayItems.Should().NotContain(x => x.Id == new6);
        result.Value.DayItems.Should().NotContain(x => x.Id == new7);
    }

    [Fact]
    public async Task GetAllFutureDayItemsForUser()
    {
        var usr = await AddUser("GetAllFutureDayItemsForUser");
        var new1 = await AddCalendarDayItem("GetAllFutureDayForUser_1", DateTime.Today);
        var new2 = await AddCalendarDayItem("GetAllFutureDayForUser_2", DateTime.Today.AddDays(20), DefaultSettingsHelper.IdTaco);
        var new3 = await AddCalendarDayItem("GetAllFutureDayForUser_3", DateTime.Today.AddDays(30));
        var new4 = await AddCalendarDayItem("GetAllFutureDayForUser_4", DateTime.Today.AddMonths(30));
        var new5 = await AddCalendarDayItem("GetAllFutureDayForUser_5", DateTime.Today.AddDays(-10), DefaultSettingsHelper.IdTaco);
        var new6 = await AddCalendarDayItem("GetAllFutureDayForUser_6", DateTime.Today.AddDays(-20), usr);
        var new7 = await AddCalendarDayItem("GetAllFutureDayForUser_7", DateTime.Today.AddMonths(-20), DefaultUserId);
        var result = await CalendarItemController.GetAllFutureDayItems(30, 0, false);
        Assert.NotNull(result?.Value?.DayItems);
        result.Value.DayItems.Should().NotContain(x => x.Id == DefaultCalendarDayItem);
        result.Value.DayItems.Should().NotContain(x => x.Id == new1);
        result.Value.DayItems.Should().Contain(x => x.Id == new2);
        result.Value.DayItems.Should().NotContain(x => x.Id == new3);
        result.Value.DayItems.Should().NotContain(x => x.Id == new4);
        result.Value.DayItems.Should().NotContain(x => x.Id == new5);
        result.Value.DayItems.Should().NotContain(x => x.Id == new6);
        result.Value.DayItems.Should().NotContain(x => x.Id == new7);
    }

    [Fact]
    public async Task GetDayItemDashboard()
    {
        var new1 = await AddCalendarDayItem("GetDayItemDashboard_1", DateTime.Today.AddHours(1));
        var new2 = await AddCalendarDayItem("GetDayItemDashboard_2", DateTime.Today.AddHours(-1));
        var new3 = await AddCalendarDayItem("GetDayItemDashboard_3", DateTime.Today.AddDays(8));
        var new4 = await AddCalendarDayItem("GetDayItemDashboard_4", DateTime.Today.AddDays(20), DefaultSettingsHelper.IdTaco);
        var new5 = await AddCalendarDayItem("GetDayItemDashboard_5", DateTime.Today.AddDays(-10), DefaultSettingsHelper.IdTaco);
        var new6 = await AddCalendarDayItem("GetDayItemDashboard_6", DateTime.Today.AddDays(-10), DefaultUserId);
        var result = await CalendarItemController.GetDayItemDashboard();
        Assert.NotNull(result?.Value?.DayItems);
        Assert.NotEmpty(result.Value.DayItems);
        result.Value.DayItems.Should().Contain(x => x.Id == new1);
        result.Value.DayItems.Should().NotContain(x => x.Id == new2);
        result.Value.DayItems.Should().NotContain(x => x.Id == new3);
        result.Value.DayItems.Should().Contain(x => x.Id == new4);
        result.Value.DayItems.Should().NotContain(x => x.Id == new5);
        result.Value.DayItems.Should().NotContain(x => x.Id == new6);
    }

    [Fact]
    public async Task DeleteDayItemTest()
    {
        var result = await CalendarItemController.GetDayItemById(DefaultCalendarDayItem);
        Assert.NotNull(result.Value?.DayItem?.Id);
        result.Value.DayItem.Text.Should().Be(TRAINING_CALENDAR_DAY);
        var deleteRes = await CalendarItemController.DeleteDayItem(DefaultCalendarDayItem);
        Assert.True(deleteRes?.Value);
        result = await CalendarItemController.GetDayItemById(DefaultCalendarDayItem);
        Assert.Null(result.Value?.DayItem);
        Assert.False(result.Value!.Success);
    }
}
