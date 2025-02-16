using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class TrainingTypesControllerTests : BaseTest
{
    public TrainingTypesControllerTests(TestService testService) : base(testService)
    {
    }

    [Fact]
    public async Task AddTrainingTypeBaseTest()
    {
        var result = await Tester.TrainingTypesController.PostNewTrainingType(new PlannerTrainingType
        {
            Name = "AddTrainingTypeBaseTest",
            ColorLight = "#bdbdbdff",
            ColorDark = "#ffffff4c",
            Order = 10,
            CountToTrainingTarget = true,
            IsDefault = true,
        });
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
    }

    [Fact]
    public async Task AddTrainingTypeOrderTest()
    {
        var type0 = await Tester.AddTrainingType("AddTrainingTypeOrderTest", 0);
        var type1 = await Tester.AddTrainingType("AddTrainingTypeOrderTest", 10);
        var type3 = await Tester.AddTrainingType("AddTrainingTypeOrderTest", 30);
        var resultPost = await Tester.TrainingTypesController.PostNewTrainingType(new PlannerTrainingType
        {
            Name = "AddTrainingTypeOrderTest",
            ColorLight = "#bdbdbdff",
            ColorDark = "#ffffff4c",
            CountToTrainingTarget = true,
            IsDefault = true,
        });
        Assert.NotNull(resultPost?.Value?.NewId);
        Assert.True(resultPost.Value.Success);
        var resultGet = await Tester.TrainingTypesController.GetById(resultPost.Value.NewId.Value);
        Assert.NotNull(resultGet?.Value?.TrainingType);
        resultGet.Value.TrainingType.Order.Should().Be(40);
    }

    [Fact]
    public async Task GetTrainingTypeByIdTest()
    {
        var result = await Tester.TrainingTypesController.GetById(Tester.DefaultTrainingType);
        Assert.NotNull(result?.Value?.TrainingType);
        Assert.True(result.Value.Success);
        result.Value.TrainingType.Id.Should().Be(Tester.DefaultTrainingType);
        result.Value.TrainingType.Name.Should().Be(TestService.TRAINING_TYPE_DEFAULT);
    }

    [Fact]
    public async Task GetTrainingTypesTest()
    {
        var oneMore = await Tester.AddTrainingType("GetTrainingTypesTest", 30);
        var result = await Tester.TrainingTypesController.GetTrainingTypes();
        Assert.NotNull(result?.Value?.PlannerTrainingTypes);
        result.Value.PlannerTrainingTypes.Should().NotBeEmpty();
        result.Value.PlannerTrainingTypes.Should().Contain(x => x.Id == Tester.DefaultTrainingType);
        result.Value.PlannerTrainingTypes.Should().Contain(x => x.Id == oneMore);
    }

    [Fact]
    public async Task PatchTrainingTypeTest()
    {
        var resultGetBefore = await Tester.TrainingTypesController.GetById(Tester.DefaultTrainingType);
        Assert.NotNull(resultGetBefore?.Value?.TrainingType);
        resultGetBefore.Value.TrainingType.IsActive.Should().BeTrue();
        resultGetBefore.Value.TrainingType.ColorLight.Should().Be("rgba(189,189,189,1)");
        resultGetBefore.Value.TrainingType.Name = "PatchTrainingTypeTest";
        resultGetBefore.Value.TrainingType.IsActive = false;
        resultGetBefore.Value.TrainingType.TextColorDark = "#000000";
        var resultPatched = await Tester.TrainingTypesController.PatchTrainingType(resultGetBefore.Value.TrainingType);
        Assert.NotNull(resultPatched?.Value?.Success);
        Assert.True(resultPatched.Value.Success);
        var resultGetAfter = await Tester.TrainingTypesController.GetById(Tester.DefaultTrainingType);
        Assert.NotNull(resultGetAfter?.Value?.TrainingType);
        resultGetAfter.Value.TrainingType.Name.Should().Be("PatchTrainingTypeTest");
        resultGetAfter.Value.TrainingType.IsActive.Should().BeFalse();
        resultGetAfter.Value.TrainingType.ColorLight.Should().Be("rgba(189,189,189,1)");
        resultGetAfter.Value.TrainingType.TextColorDark.Should().Be("rgba(0,0,0,1)");
    }
}
