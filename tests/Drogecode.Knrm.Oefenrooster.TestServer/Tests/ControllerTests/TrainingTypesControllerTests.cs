using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class TrainingTypesControllerTests : BaseTest
{
    private readonly TrainingTypesController _trainingTypesController;
    public TrainingTypesControllerTests(
        ScheduleController scheduleController, 
        TrainingTypesController trainingTypesController,
        UserController userController,
        FunctionController functionController,
        HolidayController holidayController) : base(scheduleController, userController, functionController, holidayController)
    {
        _trainingTypesController = trainingTypesController;
        MockAuthenticatedUser(trainingTypesController);
    }

    [Fact]
    public async Task GetTrainingTypesTest()
    {
        var result = await _trainingTypesController.GetTrainingTypes();
        Assert.NotNull(result?.Value?.PlannerTrainingTypes);
        //result.Value.PlannerTrainingTypes.Should().NotBeEmpty();
    }
}
