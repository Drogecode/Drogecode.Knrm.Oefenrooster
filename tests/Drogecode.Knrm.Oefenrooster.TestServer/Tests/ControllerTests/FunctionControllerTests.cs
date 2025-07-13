using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class FunctionControllerTests : BaseTest
{
    public FunctionControllerTests(TestService testService) : base(testService)
    {
    }

    [Fact]
    public async Task AddFunctionTest()
    {
        const string NAME = "AddFunctionTest";
        var result = await Tester.FunctionController.AddFunction(new Shared.Models.Function.DrogeFunction
        {
            Name = NAME,
            TrainingTarget = 2,
            Active = true,
            Special = true,
        });
        Assert.NotNull(result?.Value?.NewId);
        result.Value.Success.Should().Be(true);
        var functionGet = await Tester.FunctionController.GetById(result.Value.NewId.Value);
        Assert.NotNull(functionGet?.Value?.Function);
        functionGet.Value.Success.Should().Be(true);
        functionGet.Value.Function.Name.Should().Be(NAME);
        functionGet.Value.Function.TrainingTarget.Should().Be(2);
        functionGet.Value.Function.TrainingOnly.Should().Be(false);
        functionGet.Value.Function.Active.Should().Be(true);
        functionGet.Value.Function.Special.Should().Be(true);
        result.Value.ElapsedMilliseconds.Should().NotBe(-1);
    }

    [Fact]
    public async Task GetAll()
    {
        var newFunction = await Tester.AddFunction("GetAll functions", false);
        var result = await Tester.FunctionController.GetAll();
        Assert.NotNull(result?.Value?.Functions);
        result.Value.Success.Should().Be(true);
        result.Value.Functions.Should().Contain(x => x.Id == Tester.DefaultFunction);
        result.Value.Functions.Should().Contain(x => x.Id == newFunction);
        result.Value.ElapsedMilliseconds.Should().NotBe(-1);
    }

    [Fact]
    public async Task PatchFunctionTest()
    {
        const string NAME = "PatchFunctionTest";
        var result = await Tester.FunctionController.AddFunction(new Shared.Models.Function.DrogeFunction
        {
            Name = NAME,
            TrainingTarget = 42,
            Active = false,
            Special = false,
        });
        Assert.NotNull(result?.Value?.NewId);
        result.Value.Success.Should().Be(true);
        var functionGet = await Tester.FunctionController.GetById(result.Value.NewId.Value);
        Assert.NotNull(functionGet?.Value?.Function);
        functionGet.Value.Function.Active.Should().Be(false);
        functionGet.Value.Function.Special.Should().Be(false);
        result.Value.ElapsedMilliseconds.Should().NotBe(-1);
        functionGet.Value.Function.Active = true;
        functionGet.Value.Function.Special = true;
        var patchResult = await Tester.FunctionController.PatchFunction(functionGet.Value.Function);
        Assert.NotNull(patchResult.Value?.Success);
        patchResult.Value.Success.Should().Be(true);
        var functionGetPatched = await Tester.FunctionController.GetById(result.Value.NewId.Value);
        Assert.NotNull(functionGetPatched?.Value?.Function);
        functionGetPatched.Value.Function.Active.Should().Be(true);
        functionGetPatched.Value.Function.Special.Should().Be(true);
    }
}