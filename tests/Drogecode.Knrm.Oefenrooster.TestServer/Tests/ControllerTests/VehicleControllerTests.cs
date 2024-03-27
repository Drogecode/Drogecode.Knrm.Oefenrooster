using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class VehicleControllerTests : BaseTest
{
    public VehicleControllerTests(
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
        DefaultScheduleController defaultScheduleController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, dayItemController, monthItemController,
            preComController, vehicleController, defaultScheduleController)
    {
    }

    [Fact]
    public async Task AddVehicleTest()
    {
        var vehicle = new DrogeVehicle
        {
            Name = "xUnit test AddVehicleTest",
            Code = "xUnit",
            IsDefault = false,
            IsActive = true,
        };
        var result = await VehicleController.PutVehicle(vehicle);
        Assert.NotNull(result?.Value);
    }

    [Fact]
    public async Task LinkVehicle()
    {
        var body = new DrogeLinkVehicleTraining()
        {
            RoosterTrainingId = DefaultTraining,
            VehicleId = DefaultVehicle,
        };
        var result = await VehicleController.UpdateLinkVehicleTraining(body);
        Assert.NotNull(result?.Value?.DrogeLinkVehicleTraining);
        Assert.True(result.Value.Success);
    }
}
