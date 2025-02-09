namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class AuditControllerTest : BaseTest
{
    public AuditControllerTest(
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
        ReportActionSharedController reportActionSharedController,
        AuditController auditController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, dayItemController, monthItemController,
            preComController, vehicleController, defaultScheduleController, reportActionController, reportTrainingController, userRoleController, userLinkedMailsController,
            reportActionSharedController,
            auditController)
    {
    }

    [Fact]
    public async Task GetAllTrainingsAuditTest()
    {
        var getResponse = await AuditController.GetAllTrainingsAudit(30, 0);
        Assert.NotNull(getResponse?.Value?.TrainingAudits);
        getResponse.Value.Success.Should().BeTrue();
        getResponse.Value.TrainingAudits.Should().NotBeNull();
    }
}