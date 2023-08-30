using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class VehicleControllerTests : BaseTest
{
    public VehicleControllerTests(
        DataContext dataContext,
        IDateTimeService dateTimeServiceMock,
        ScheduleController scheduleController,
        UserController userController,
        FunctionController functionController,
        HolidayController holidayController,
        TrainingTypesController trainingTypesController,
        CalendarItemController calendarItemController,
        PreComController preComController,
        VehicleController vehicleController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, calendarItemController, preComController, vehicleController)
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
