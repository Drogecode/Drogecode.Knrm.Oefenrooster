using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class UserControllerTests : BaseTest
{
    public UserControllerTests(TestService testService) : base(testService)
    {
    }

    [Fact]
    public async Task AddUserTest()
    {
        var result = await Tester.UserController.AddUser(new DrogeUser
        {
            Name = "AddUserTest"
        });
        Assert.NotNull(result?.Value?.UserId);
        Assert.True(result.Value.Success);
    }

    [Fact]
    public async Task GetAllTest()
    {
        var newUser = await Tester.AddUser("GetAllTest");
        var users = await Tester.UserController.GetAll(true);
        Assert.NotNull(users?.Value?.DrogeUsers?.Count);
        Assert.True(users.Value.Success);
        users.Value.DrogeUsers.Should().Contain(x => x.Id == Tester.DefaultUserId);
        users.Value.DrogeUsers.Should().Contain(x => x.Id == newUser);
    }

    [Fact]
    public async Task GetUserByIdTest()
    {
        var user = await Tester.UserController.GetById(Tester.DefaultUserId);
        Assert.True(user.Value?.Success);
        Assert.NotNull(user.Value!.User);
        user.Value.User.Id.Should().Be(Tester.DefaultUserId);
        user.Value.User.Name.Should().Be(TestService.USER_NAME);
        user.Value.User.LinkedAsA.Should().BeNull();
        user.Value.User.LinkedAsB.Should().BeNull();
    }

    [Fact]
    public async Task LinkUserUserTest()
    {
        var userAId = await Tester.AddUser("User A");
        var userBId = await Tester.AddUser("User B");
        var userA = (await Tester.UserController.GetById(userAId))!.Value!.User;
        var userB = (await Tester.UserController.GetById(userBId))!.Value!.User;
        Assert.Null(userA?.LinkedAsA);
        Assert.Null(userB?.LinkedAsB);
        var body = new UpdateLinkUserUserForUserRequest
        {
            UserAId = userAId,
            UserBId = userBId,
            LinkType = Shared.Enums.UserUserLinkType.Buddy,
            Add = true
        };
        var linkUser = await Tester.UserController.UpdateLinkUserUserForUser(body);
        Assert.True(linkUser.Value?.Success);
        userA = (await Tester.UserController.GetById(userAId))!.Value!.User;
        userB = (await Tester.UserController.GetById(userBId))!.Value!.User;
        userA!.LinkedAsA.Should().Contain(x => x.LinkedUserId == userBId);
        userB!.LinkedAsB.Should().Contain(x => x.LinkedUserId == userAId);
    }

    [Fact(Skip = "Fails because of an in memory database bug")]
    public async Task UnLinkUserUserTest()
    {
        var userAId = await Tester.AddUser("User A");
        var userBId = await Tester.AddUser("User B");
        var body = new UpdateLinkUserUserForUserRequest
        {
            UserAId = userAId,
            UserBId = userBId,
            LinkType = Shared.Enums.UserUserLinkType.Buddy,
            Add = true
        };
        var linkUser = await Tester.UserController.UpdateLinkUserUserForUser(body);
        Assert.True(linkUser.Value?.Success);
        var userA = (await Tester.UserController.GetById(userAId))!.Value!.User;
        var userB = (await Tester.UserController.GetById(userBId))!.Value!.User;
        userA!.LinkedAsA.Should().Contain(x => x.LinkedUserId == userBId);
        userB!.LinkedAsB.Should().Contain(x => x.LinkedUserId == userAId);
        body.Add = false;
        var unLinkUser = await Tester.UserController.UpdateLinkUserUserForUser(body);
        Assert.True(unLinkUser.Value?.Success);
        var userAa = (await Tester.UserController.GetById(userAId))!.Value!.User;
        var userBb = (await Tester.UserController.GetById(userBId))!.Value!.User;
        userAa!.LinkedAsA.Should().NotContain(x => x.LinkedUserId == userBId);
        userBb!.LinkedAsB.Should().NotContain(x => x.LinkedUserId == userAId);

    }

    [Fact]
    public async Task GetTest()
    {
        var user = await Tester.UserController.GetCurrentUser();
        Assert.NotNull(user?.Value?.DrogeUser);
        user.Value.DrogeUser.Id.Should().Be(DefaultSettingsHelperMock.IdTaco);
    }
}
