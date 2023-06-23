using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class UserControllerTests : BaseTest
{

    public UserControllerTests(
        ScheduleController scheduleController, 
        UserController userController,
        FunctionController functionController,
        HolidayController holidayController) : base(scheduleController, userController, functionController, holidayController)
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
        users.Value.DrogeUsers.Should().Contain(x => x.Id == UserId);
        users.Value.DrogeUsers.Should().Contain(x => x.Id == newUser);
    }

    [Fact]
    public async Task GetTest()
    {
        var user = await UserController.Get();
        Assert.NotNull(user?.Value?.DrogeUser);
        user.Value.DrogeUser.Id.Should().Be(DefaultSettingsHelper.IdTaco);
    }
}
