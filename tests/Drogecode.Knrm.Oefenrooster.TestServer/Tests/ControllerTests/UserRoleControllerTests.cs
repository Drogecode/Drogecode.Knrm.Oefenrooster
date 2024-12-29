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
        UserRoleController userRoleController,
        UserLinkedMailsController userLinkedMailsController,
        ReportActionSharedController reportActionSharedController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, dayItemController, monthItemController,
            preComController, vehicleController, defaultScheduleController, reportActionController, reportTrainingController, userRoleController, userLinkedMailsController, reportActionSharedController)
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
        role.AUTH_scheduler_dayitem.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdTest()
    {
        var getResponse = await UserRoleController.GetById(DefaultUserRoleId);
        Assert.NotNull(getResponse?.Value?.Role);
        getResponse.Value.Success.Should().BeTrue();
        getResponse.Value.Role.Name.Should().Be(USER_ROLE_NAME);
    }

    [Fact]
    public async Task GetLinkedUsersByIdTest()
    {
        var getResponse = await UserRoleController.GetLinkedUsersById(DefaultUserRoleId);
        Assert.NotNull(getResponse?.Value?.LinkedUsers);
        getResponse.Value.Success.Should().BeTrue();
        getResponse.Value.LinkedUsers.Should().BeEmpty();
    }

    [Fact]
    public async Task PatchUserRoleTest()
    {
        var getResponse = await UserRoleController.GetById(DefaultUserRoleId);
        Assert.NotNull(getResponse?.Value?.Role);
        Assert.False(getResponse.Value.Role.AUTH_dashboard_holidays);
        getResponse.Value.Role.AUTH_dashboard_holidays = true;
        var patchResponse = await UserRoleController.PatchUserRole(getResponse.Value.Role);
        Assert.NotNull(patchResponse?.Value?.Success);
        Assert.True(patchResponse.Value.Success);
        var getResponse2 = await UserRoleController.GetById(DefaultUserRoleId);
        Assert.NotNull(getResponse2?.Value?.Role);
        Assert.True(getResponse2.Value.Role.AUTH_dashboard_holidays);
    }
}