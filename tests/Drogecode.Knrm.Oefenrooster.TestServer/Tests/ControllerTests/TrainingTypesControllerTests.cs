using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Microsoft.Extensions.Azure;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class TrainingTypesControllerTests : BaseTest
{
    public TrainingTypesControllerTests(
        ScheduleController scheduleController,
        UserController userController,
        FunctionController functionController,
        HolidayController holidayController,
        TrainingTypesController trainingTypesController,
        CalendarItemController calendarItemController,
        PreComController preComController) :
        base(scheduleController, userController, functionController, holidayController, trainingTypesController, calendarItemController, preComController)
    {
    }

    [Fact]
    public async Task AddTrainingTypeBaseTest()
    {
        var result = await TrainingTypesController.PostNewTrainingType(new PlannerTrainingType
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
        var type0 = await AddTrainingType("AddTrainingTypeOrderTest", 0);
        var type1 = await AddTrainingType("AddTrainingTypeOrderTest", 10);
        var type3 = await AddTrainingType("AddTrainingTypeOrderTest", 30);
        var resultPost = await TrainingTypesController.PostNewTrainingType(new PlannerTrainingType
        {
            Name = "AddTrainingTypeOrderTest",
            ColorLight = "#bdbdbdff",
            ColorDark = "#ffffff4c",
            CountToTrainingTarget = true,
            IsDefault = true,
        });
        Assert.NotNull(resultPost?.Value?.NewId);
        Assert.True(resultPost.Value.Success);
        var resultGet = await TrainingTypesController.GetById(resultPost.Value.NewId.Value);
        Assert.NotNull(resultGet?.Value?.TrainingType);
        resultGet.Value.TrainingType.Order.Should().Be(40);
    }

    [Fact]
    public async Task GetTrainingTypeByIdTest()
    {
        var result = await TrainingTypesController.GetById(DefaultTrainingType);
        Assert.NotNull(result?.Value?.TrainingType);
        Assert.True(result.Value.Success);
        result.Value.TrainingType.Id.Should().Be(DefaultTrainingType);
        result.Value.TrainingType.Name.Should().Be(TRAINING_TYPE_DEFAULT);
    }

    [Fact]
    public async Task GetTrainingTypesTest()
    {
        var oneMore = await AddTrainingType("GetTrainingTypesTest", 30);
        var result = await TrainingTypesController.GetTrainingTypes();
        Assert.NotNull(result?.Value?.PlannerTrainingTypes);
        result.Value.PlannerTrainingTypes.Should().NotBeEmpty();
        result.Value.PlannerTrainingTypes.Should().Contain(x => x.Id == DefaultTrainingType);
        result.Value.PlannerTrainingTypes.Should().Contain(x => x.Id == oneMore);
    }

    [Fact]
    public async Task PatchTrainingTypeTest()
    {
        var resultGetBefore = await TrainingTypesController.GetById(DefaultTrainingType);
        Assert.NotNull(resultGetBefore?.Value?.TrainingType);
        resultGetBefore.Value.TrainingType.IsActive.Should().BeTrue();
        resultGetBefore.Value.TrainingType.ColorLight.Should().Be("#bdbdbdff");
        resultGetBefore.Value.TrainingType.Name = "PatchTrainingTypeTest";
        resultGetBefore.Value.TrainingType.IsActive = false;
        resultGetBefore.Value.TrainingType.TextColorDark = "#000000";
        var resultPatched = await TrainingTypesController.PatchTrainingType(resultGetBefore.Value.TrainingType);
        Assert.NotNull(resultPatched?.Value?.Success);
        Assert.True(resultPatched.Value.Success);
        var resultGetAfter = await TrainingTypesController.GetById(DefaultTrainingType);
        Assert.NotNull(resultGetAfter?.Value?.TrainingType);
        resultGetAfter.Value.TrainingType.Name.Should().Be("PatchTrainingTypeTest");
        resultGetAfter.Value.TrainingType.IsActive.Should().BeFalse();
        resultGetAfter.Value.TrainingType.ColorLight.Should().Be("#bdbdbdff");
        resultGetAfter.Value.TrainingType.TextColorDark.Should().Be("#000000ff");
    }
}
