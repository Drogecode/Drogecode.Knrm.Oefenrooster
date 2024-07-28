using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class UserRoleControllerTests : BaseTest
{
    public UserRoleControllerTests(
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
    public async Task NewUserRoleTest()
    {
        var body = new DrogeUserRole { Name = "NewUserRoleTest"};
        var postResponse = await UserRoleController.NewUserRole(body);
        Assert.NotNull(postResponse?.Value?.NewId);
        postResponse.Value.Success.Should().BeTrue();
        postResponse.Value.NewId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetAllTest()
    {
        var getResponse = await UserRoleController.GetAll();
        Assert.NotNull(getResponse?.Value?.Roles);
        Assert.NotEmpty(getResponse.Value.Roles);
        getResponse.Value.Roles.Should().HaveCount(1);
        var role = getResponse.Value.Roles.FirstOrDefault();
        Assert.NotNull(role);
        role.AUTH_scheduler_other_user.Should().BeTrue();
    }
}