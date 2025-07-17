using Drogecode.Knrm.Oefenrooster.Shared.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;
using Drogecode.Knrm.Oefenrooster.SharedForTests.Helpers;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class UserRoleControllerTests : BaseTest
{
    public UserRoleControllerTests(TestService testService) : base(testService)
    {
    }

    [Fact]
    public async Task NewUserRoleTest()
    {
        var body = new DrogeUserRole { Name = "NewUserRoleTest" };
        var postResponse = await Tester.UserRoleController.NewUserRole(body);
        Assert.NotNull(postResponse?.Value?.NewId);
        postResponse.Value.Success.Should().BeTrue();
        postResponse.Value.NewId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetAllTest()
    {
        var getResponse = await Tester.UserRoleController.GetAll();
        Assert.NotNull(getResponse?.Value?.Roles);
        Assert.NotEmpty(getResponse.Value.Roles);
        getResponse.Value.Roles.Should().HaveCount(1);
        var role = getResponse.Value.Roles.FirstOrDefault();
        Assert.NotNull(role);
        role.Name.Should().Be(TestService.USER_ROLE_NAME);
        role.AUTH_scheduler_other.Should().BeTrue();
        role.AUTH_scheduler_dayitem.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdTest()
    {
        var getResponse = await Tester.UserRoleController.GetById(Tester.DefaultUserRoleId);
        Assert.NotNull(getResponse?.Value?.Role);
        getResponse.Value.Success.Should().BeTrue();
        getResponse.Value.Role.Name.Should().Be(TestService.USER_ROLE_NAME);
    }

    [Fact]
    public async Task GetLinkedUsersByIdTest()
    {
        var getResponse = await Tester.UserRoleController.GetLinkedUsersById(Tester.DefaultUserRoleId);
        Assert.NotNull(getResponse?.Value?.LinkedUsers);
        getResponse.Value.Success.Should().BeTrue();
        getResponse.Value.LinkedUsers.Should().BeEmpty();
    }

    [Fact]
    public async Task PatchUserRoleTest()
    {
        var getResponse = await Tester.UserRoleController.GetById(Tester.DefaultUserRoleId);
        Assert.NotNull(getResponse?.Value?.Role);
        Assert.False(getResponse.Value.Role.AUTH_dashboard_holidays);
        getResponse.Value.Role.AUTH_dashboard_holidays = true;
        var patchResponse = await Tester.UserRoleController.PatchUserRole(getResponse.Value.Role);
        Assert.NotNull(patchResponse?.Value?.Success);
        Assert.True(patchResponse.Value.Success);
        var getResponse2 = await Tester.UserRoleController.GetById(Tester.DefaultUserRoleId);
        Assert.NotNull(getResponse2?.Value?.Role);
        Assert.True(getResponse2.Value.Role.AUTH_dashboard_holidays);
    }

    [Fact]
    public async Task LinkUserToRoleAsync()
    {
        var allRoles = await Tester.UserRoleController.GetAllBasic();
        var rolesForUser = await Tester.UserController.GetRolesForUserById(Tester.DefaultUserId);
        Assert.NotNull(allRoles?.Value?.Roles);
        Assert.Null(rolesForUser?.Value?.Roles);
        var role = allRoles.Value.Roles.FirstOrDefault()?.ToDrogeUserRoleLinked();
        Assert.NotNull(role);
        role.IsSet = true;
        role.SetExternal = false;
        var linkResult = await Tester.UserRoleController.LinkUserToRoleAsync(role, Tester.DefaultUserId);
        Assert.NotNull(linkResult?.Value?.Success);
        Assert.True(linkResult.Value.Success);
        rolesForUser = await Tester.UserController.GetRolesForUserById(Tester.DefaultUserId);
        Assert.NotNull(rolesForUser?.Value?.Roles);
        rolesForUser.Value.Roles.Should().Contain(x => x.Id == role.Id);
    }
}