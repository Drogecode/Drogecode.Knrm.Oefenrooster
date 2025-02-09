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
        DefaultScheduleController defaultScheduleController,
        ReportActionController reportActionController,
        ReportTrainingController reportTrainingController,
        UserRoleController userRoleController,
        UserLinkedMailsController userLinkedMailsController,
        ReportActionSharedController reportActionSharedController,
        AuditController auditController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, dayItemController, monthItemController,
            preComController, vehicleController, defaultScheduleController, reportActionController, reportTrainingController, userRoleController, userLinkedMailsController, reportActionSharedController,
            auditController)
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
    public async Task GetVehicleTest()
    {
        var result = await VehicleController.GetAll(false);
        Assert.NotNull(result.Value?.DrogeVehicles);
        Assert.NotEmpty(result.Value.DrogeVehicles);
        result.Value.DrogeVehicles.Should().Contain(x => x.Id == DefaultVehicle);
    }

    [Fact]
    public async Task PatchVehicleTest()
    {
        var NEW_NAME = "Patched";
        var result = await VehicleController.GetAll(false);
        result.Value!.DrogeVehicles.Should().Contain(x => x.Id == DefaultVehicle);
        var vehicle = result.Value!.DrogeVehicles!.FirstOrDefault(x => x.Id == DefaultVehicle);
        vehicle!.Name.Should().NotBe(NEW_NAME);
        vehicle!.Name = NEW_NAME;
        var patchResult = await VehicleController.PatchVehicle(vehicle);
        Assert.NotNull(patchResult.Value?.Success);
        Assert.True(patchResult.Value.Success);result = await VehicleController.GetAll(false);
        result.Value!.DrogeVehicles.Should().Contain(x => x.Id == DefaultVehicle);
        vehicle = result.Value!.DrogeVehicles!.FirstOrDefault(x => x.Id == DefaultVehicle);
        Assert.NotNull(vehicle);
        vehicle!.Name.Should().Be(NEW_NAME);
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
