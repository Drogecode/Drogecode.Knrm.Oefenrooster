using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class DayItemControllerTests: BaseTest
{
    public DayItemControllerTests(TestService testService) : base(testService)
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
        var result = await Tester.DayItemController.PutDayItem(body);
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
    }

    [Fact]
    public async Task GetDayItem()
    {
        var result = await Tester.DayItemController.ById(Tester.DefaultCalendarDayItem);
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
        Assert.NotNull(result.Value.DayItem?.Id);
        result.Value.DayItem.Text.Should().Be(TestService.TRAINING_CALENDAR_DAY);
    }

    [Fact]
    public async Task PatchDayItem()
    {
        var patchedText = "PatchDayItem";

        var result = await Tester.DayItemController.ById(Tester.DefaultCalendarDayItem);
        var oldDayItem = result.Value?.DayItem;
        Assert.NotNull(oldDayItem);
        oldDayItem.Text.Should().NotBe(patchedText);
        oldDayItem.Text = patchedText;

        var patched = await Tester.DayItemController.PatchDayItem(oldDayItem);
        Assert.NotNull(patched.Value);
        Assert.True(patched.Value.Success);

        var getAfterPatch = await Tester.DayItemController.ById(Tester.DefaultCalendarDayItem);
        Assert.NotNull(getAfterPatch.Value?.DayItem);
        getAfterPatch.Value.DayItem.Text.Should().Be(patchedText);
    }

    [Fact]
    public async Task GetAllDay()
    {
        var new1 = await Tester.AddCalendarDayItem("GetAllDay_1", DateTime.Today);
        var new2 = await Tester.AddCalendarDayItem("GetAllDay_2", DateTime.Today.AddDays(20));
        var new3 = await Tester.AddCalendarDayItem("GetAllDay_3", DateTime.Today.AddDays(30));
        var new4 = await Tester.AddCalendarDayItem("GetAllDay_3", DateTime.Today.AddMonths(30));
        var new5 = await Tester.AddCalendarDayItem("GetAllDay_4", DateTime.Today.AddDays(-10));
        var new6 = await Tester.AddCalendarDayItem("GetAllDay_5", DateTime.Today.AddDays(-20));
        var new7 = await Tester.AddCalendarDayItem("GetAllDay_5", DateTime.Today.AddMonths(-20));
        var from = DateTime.Today.AddDays(-10);
        var till = DateTime.Today.AddDays(20);
        var result = await Tester.DayItemController.GetItems(from.Year, from.Month, from.Day, till.Year, till.Month, till.Day, Guid.Empty);
        Assert.NotNull(result?.Value?.DayItems);
        result.Value.DayItems.Should().Contain(x => x.Id == Tester.DefaultCalendarDayItem);
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
        var usr = await Tester.AddUser("GetAllFutureDayItems");
        var new1 = await Tester.AddCalendarDayItem("GetAllFutureDay_1", DateTime.Today);
        var new2 = await Tester.AddCalendarDayItem("GetAllFutureDay_2", DateTime.Today.AddDays(20), null, DefaultSettingsHelperMock.IdTaco);
        var new3 = await Tester.AddCalendarDayItem("GetAllFutureDay_3", DateTime.Today.AddDays(30));
        var new4 = await Tester.AddCalendarDayItem("GetAllFutureDay_4", DateTime.Today.AddMonths(30));
        var new5 = await Tester.AddCalendarDayItem("GetAllFutureDay_5", DateTime.Today.AddDays(-10), null, DefaultSettingsHelperMock.IdTaco);
        var new6 = await Tester.AddCalendarDayItem("GetAllFutureDay_6", DateTime.Today.AddDays(-20), null, usr);
        var new7 = await Tester.AddCalendarDayItem("GetAllFutureDay_7", DateTime.Today.AddMonths(-20), null, Tester.DefaultUserId);
        var result = await Tester.DayItemController.GetAllFuture(30, 0, true);
        Assert.NotNull(result?.Value?.DayItems);
        result.Value.DayItems.Should().Contain(x => x.Id == Tester.DefaultCalendarDayItem);
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
        var usr = await Tester.AddUser("GetAllFutureDayItemsForUser");
        var new1 = await Tester.AddCalendarDayItem("GetAllFutureDayForUser_1", DateTime.Today);
        var new2 = await Tester.AddCalendarDayItem("GetAllFutureDayForUser_2", DateTime.Today.AddDays(20), null, DefaultSettingsHelperMock.IdTaco);
        var new3 = await Tester.AddCalendarDayItem("GetAllFutureDayForUser_3", DateTime.Today.AddDays(30));
        var new4 = await Tester.AddCalendarDayItem("GetAllFutureDayForUser_4", DateTime.Today.AddMonths(30));
        var new5 = await Tester.AddCalendarDayItem("GetAllFutureDayForUser_5", DateTime.Today.AddDays(-10), null, DefaultSettingsHelperMock.IdTaco);
        var new6 = await Tester.AddCalendarDayItem("GetAllFutureDayForUser_6", DateTime.Today.AddDays(-20), null, usr);
        var new7 = await Tester.AddCalendarDayItem("GetAllFutureDayForUser_7", DateTime.Today.AddMonths(-20), null, Tester.DefaultUserId);
        var result = await Tester.DayItemController.GetAllFuture(30, 0, false);
        Assert.NotNull(result?.Value?.DayItems);
        result.Value.DayItems.Should().NotContain(x => x.Id == Tester.DefaultCalendarDayItem);
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
        var new1 = await Tester.AddCalendarDayItem("GetDayItemDashboard_1", DateTime.Today.AddHours(1));
        var new2 = await Tester.AddCalendarDayItem("GetDayItemDashboard_2", DateTime.Today.AddHours(-1));
        var new3 = await Tester.AddCalendarDayItem("GetDayItemDashboard_3", DateTime.Today.AddDays(8));
        var new4 = await Tester.AddCalendarDayItem("GetDayItemDashboard_4", DateTime.Today.AddDays(26));
        var new5 = await Tester.AddCalendarDayItem("GetDayItemDashboard_5", DateTime.Today.AddDays(20), null, DefaultSettingsHelperMock.IdTaco);
        var new6 = await Tester.AddCalendarDayItem("GetDayItemDashboard_6", DateTime.Today.AddDays(-10), null, DefaultSettingsHelperMock.IdTaco);
        var new7 = await Tester.AddCalendarDayItem("GetDayItemDashboard_7", DateTime.Today.AddDays(-10), null, Tester.DefaultUserId);
        var new8 = await Tester.AddCalendarDayItem("GetDayItemDashboard_8", DateTime.Today.AddDays(-10), DateTime.Today.AddDays(10), DefaultSettingsHelperMock.IdTaco);
        var new9 = await Tester.AddCalendarDayItem("GetDayItemDashboard_9", DateTime.Today.AddDays(-10), DateTime.Today, DefaultSettingsHelperMock.IdTaco);
        var result = await Tester.DayItemController.GetDashboard();
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
        var result = await Tester.DayItemController.ById(Tester.DefaultCalendarDayItem);
        Assert.NotNull(result.Value?.DayItem?.Id);
        result.Value.DayItem.Text.Should().Be(TestService.TRAINING_CALENDAR_DAY);
        var deleteRes = await Tester.DayItemController.DeleteDayItem(Tester.DefaultCalendarDayItem);
        Assert.True(deleteRes?.Value);
        result = await Tester.DayItemController.ById(Tester.DefaultCalendarDayItem);
        Assert.Null(result.Value?.DayItem);
        Assert.False(result.Value!.Success);
    }
}
