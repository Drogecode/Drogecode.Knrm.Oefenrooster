using System.Text.Json;
using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class ReportTrainingControllerTests : BaseTest
{
    public ReportTrainingControllerTests(
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
        UserRoleController userRoleController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, dayItemController, monthItemController,
            preComController, vehicleController, defaultScheduleController, reportActionController, reportTrainingController, userRoleController)
    {
    }

    [Fact]
    public async Task GetReportActionsCurrentUserTest()
    {
        var getResult = await ReportTrainingController.GetLastTrainingsForCurrentUser(10, 0);
        Assert.NotNull(getResult.Value?.Trainings);
        Assert.NotEmpty(getResult.Value.Trainings);
        Assert.True(getResult.Value.Success);
        getResult.Value.Trainings.Count.Should().BeGreaterOrEqualTo(2);
        var prevStart = DateTime.MaxValue;
        foreach (var action in getResult.Value.Trainings)
        {
            action.Start.Should().BeBefore(prevStart);
            prevStart = action.Start;
        }
    }

    [Fact]
    public async Task GetReportActionsAllUsersTest()
    {
        var emptyList = JsonSerializer.Serialize(new List<Guid>());
        var getResult = await ReportTrainingController.GetLastTrainings(emptyList, 10, 0);
        Assert.NotNull(getResult.Value?.Trainings);
        Assert.NotEmpty(getResult.Value.Trainings);
        Assert.True(getResult.Value.Success);
        getResult.Value.Trainings.Count.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task GetReportActionsUnknownUserTest()
    {
        var unknownUser = JsonSerializer.Serialize(new List<Guid> { Guid.NewGuid() });
        var getResult = await ReportTrainingController.GetLastTrainings(unknownUser, 10, 0);
        Assert.NotNull(getResult.Value?.Trainings);
        Assert.True(getResult.Value.Success);
        getResult.Value.Trainings.Should().BeEmpty();
    }
}