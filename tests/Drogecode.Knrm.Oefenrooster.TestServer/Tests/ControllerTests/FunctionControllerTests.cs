using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class FunctionControllerTests : BaseTest
{
    public FunctionControllerTests(
        ScheduleController scheduleController, 
        UserController userController,
        FunctionController functionController,
        HolidayController holidayController) : base(scheduleController, userController, functionController, holidayController)
    {
    }

    [Fact]
    public async Task AddFunctionTest()
    {
        const string NAME = "AddFunctionTest";
        var result = await FunctionController.AddFunction(new Shared.Models.Function.DrogeFunction
        {
            Name = NAME,
            TrainingTarget = 2,
            Active = true,
        });
        Assert.NotNull(result?.Value?.NewFunction);
        result.Value.Success.Should().Be(true);
        result.Value.NewFunction.Name.Should().Be(NAME);
        result.Value.NewFunction.TrainingTarget.Should().Be(2);
        result.Value.NewFunction.TrainingOnly.Should().Be(false);
        result.Value.NewFunction.Active.Should().Be(true);
        result.Value.ElapsedMilliseconds.Should().NotBe(-1);
    }

    [Fact]
    public async Task GetAll()
    {
        var newFunction = await AddFunction("GetAll functions", false);
        var result = await FunctionController.GetAll();
        Assert.NotNull(result?.Value?.Functions);
        result.Value.Success.Should().Be(true);
        result.Value.Functions.Should().Contain(x => x.Id == DefaultFunction);
        result.Value.Functions.Should().Contain(x => x.Id == newFunction);
        result.Value.ElapsedMilliseconds.Should().NotBe(-1);
    }
}
