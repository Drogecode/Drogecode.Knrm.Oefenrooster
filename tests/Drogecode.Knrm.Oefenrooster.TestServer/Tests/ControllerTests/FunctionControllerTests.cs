using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class FunctionControllerTests : BaseTest
{
    public FunctionControllerTests(
        DataContext dataContext,
        IDateTimeService dateTimeServiceMock,
        ScheduleController scheduleController,
        FunctionController functionController,
        UserController userController,
        HolidayController holidayController,
        TrainingTypesController trainingTypesController,
        DayItemController dayItemController,
        MonthItemController monthItemController,
        PreComController preComController,
        VehicleController vehicleController,
        DefaultScheduleController defaultScheduleController,
        ReportActionController reportActionController,
        ReportTrainingController reportTrainingController,
        UserRoleController userRoleController,
        UserLinkedMailsController userLinkedMailsController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, dayItemController, monthItemController,
            preComController, vehicleController, defaultScheduleController, reportActionController, reportTrainingController, userRoleController, userLinkedMailsController)
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
