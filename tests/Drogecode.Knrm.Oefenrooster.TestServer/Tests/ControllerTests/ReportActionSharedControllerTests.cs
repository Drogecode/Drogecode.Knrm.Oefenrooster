using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportActionShared;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class ReportActionSharedControllerTests : BaseTest
{
    public ReportActionSharedControllerTests(
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
    }

    [Fact]
    public async Task PutReportActionSharedTest()
    {
        var body = new ReportActionSharedConfiguration
        {
            SelectedUsers = [DefaultSettingsHelperMock.IdTaco],
            Types = ["xUnit","MoreTests"],
            Search = ["unhealthy"],
            StartDate = DateTime.UtcNow.AddDays(-50),
            EndDate = DateTime.UtcNow.AddDays(-5),
            ValidUntil = DateTime.UtcNow.AddDays(100),
        };
        var result = await ReportActionSharedController.PutReportActionShared(body);
        Assert.NotNull(result.Value?.NewId);
        result.Value.Success.Should().BeTrue();
    }
}