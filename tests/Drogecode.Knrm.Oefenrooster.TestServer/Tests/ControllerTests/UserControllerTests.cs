using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class UserControllerTests : BaseTest
{
    private readonly UserController _userController;

    public UserControllerTests(UserController userController) : base(userController)
    {
        _userController = userController;
    }

    [Fact]
    public async Task GetAllTest()
    {
        var users = await _userController.GetAll(true);
        Assert.NotNull(users?.Value?.DrogeUsers?.Count);
        Assert.True(users.Value.Success);
        users.Value.DrogeUsers.Should().Contain(x => x.Id == UserId);
        var newUser = await AddUser("GetAllTest"); 
        var users2 = await _userController.GetAll(true);
        Assert.NotNull(users2?.Value?.DrogeUsers?.Count);
        Assert.True(users2.Value.Success);
        users2.Value.DrogeUsers.Should().Contain(x => x.Id == UserId);
        users2.Value.DrogeUsers.Should().Contain(x=>x.Id == newUser);
    }
}
