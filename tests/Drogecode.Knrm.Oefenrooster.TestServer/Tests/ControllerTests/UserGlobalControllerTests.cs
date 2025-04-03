using Drogecode.Knrm.Oefenrooster.Shared.Models.UserGlobal;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class UserGlobalControllerTests : BaseTest
{
    public UserGlobalControllerTests(TestService testService) : base(testService)
    {
    }

    [Fact]
    public async Task PutGlobalUserTest()
    {
        var result = await Tester.UserGlobalController.PutGlobalUser(new DrogeUserGlobal
        {
            Name = "xUnit PutGlobalUser",
        });
        Assert.NotNull(result?.Value?.NewId);
        Assert.True(result.Value.Success);
        result.Value.NewId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetGlobalUserByIdTest()
    {
        var result = await Tester.UserGlobalController.GetGlobalUserById(Tester.DefaultGlobalUserId);
        Assert.NotNull(result?.Value?.GlobalUser);
        Assert.True(result.Value.Success);
        result.Value.GlobalUser.Name.Should().Be(TestService.USER_NAME);
    }

    [Fact]
    public async Task PatchGlobalUserTest()
    {
        const string newName = "xUnit PatchGlobalUser";
        var old = (await Tester.UserGlobalController.GetGlobalUserById(Tester.DefaultGlobalUserId))!.Value!.GlobalUser!;
        old.Name = newName;
        var result = await Tester.UserGlobalController.PatchGlobalUser(old);
        Assert.NotNull(result?.Value?.Success);
        Assert.True(result.Value.Success);
        var patched = (await Tester.UserGlobalController.GetGlobalUserById(Tester.DefaultGlobalUserId))!.Value!.GlobalUser!;
        Assert.NotNull(patched);
        patched.Name.Should().Be(newName);
    }
}