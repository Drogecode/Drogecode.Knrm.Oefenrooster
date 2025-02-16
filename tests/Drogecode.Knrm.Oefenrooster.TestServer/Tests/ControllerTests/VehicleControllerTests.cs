using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class VehicleControllerTests : BaseTest
{
    public VehicleControllerTests(TestService testService) : base(testService)
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
        var result = await Tester.VehicleController.PutVehicle(vehicle);
        Assert.NotNull(result?.Value);
    }
    
    [Fact]
    public async Task GetVehicleTest()
    {
        var result = await Tester.VehicleController.GetAll(false);
        Assert.NotNull(result.Value?.DrogeVehicles);
        Assert.NotEmpty(result.Value.DrogeVehicles);
        result.Value.DrogeVehicles.Should().Contain(x => x.Id == Tester.DefaultVehicle);
    }

    [Fact]
    public async Task PatchVehicleTest()
    {
        var NEW_NAME = "Patched";
        var result = await Tester.VehicleController.GetAll(false);
        result.Value!.DrogeVehicles.Should().Contain(x => x.Id == Tester.DefaultVehicle);
        var vehicle = result.Value!.DrogeVehicles!.FirstOrDefault(x => x.Id == Tester.DefaultVehicle);
        vehicle!.Name.Should().NotBe(NEW_NAME);
        vehicle!.Name = NEW_NAME;
        var patchResult = await Tester.VehicleController.PatchVehicle(vehicle);
        Assert.NotNull(patchResult.Value?.Success);
        Assert.True(patchResult.Value.Success);result = await Tester.VehicleController.GetAll(false);
        result.Value!.DrogeVehicles.Should().Contain(x => x.Id == Tester.DefaultVehicle);
        vehicle = result.Value!.DrogeVehicles!.FirstOrDefault(x => x.Id == Tester.DefaultVehicle);
        Assert.NotNull(vehicle);
        vehicle!.Name.Should().Be(NEW_NAME);
    }

    [Fact]
    public async Task LinkVehicle()
    {
        var body = new DrogeLinkVehicleTraining()
        {
            RoosterTrainingId = Tester.DefaultTraining,
            VehicleId = Tester.DefaultVehicle,
        };
        var result = await Tester.VehicleController.UpdateLinkVehicleTraining(body);
        Assert.NotNull(result?.Value?.DrogeLinkVehicleTraining);
        Assert.True(result.Value.Success);
    }
}
