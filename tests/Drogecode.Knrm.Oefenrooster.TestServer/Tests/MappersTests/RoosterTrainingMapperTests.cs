using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.SharedForTests.Helpers;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.MappersTests;

public class RoosterTrainingMapperTests : BaseTest
{
    public RoosterTrainingMapperTests(TestService testService) : base(testService)
    {
    }

    [Fact]
    public void ToPlannedTrainingTest()
    {
        var id = Guid.NewGuid();
        var name = "ToPlannedTrainingTest";
        var orignal = new DbRoosterTraining()
        {
            Id = id,
            Name = name,
            RoosterAvailables = new List<DbRoosterAvailable>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = DefaultSettingsHelperMock.IdTaco,
                    VehicleId = Tester.DefaultVehicle
                }
            },
            LinkVehicleTrainings = new List<DbLinkVehicleTraining>
            {
                new()
                {
                    RoosterTrainingId = id,
                    VehicleId = Tester.DefaultVehicle,
                    IsSelected = true,
                }
            }
        };
        var mapped = orignal.ToPlannedTraining();
        mapped.Should().NotBeNull();
        mapped.Name.Should().Be(name);
        mapped.PlanUsers.Should().HaveCount(1);
        var taco = mapped.PlanUsers.FirstOrDefault(x => x.UserId == DefaultSettingsHelperMock.IdTaco);
        taco.Should().NotBeNull();
        taco!.VehicleId.Should().Be(Tester.DefaultVehicle);
        taco.Name.Should().Be("Some dude");
    }
}