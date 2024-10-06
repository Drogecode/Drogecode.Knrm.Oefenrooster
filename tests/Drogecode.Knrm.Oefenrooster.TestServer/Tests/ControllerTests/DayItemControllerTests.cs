using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class DayItemControllerTests : BaseTest
{
    public DayItemControllerTests(
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
        ReportTrainingController reportTrainingController,
        UserRoleController userRoleController,
        UserLinkedMailsController userLinkedMailsController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, dayItemController, monthItemController,
            preComController, vehicleController, defaultScheduleController, reportActionController, reportTrainingController, userRoleController, userLinkedMailsController)
    {
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
        var result = await DayItemController.PutDayItem(body);
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
    }

    [Fact]
    public async Task GetDayItem()
    {
        var result = await DayItemController.ById(DefaultCalendarDayItem);
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
        Assert.NotNull(result.Value.DayItem?.Id);
        result.Value.DayItem.Text.Should().Be(TRAINING_CALENDAR_DAY);
    }

    [Fact]
    public async Task PatchDayItem()
    {
        var patchedText = "PatchDayItem";

        var result = await DayItemController.ById(DefaultCalendarDayItem);
        var oldDayItem = result.Value?.DayItem;
        Assert.NotNull(oldDayItem);
        oldDayItem.Text.Should().NotBe(patchedText);
        oldDayItem.Text = patchedText;

        var patched = await DayItemController.PatchDayItem(oldDayItem);
        Assert.NotNull(patched.Value);
        Assert.True(patched.Value.Success);

        var getAfterPatch = await DayItemController.ById(DefaultCalendarDayItem);
        Assert.NotNull(getAfterPatch.Value?.DayItem);
        getAfterPatch.Value.DayItem.Text.Should().Be(patchedText);
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
        var result = await DayItemController.GetItems(from.Year, from.Month, from.Day, till.Year, till.Month, till.Day, Guid.Empty);
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
        var new2 = await AddCalendarDayItem("GetAllFutureDay_2", DateTime.Today.AddDays(20), null, DefaultSettingsHelper.IdTaco);
        var new3 = await AddCalendarDayItem("GetAllFutureDay_3", DateTime.Today.AddDays(30));
        var new4 = await AddCalendarDayItem("GetAllFutureDay_4", DateTime.Today.AddMonths(30));
        var new5 = await AddCalendarDayItem("GetAllFutureDay_5", DateTime.Today.AddDays(-10), null, DefaultSettingsHelper.IdTaco);
        var new6 = await AddCalendarDayItem("GetAllFutureDay_6", DateTime.Today.AddDays(-20), null, usr);
        var new7 = await AddCalendarDayItem("GetAllFutureDay_7", DateTime.Today.AddMonths(-20), null, DefaultUserId);
        var result = await DayItemController.GetAllFuture(30, 0, true);
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
        var new2 = await AddCalendarDayItem("GetAllFutureDayForUser_2", DateTime.Today.AddDays(20), null, DefaultSettingsHelper.IdTaco);
        var new3 = await AddCalendarDayItem("GetAllFutureDayForUser_3", DateTime.Today.AddDays(30));
        var new4 = await AddCalendarDayItem("GetAllFutureDayForUser_4", DateTime.Today.AddMonths(30));
        var new5 = await AddCalendarDayItem("GetAllFutureDayForUser_5", DateTime.Today.AddDays(-10), null, DefaultSettingsHelper.IdTaco);
        var new6 = await AddCalendarDayItem("GetAllFutureDayForUser_6", DateTime.Today.AddDays(-20), null, usr);
        var new7 = await AddCalendarDayItem("GetAllFutureDayForUser_7", DateTime.Today.AddMonths(-20), null, DefaultUserId);
        var result = await DayItemController.GetAllFuture(30, 0, false);
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
        var new4 = await AddCalendarDayItem("GetDayItemDashboard_4", DateTime.Today.AddDays(26));
        var new5 = await AddCalendarDayItem("GetDayItemDashboard_5", DateTime.Today.AddDays(20), null, DefaultSettingsHelper.IdTaco);
        var new6 = await AddCalendarDayItem("GetDayItemDashboard_6", DateTime.Today.AddDays(-10), null, DefaultSettingsHelper.IdTaco);
        var new7 = await AddCalendarDayItem("GetDayItemDashboard_7", DateTime.Today.AddDays(-10), null, DefaultUserId);
        var new8 = await AddCalendarDayItem("GetDayItemDashboard_8", DateTime.Today.AddDays(-10), DateTime.Today.AddDays(10), DefaultSettingsHelper.IdTaco);
        var new9 = await AddCalendarDayItem("GetDayItemDashboard_9", DateTime.Today.AddDays(-10), DateTime.Today, DefaultSettingsHelper.IdTaco);
        var result = await DayItemController.GetDashboard();
        Assert.NotNull(result?.Value?.DayItems);
        Assert.NotEmpty(result.Value.DayItems);
        result.Value.DayItems.Should().Contain(x => x.Id == new1);
        result.Value.DayItems.Should().NotContain(x => x.Id == new2);
        result.Value.DayItems.Should().Contain(x => x.Id == new3);
        result.Value.DayItems.Should().NotContain(x => x.Id == new4);
        result.Value.DayItems.Should().Contain(x => x.Id == new5);
        result.Value.DayItems.Should().NotContain(x => x.Id == new6);
        result.Value.DayItems.Should().NotContain(x => x.Id == new7);
        result.Value.DayItems.Should().Contain(x => x.Id == new8);
        result.Value.DayItems.Should().Contain(x => x.Id == new9);
    }

    [Fact]
    public async Task DeleteDayItemTest()
    {
        var result = await DayItemController.ById(DefaultCalendarDayItem);
        Assert.NotNull(result.Value?.DayItem?.Id);
        result.Value.DayItem.Text.Should().Be(TRAINING_CALENDAR_DAY);
        var deleteRes = await DayItemController.DeleteDayItem(DefaultCalendarDayItem);
        Assert.True(deleteRes?.Value);
        result = await DayItemController.ById(DefaultCalendarDayItem);
        Assert.Null(result.Value?.DayItem);
        Assert.False(result.Value!.Success);
    }
}
