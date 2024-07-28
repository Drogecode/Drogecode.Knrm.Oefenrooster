using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.MappersTests;

public class RoosterTrainingMapperTests : BaseTest
{
    public RoosterTrainingMapperTests(
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
        UserRoleController userRoleController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, dayItemController, monthItemController,
            preComController, vehicleController, defaultScheduleController, reportActionController, reportTrainingController, userRoleController)
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
                    UserId = DefaultSettingsHelper.IdTaco,
                    VehicleId = DefaultVehicle
                }
            },
            LinkVehicleTrainings = new List<DbLinkVehicleTraining>
            {
                new()
                {
                    RoosterTrainingId = id,
                    VehicleId = DefaultVehicle,
                    IsSelected = true,
                }
            }
        };
        var mapped = orignal.ToPlannedTraining();
        mapped.Should().NotBeNull();
        mapped.Name.Should().Be(name);
        mapped.PlanUsers.Should().HaveCount(1);
        var taco = mapped.PlanUsers.FirstOrDefault(x => x.UserId == DefaultSettingsHelper.IdTaco);
        taco.Should().NotBeNull();
        taco!.VehicleId.Should().Be(DefaultVehicle);
        taco.Name.Should().Be("Some dude");
    }
}