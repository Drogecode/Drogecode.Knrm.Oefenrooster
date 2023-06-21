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

}
