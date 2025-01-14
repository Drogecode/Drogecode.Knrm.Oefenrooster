using System.Text.Json;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.TestServer.Seeds;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class ReportActionControllerTests : BaseTest
{
    public ReportActionControllerTests(
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
        UserLinkedMailsController userLinkedMailsController,
        ReportActionSharedController reportActionSharedController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, dayItemController, monthItemController,
            preComController, vehicleController, defaultScheduleController, reportActionController, reportTrainingController, userRoleController, userLinkedMailsController, reportActionSharedController)
    {
        SeedReportAction.Seed(dataContext, DefaultCustomerId);
    }

    [Fact]
    public async Task GetReportActionsCurrentUserTest()
    {
        var getResult = await ReportActionController.GetLastActionsForCurrentUser(10, 0);
        Assert.NotNull(getResult.Value?.Actions);
        Assert.NotEmpty(getResult.Value.Actions);
        Assert.True(getResult.Value.Success);
        getResult.Value.Actions.Count.Should().BeGreaterThanOrEqualTo(2);
        var prevStart = DateTime.MaxValue;
        foreach (var action in getResult.Value.Actions)
        {
            action.Start.Should().BeBefore(prevStart);
            prevStart = action.Start;
        }
    }

    [Fact]
    public async Task GetReportActionsAllUsersTest()
    {
        var emptyList = JsonSerializer.Serialize(new List<Guid>());
        var getResult = await ReportActionController.GetLastActions(emptyList, 10, 0);
        Assert.NotNull(getResult.Value?.Actions);
        Assert.NotEmpty(getResult.Value.Actions);
        Assert.True(getResult.Value.Success);
        getResult.Value.Actions.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetReportActionsUnknownUserTest()
    {
        var unknownUser = JsonSerializer.Serialize(new List<Guid> { Guid.NewGuid() });
        var getResult = await ReportActionController.GetLastActions(unknownUser, 10, 0);
        Assert.NotNull(getResult.Value?.Actions);
        Assert.True(getResult.Value.Success);
        getResult.Value.Actions.Should().BeEmpty();
    }

    [Fact]
    public async Task AnalyzeYearChartsAllForAllUsersTest()
    {
        var request = new AnalyzeActionRequest()
        {
            Users = new List<Guid?> {  },
        };
        var getResult = await ReportActionController.AnalyzeYearChartsAll(request);
        Assert.NotNull(getResult.Value?.Years);
        Assert.True(getResult.Value.Success);
        getResult.Value.Years.Should().HaveCount(7);
        getResult.Value.TotalCount.Should().Be(7*3);
        var y2022 = getResult.Value.Years.FirstOrDefault(x => x.Year == 2022);
        Assert.NotNull(y2022);
        y2022.Months.Should().Contain(x => x.Month == 3 && x.Count == 2);
        y2022.Months.Should().Contain(x => x.Month == 4 && x.Count == 1);
    }

    [Fact]
    public async Task AnalyzeYearChartsAllForAllUsers3YearsTest()
    {
        var request = new AnalyzeActionRequest()
        {
            Users = new List<Guid?> {  },
            Years = 3
        };
        var getResult = await ReportActionController.AnalyzeYearChartsAll(request);
        Assert.NotNull(getResult.Value?.Years);
        Assert.True(getResult.Value.Success);
        getResult.Value.Years.Should().HaveCount(3);
        getResult.Value.TotalCount.Should().Be(3*3);
        getResult.Value.Years.Should().Contain(x => x.Year == 2022);
        getResult.Value.Years.Should().Contain(x => x.Year == 2020);
        getResult.Value.Years.Should().Contain(x => x.Year == 2019);
        getResult.Value.Years.Should().NotContain(x => x.Year == 2018);
    }

    [Fact]
    public async Task AnalyzeYearChartsAllForTacoTest()
    {
        var request = new AnalyzeActionRequest()
        {
            Users = new List<Guid?> { DefaultSettingsHelperMock.IdTaco },
        };
        var getResult = await ReportActionController.AnalyzeYearChartsAll(request);
        Assert.NotNull(getResult.Value?.Years);
        Assert.True(getResult.Value.Success);
        getResult.Value.Years.Should().HaveCount(7);
        getResult.Value.TotalCount.Should().Be(7*3);
        var y2022 = getResult.Value.Years.FirstOrDefault(x => x.Year == 2022);
        Assert.NotNull(y2022);
        y2022.Months.Should().Contain(x => x.Month == 3 && x.Count == 2);
        y2022.Months.Should().Contain(x => x.Month == 4 && x.Count == 1);
    }

    [Fact]
    public async Task AnalyzeYearChartsAllForUnknownUserTest()
    {
        var request = new AnalyzeActionRequest()
        {
            Users = new List<Guid?> { Guid.NewGuid() },
        };
        var getResult = await ReportActionController.AnalyzeYearChartsAll(request);
        Assert.NotNull(getResult.Value?.Years);
        Assert.True(getResult.Value.Success);
        getResult.Value.Years.Should().HaveCount(0);
        getResult.Value.TotalCount.Should().Be(0);
        var y2022 = getResult.Value.Years.FirstOrDefault(x => x.Year == 2022);
        Assert.Null(y2022);
    }

    [Fact]
    public async Task AnalyzeYearChartsAllTacoAndOtherUserTest()
    {
        var start = new DateTime(2022, 4, 8, 8, 5, 41);
        var otherUser = Guid.NewGuid();
        DataContext.ReportActions.Add(new DbReportAction
        {
            Id = Guid.NewGuid(),
            CustomerId = DefaultCustomerId,
            Description = "xUnit AnalyzeYearChartsAllTacoAndOtherUserTest",
            Start = start,
            Commencement = start.AddMinutes(5),
            Departure = start.AddMinutes(15),
            End = start.AddMinutes(121),
            Boat = "xUnit boat",
            Prio = "Prio 1",
            Users = new List<DbReportUser> { new() { DrogeCodeId = DefaultSettingsHelperMock.IdTaco }, new() { DrogeCodeId = otherUser } },
        });
        await DataContext.SaveChangesAsync();
        var request = new AnalyzeActionRequest()
        {
            Users = new List<Guid?> { otherUser, DefaultSettingsHelperMock.IdTaco },
        };
        var getResult = await ReportActionController.AnalyzeYearChartsAll(request);
        Assert.NotNull(getResult.Value?.Years);
        Assert.True(getResult.Value.Success);
        getResult.Value.Years.Should().HaveCount(1);
        getResult.Value.TotalCount.Should().Be(1);
        var y2022 = getResult.Value.Years.FirstOrDefault(x => x.Year == 2022);
        Assert.NotNull(y2022);
        y2022.Months.Should().Contain(x => x.Month == 4 && x.Count == 1);
    }

    [Fact]
    public async Task GetDistinctNoneTest()
    {
        var prioResponse = await ReportActionController.Distinct(DistinctReport.None);
        Assert.Null(prioResponse.Value?.Values);
        Assert.NotNull(prioResponse.Value?.Message);
        prioResponse.Value.Message.Should().Be("None is not valid");
    }

    [Fact]
    public async Task GetDistinctPrioTest()
    {
        var prioResponse = await ReportActionController.Distinct(DistinctReport.Prio);
        Assert.NotNull(prioResponse.Value?.Values);
        Assert.NotEmpty(prioResponse.Value.Values);
        prioResponse.Value.Values.Should().HaveCount(2);
        prioResponse.Value.Values.Where(x => x.Equals("Prio 1")).Should().HaveCount(1);
    }
}