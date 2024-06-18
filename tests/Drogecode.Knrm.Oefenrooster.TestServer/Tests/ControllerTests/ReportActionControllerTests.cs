using System.Text.Json;
using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

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
        ReportTrainingController reportTrainingController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, dayItemController, monthItemController,
            preComController, vehicleController, defaultScheduleController, reportActionController, reportTrainingController)
    {
    }

    [Fact]
    public async Task GetReportActionsCurrentUserTest()
    {
        var getResult = await ReportActionController.GetLastActionsForCurrentUser(10, 0);
        Assert.NotNull(getResult.Value?.Actions);
        Assert.NotEmpty(getResult.Value.Actions);
        Assert.True(getResult.Value.Success);
        getResult.Value.Actions.Count.Should().BeGreaterOrEqualTo(2);
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
        getResult.Value.Actions.Count.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task GetReportActionsDifferentUsersTest()
    {
        var emptyList = JsonSerializer.Serialize(new List<Guid>{Guid.NewGuid()});
        var getResult = await ReportActionController.GetLastActions(emptyList, 10, 0);
        Assert.NotNull(getResult.Value?.Actions);
        Assert.True(getResult.Value.Success);
        getResult.Value.Actions.Should().BeEmpty();
    }

    [Fact]
    public async Task AnalyzeYearChartsAllTest()
    {
        var getResult = await ReportActionController.AnalyzeYearChartsAll();
        Assert.NotNull(getResult.Value?.Years);
        Assert.True(getResult.Value.Success);
        getResult.Value.Years.Should().HaveCount(1);
        getResult.Value.TotalCount.Should().Be(3);
        var y2022 = getResult.Value.Years.FirstOrDefault(x => x.Year == 2022);
        Assert.NotNull(y2022);
        y2022.Months.Should().Contain(x => x.Month == 3 && x.Count == 2);
        y2022.Months.Should().Contain(x => x.Month == 4 && x.Count == 1);
    }
}