using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class UserControllerTests : BaseTest
{
    public UserControllerTests(
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
    public async Task AddUserTest()
    {
        var result = await UserController.AddUser(new DrogeUser
        {
            Name = "AddUserTest"
        });
        Assert.NotNull(result?.Value?.UserId);
        Assert.True(result.Value.Success);
    }

    [Fact]
    public async Task GetAllTest()
    {
        var newUser = await AddUser("GetAllTest");
        var users = await UserController.GetAll(true);
        Assert.NotNull(users?.Value?.DrogeUsers?.Count);
        Assert.True(users.Value.Success);
        users.Value.DrogeUsers.Should().Contain(x => x.Id == DefaultUserId);
        users.Value.DrogeUsers.Should().Contain(x => x.Id == newUser);
    }

    [Fact]
    public async Task GetUserByIdTest()
    {
        var user = await UserController.GetById(DefaultUserId);
        Assert.True(user.Value?.Success);
        Assert.NotNull(user.Value!.User);
        user.Value.User.Id.Should().Be(DefaultUserId);
        user.Value.User.Name.Should().Be(USER_NAME);
        user.Value.User.LinkedAsA.Should().BeNull();
        user.Value.User.LinkedAsB.Should().BeNull();
    }

    [Fact]
    public async Task LinkUserUserTest()
    {
        var userAId = await AddUser("User A");
        var userBId = await AddUser("User B");
        var userA = (await UserController.GetById(userAId))!.Value!.User;
        var userB = (await UserController.GetById(userBId))!.Value!.User;
        Assert.Null(userA?.LinkedAsA);
        Assert.Null(userB?.LinkedAsB);
        var body = new UpdateLinkUserUserForUserRequest
        {
            UserAId = userAId,
            UserBId = userBId,
            LinkType = Shared.Enums.UserUserLinkType.Buddy,
            Add = true
        };
        var linkUser = await UserController.UpdateLinkUserUserForUser(body);
        Assert.True(linkUser.Value?.Success);
        userA = (await UserController.GetById(userAId))!.Value!.User;
        userB = (await UserController.GetById(userBId))!.Value!.User;
        userA!.LinkedAsA.Should().Contain(x => x.LinkedUserId == userBId);
        userB!.LinkedAsB.Should().Contain(x => x.LinkedUserId == userAId);
    }

    [Fact(Skip = "Fails because of an in memory database bug")]
    public async Task UnLinkUserUserTest()
    {
        var userAId = await AddUser("User A");
        var userBId = await AddUser("User B");
        var body = new UpdateLinkUserUserForUserRequest
        {
            UserAId = userAId,
            UserBId = userBId,
            LinkType = Shared.Enums.UserUserLinkType.Buddy,
            Add = true
        };
        var linkUser = await UserController.UpdateLinkUserUserForUser(body);
        Assert.True(linkUser.Value?.Success);
        var userA = (await UserController.GetById(userAId))!.Value!.User;
        var userB = (await UserController.GetById(userBId))!.Value!.User;
        userA!.LinkedAsA.Should().Contain(x => x.LinkedUserId == userBId);
        userB!.LinkedAsB.Should().Contain(x => x.LinkedUserId == userAId);
        body.Add = false;
        var unLinkUser = await UserController.UpdateLinkUserUserForUser(body);
        Assert.True(unLinkUser.Value?.Success);
        var userAa = (await UserController.GetById(userAId))!.Value!.User;
        var userBb = (await UserController.GetById(userBId))!.Value!.User;
        userAa!.LinkedAsA.Should().NotContain(x => x.LinkedUserId == userBId);
        userBb!.LinkedAsB.Should().NotContain(x => x.LinkedUserId == userAId);

    }

    [Fact]
    public async Task GetTest()
    {
        var user = await UserController.GetCurrentUser();
        Assert.NotNull(user?.Value?.DrogeUser);
        user.Value.DrogeUser.Id.Should().Be(DefaultSettingsHelper.IdTaco);
    }
}
