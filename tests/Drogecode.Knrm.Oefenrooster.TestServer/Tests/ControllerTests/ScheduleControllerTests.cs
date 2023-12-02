using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class ScheduleControllerTests : BaseTest
{
    public ScheduleControllerTests(
        DataContext dataContext,
        IDateTimeService dateTimeServiceMock,
        ScheduleController scheduleController,
        FunctionController functionController,
        UserController userController,
        HolidayController holidayController,
        TrainingTypesController trainingTypesController,
        CalendarItemController calendarItemController,
        PreComController preComController,
        VehicleController vehicleController,
        DefaultScheduleController defaultScheduleController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, calendarItemController, preComController,
            vehicleController, defaultScheduleController)
    {
    }

    [Fact]
    public async Task AddTrainingTest()
    {
        var body = new PlannedTraining
        {
            Name = "EditTraining",
            DateStart = DateTime.Today.AddHours(12).AddMinutes(50),
            DateEnd = DateTime.Today.AddHours(13).AddMinutes(40),
        };
        var result = await ScheduleController.AddTraining(body);
        Assert.NotNull(result?.Value?.NewId);
        Assert.True(result.Value.Success);
        var tody = DateTime.Today;
        var yesterday = DateTime.Today.AddDays(-1);
        var tomorrow = DateTime.Today.AddDays(1);
        var getResult = await ScheduleController.ForAll(tody.Month, yesterday.Year, yesterday.Month, yesterday.Day, tomorrow.Year, tomorrow.Month, tomorrow.Day, false);
        Assert.NotNull(getResult?.Value?.Success);
        Assert.True(getResult.Value.Success);
    }

    [Fact]
    public async Task FailAssignTrainingTest()
    {
        var user = (await UserController.GetById(DefaultUserId))!.Value!.User;
        Assert.NotNull(user);
        var training = await AddTraining("AssignTraining", true);
        var body = new PatchAssignedUserRequest
        {
            TrainingId = training,
            User = new PlanUser
            {
                UserId = user.Id,
                Assigned = true,
                UserFunctionId = DefaultFunction
            }
        };
        var result = await ScheduleController.PatchAssignedUser(body);
        Assert.NotNull(result?.Value?.IdPatched);
        Assert.False(result.Value.Success);
    }

    [Fact]
    public async Task AssignTrainingTest()
    {
        var user = (await UserController.GetById(DefaultUserId))!.Value!.User;
        Assert.NotNull(user);
        var training = await AddTraining("AssignTraining", true);
        var body = new PatchAssignedUserRequest
        {
            TrainingId = training,
            User = new PlanUser
            {
                UserId = user.Id,
                Assigned = true,
                UserFunctionId = DefaultFunction
            },
            Training = new TrainingAdvance
            {
                TrainingId = training
            }
        };
        var result = await ScheduleController.PatchAssignedUser(body);
        Assert.NotNull(result?.Value?.IdPatched);
        Assert.True(result.Value.Success);
    }

    [Fact]
    public async Task AllTrainingsForUserSingleTest()
    {
        var result = await ScheduleController.AllTrainingsForUser(DefaultUserId);
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
        result.Value.Trainings.Should().NotBeEmpty();
        result.Value.UserMonthInfos.Should().NotBeEmpty();
        var training = result.Value.Trainings.FirstOrDefault(x => x.TrainingId == DefaultTraining);
        Assert.NotNull(training);
        training.CountToTrainingTarget.Should().BeFalse();
        var userMonthInfo = result.Value.UserMonthInfos.FirstOrDefault(x => x.Year == DateTime.UtcNow.Year && x.Month == DateTime.UtcNow.Month);
        Assert.NotNull(userMonthInfo);
        userMonthInfo.All.Should().Be(1);
        userMonthInfo.Valid.Should().Be(0);
    }

    [Fact]
    public async Task AllTrainingsForUserMultipleTest()
    {
        var training1 = await AddTraining("AllTrainings_1", false);
        var training2 = await AddTraining("AllTrainings_2", true);
        var training3 = await AddTraining("AllTrainings_3", false);
        var training4 = await AddTraining("AllTrainings_4", true);
        var training5 = await AddTraining("AllTrainings_5", false);
        var training6 = await AddTraining("AllTrainings_6", true);
        await AssignTrainingToUser(training1, DefaultUserId, true);
        await AssignTrainingToUser(training2, DefaultUserId, true);
        await AssignTrainingToUser(training3, DefaultUserId, true);
        await AssignTrainingToUser(training4, DefaultUserId, false);
        await AssignTrainingToUser(training5, DefaultUserId, true);
        await AssignTrainingToUser(training6, DefaultUserId, true);
        var result = await ScheduleController.AllTrainingsForUser(DefaultUserId);
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
        result.Value.Trainings.Should().NotBeEmpty();
        result.Value.UserMonthInfos.Should().NotBeEmpty();
        var training = result.Value.Trainings.FirstOrDefault(x => x.TrainingId == DefaultTraining);
        Assert.NotNull(training);
        training.CountToTrainingTarget.Should().BeFalse();
        var userMonthInfo = result.Value.UserMonthInfos.FirstOrDefault(x => x.Year == DateTime.UtcNow.Year && x.Month == DateTime.UtcNow.Month);
        Assert.NotNull(userMonthInfo);
        userMonthInfo.All.Should().BeGreaterThanOrEqualTo(5);
        userMonthInfo.Valid.Should().BeGreaterThanOrEqualTo(2);
        userMonthInfo.Valid.Should().NotBe(userMonthInfo.All);
    }

    [Fact]
    public async Task GetPinnedTrainingsForUserTest()
    {
        var body = new PlannedTraining
        {
            Name = "GetPinnedTrainings",
            DateStart = DateTime.Today.AddDays(1).AddHours(12).AddMinutes(50),
            DateEnd = DateTime.Today.AddDays(1).AddHours(13).AddMinutes(40),
            CountToTrainingTarget = true,
            IsPinned = true,
        };
        var putResult = await ScheduleController.AddTraining(body);
        Assert.NotNull(putResult?.Value?.NewId);
        var Getresult = await ScheduleController.GetPinnedTrainingsForUser();
        Assert.NotNull(Getresult?.Value?.Trainings);
        Getresult.Value.Trainings.Should().NotBeEmpty();
        Getresult.Value.Trainings.Should().Contain(x => x.TrainingId == putResult.Value.NewId);
        Getresult.Value.Trainings.Should().NotContain(x => x.TrainingId == DefaultTraining);
    }

    [Fact]
    public async Task GetScheduledTrainingsForUserTest()
    {
        DateTimeServiceMock.SetMockDateTime(DateTime.Today.AddMonths(12));
        var dateStart = DateTime.Today.AddMonths(12).AddHours(21);
        var dateEnd = DateTime.Today.AddMonths(12).AddHours(15);
        var trainingId = await PrepareAssignedTraining(dateStart, dateEnd);
        var trainings = await ScheduleController.GetScheduledTrainingsForUser();
        Assert.NotNull(trainings?.Value?.Trainings);
        trainings.Value.Trainings.Should().Contain(x => x.TrainingId == trainingId);
        DateTimeServiceMock.SetMockDateTime(null);
    }
    [Fact]
    public async Task GetScheduledTrainingsForUserButNotInPastTest()
    {
        DateTimeServiceMock.SetMockDateTime(DateTime.Today.AddMonths(12).AddDays(2));
        var dateStart = DateTime.Today.AddMonths(12).AddHours(21);
        var dateEnd = DateTime.Today.AddMonths(12).AddHours(15);
        var trainingId = await PrepareAssignedTraining(dateStart, dateEnd);
        var trainings = await ScheduleController.GetScheduledTrainingsForUser();
        Assert.NotNull(trainings?.Value?.Trainings);
        trainings.Value.Trainings.Should().NotContain(x => x.TrainingId == trainingId);
        DateTimeServiceMock.SetMockDateTime(null);
    }

    [Fact]
    public async Task PatchScheduleForUserTest()
    {
        var start = DateTime.Today.AddDays(1).AddHours(12);
        var end = DateTime.Today.AddDays(1).AddHours(14);
        var trainingId = await AddTraining("PatchScheduleForUserTest", false, start, end);
        var training = (await ScheduleController.GetTrainingById(trainingId))?.Value?.Training;
        Assert.NotNull(training);
        training.Availabilty.Should().NotBe(Availabilty.Available);
        training.Availabilty = Availabilty.Available;
        var patchedResult = await ScheduleController.PatchScheduleForUser(training);
        Assert.NotNull(patchedResult?.Value);
        Assert.True(patchedResult.Value.Success);
        var trainingAfterPatch = (await ScheduleController.GetTrainingById(trainingId))?.Value?.Training;
        Assert.NotNull(trainingAfterPatch);
        trainingAfterPatch.Availabilty.Should().Be(Availabilty.Available);
    }

    [Fact]
    public async Task PatchScheduleForUserPastTest()
    {
        var triningInPastId = await AddTraining("PatchScheduleForUserPastTest", false, DateTime.Today.AddDays(-1).AddHours(10), DateTime.Today.AddDays(-1).AddHours(12));
        var training = (await ScheduleController.GetTrainingById(triningInPastId))?.Value?.Training;
        Assert.NotNull(training);
        training.Availabilty.Should().Be(null);
        training.Availabilty = Availabilty.Available;
        var patchedResult = await ScheduleController.PatchScheduleForUser(training);
        Assert.NotNull(patchedResult?.Value);
        Assert.False(patchedResult.Value.Success);
        var trainingAfterPatch = (await ScheduleController.GetTrainingById(DefaultTraining))?.Value?.Training;
        Assert.NotNull(trainingAfterPatch);
        trainingAfterPatch.Availabilty.Should().Be(null);
    }

    [Fact]
    public async Task DeleteTrainingBaseTest()
    {
        var training = await ScheduleController.GetTrainingById(DefaultTraining);
        Assert.NotNull(training?.Value?.Training?.TrainingId);
        var deleteResult = await ScheduleController.DeleteTraining(DefaultTraining);
        Assert.True(deleteResult?.Value);
        training = await ScheduleController.GetTrainingById(DefaultTraining);
        Assert.Null(training?.Value?.Training?.TrainingId);
    }

    [Fact]
    public async Task DeleteTraining_NotInPinnedTest()
    {
        var trainingId = await AddTraining("DeleteTraining_NotInPinned", false, DateTime.Today.AddDays(1).AddHours(10), DateTime.Today.AddDays(1).AddHours(12), true);
        var trainings = await ScheduleController.GetPinnedTrainingsForUser(); ;
        Assert.NotNull(trainings?.Value?.Trainings);
        trainings.Value.Trainings.Should().Contain(x => x.TrainingId == trainingId);
        var deleteResult = await ScheduleController.DeleteTraining(trainingId);
        Assert.True(deleteResult?.Value);
        trainings = await ScheduleController.GetPinnedTrainingsForUser();
        Assert.NotNull(trainings?.Value?.Trainings);
        trainings.Value.Trainings.Should().NotContain(x => x.TrainingId == trainingId);
    }

    [Fact]
    public async Task DeleteTraining_NotInAllTrainingsForUserTest()
    {
        var trainingId = await AddTraining("NotInAllTrainingsForUser", false);
        var training = await ScheduleController.GetTrainingById(trainingId);
        await ScheduleController.PutAssignedUser(new OtherScheduleUserRequest
        {
            TrainingId = trainingId,
            UserId = DefaultUserId,
            Assigned = true,
            Training = training.Value!.Training,
        });
        var trainings = await ScheduleController.AllTrainingsForUser(DefaultUserId);
        Assert.NotNull(trainings?.Value?.Trainings);
        trainings.Value.Trainings.Should().Contain(x => x.TrainingId == trainingId);
        var deleteResult = await ScheduleController.DeleteTraining(trainingId);
        Assert.True(deleteResult?.Value);
        trainings = await ScheduleController.AllTrainingsForUser(DefaultUserId);
        Assert.NotNull(trainings?.Value?.Trainings);
        trainings.Value.Trainings.Should().NotContain(x => x.TrainingId == trainingId);
    }

    [Fact]
    public async Task DeleteTraining_NotInForUserTest()
    {
        MockAuthenticatedUser(ScheduleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId, new List<string> { AccessesNames.AUTH_scheduler_edit_past });
        var trainingId = await AddTraining("DeleteTraining_NotInForUser", false, new DateTime(2020, 9, 4, 12, 0, 0), new DateTime(2020, 9, 4, 14, 0, 0));
        var training = await ScheduleController.GetTrainingById(trainingId);
        await ScheduleController.PutAssignedUser(new OtherScheduleUserRequest
        {
            TrainingId = trainingId,
            UserId = DefaultUserId,
            Assigned = true,
            Training = training.Value!.Training,
        });
        var trainings = await ScheduleController.ForUser(2020, 9, 1, 2020, 9, 30);
        Assert.NotNull(trainings?.Value?.Trainings);
        trainings.Value.Trainings.Should().Contain(x => x.TrainingId == trainingId);
        var deleteResult = await ScheduleController.DeleteTraining(trainingId);
        Assert.True(deleteResult?.Value);
        trainings = await ScheduleController.ForUser(2020, 9, 1, 2020, 9, 30);
        Assert.NotNull(trainings?.Value?.Trainings);
        trainings.Value.Trainings.Should().NotContain(x => x.TrainingId == trainingId);
        MockAuthenticatedUser(ScheduleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
    }

    [Fact]
    public async Task DeleteTraining_NotInForAllTest()
    {
        MockAuthenticatedUser(ScheduleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId, new List<string> { AccessesNames.AUTH_scheduler_edit_past });
        var trainingId = await AddTraining("DeleteTraining_NotInForAll", false, new DateTime(2020, 9, 4, 12, 0, 0), new DateTime(2020, 9, 4, 14, 0, 0));
        var training = await ScheduleController.GetTrainingById(trainingId);
        var trainings = await ScheduleController.ForAll(9, 2020, 8, 28, 2020, 10, 2, false);
        Assert.NotNull(trainings?.Value?.Planners);
        trainings.Value.Planners.Should().Contain(x => x.TrainingId == trainingId);
        var deleteResult = await ScheduleController.DeleteTraining(trainingId);
        Assert.True(deleteResult?.Value);
        trainings = await ScheduleController.ForAll(9, 2020, 8, 28, 2020, 10, 2, false);
        Assert.NotNull(trainings?.Value?.Planners);
        trainings.Value.Planners.Should().NotContain(x => x.TrainingId == trainingId);
        MockAuthenticatedUser(ScheduleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
    }

    [Fact]
    public async Task DeleteTraining_NotAllowedTest()
    {
        var trainingId = await AddTraining("DeleteTraining_NotAllowed", false, new DateTime(2020, 9, 4, 12, 0, 0), new DateTime(2020, 9, 4, 14, 0, 0));
        var training = await ScheduleController.GetTrainingById(trainingId);
        var trainings = await ScheduleController.ForAll(9, 2020, 8, 28, 2020, 10, 2, false);
        Assert.NotNull(trainings?.Value?.Planners);
        trainings.Value.Planners.Should().Contain(x => x.TrainingId == trainingId);
        var deleteResult = await ScheduleController.DeleteTraining(trainingId);
        Assert.False(deleteResult?.Value);
        deleteResult!.Result.Should().BeOfType<Microsoft.AspNetCore.Mvc.UnauthorizedResult>();
    }

    [Fact]
    public async Task PinnedWhenOtherUserSelectTest()
    {
        var trainingId = await AddTraining("PinnedWhenOtherUserSelect", false, DateTime.UtcNow.AddDays(1).AddHours(12), DateTime.UtcNow.AddDays(1).AddHours(14), true);
        var trainings = await ScheduleController.GetPinnedTrainingsForUser();
        Assert.NotNull(trainings?.Value?.Trainings);
        trainings.Value.Trainings.Should().Contain(x => x.TrainingId == trainingId);
        var training = trainings.Value.Trainings.FirstOrDefault(x => x.TrainingId == trainingId);
        training.Should().NotBeNull();
        training!.Availabilty = Availabilty.Available;
        MockAuthenticatedUser(ScheduleController, DefaultUserId, DefaultCustomerId);
        trainings = await ScheduleController.GetPinnedTrainingsForUser();
        Assert.NotNull(trainings?.Value?.Trainings);
        trainings.Value.Trainings.Should().Contain(x => x.TrainingId == trainingId);
        var patchAssigned = await ScheduleController.PatchScheduleForUser(training, CancellationToken.None);
        patchAssigned?.Value?.Success.Should().BeTrue();
        trainings = await ScheduleController.GetPinnedTrainingsForUser();
        Assert.NotNull(trainings?.Value?.Trainings);
        trainings.Value.Trainings.Should().NotContain(x => x.TrainingId == trainingId);
        MockAuthenticatedUser(ScheduleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        trainings = await ScheduleController.GetPinnedTrainingsForUser();
        Assert.NotNull(trainings?.Value?.Trainings);
        trainings.Value.Trainings.Should().Contain(x => x.TrainingId == trainingId);
    }

    [Fact]
    public async Task TrainingsForAllNoVehicleLinkedTest()
    {
        MockAuthenticatedUser(ScheduleController, DefaultUserId, DefaultCustomerId);
        var startDate = DateTime.Today.AddMonths(12).AddHours(21);
        var endDate = DateTime.Today.AddMonths(12).AddHours(15);
        var trainingId = await AddTraining("TrainingsForAll", false, startDate, endDate);
        var training = (await ScheduleController.GetTrainingById(trainingId)).Value?.Training;
        Assert.NotNull(training);
        var body = new OtherScheduleUserRequest
        {
            TrainingId = trainingId,
            UserId = DefaultUserId,
            Assigned = true,
            Training = training,
        };
        var PutAssigned = await ScheduleController.PutAssignedUser(body);
        PutAssigned?.Value?.Success.Should().BeTrue();
        var trainings = await ScheduleController.ForAll(startDate.Month, startDate.Year, startDate.Month, startDate.Day, endDate.Year, endDate.Month, endDate.Day, false);
        Assert.NotNull(trainings?.Value?.Planners);
        trainings.Value.Planners.Should().Contain(x => x.TrainingId == trainingId);
        var trainingFromAll = trainings.Value.Planners.FirstOrDefault(x => x.TrainingId == trainingId);
        Assert.NotNull(trainingFromAll);
        var planUser = trainingFromAll.PlanUsers.FirstOrDefault(x => x.UserId == DefaultUserId);
        Assert.NotNull(planUser);
        Assert.Null(planUser?.VehicleId);
    }

    [Fact]
    public async Task TrainingsForAllVehicleLinkedTest()
    {
        var dateStart = DateTime.Today.AddMonths(12).AddHours(21);
        var dateEnd = DateTime.Today.AddMonths(12).AddHours(15);
        var trainingId = await PrepareAssignedTraining(dateStart, dateEnd);
        var trainings = await ScheduleController.ForAll(dateStart.Month, dateStart.Year, dateStart.Month, dateStart.Day, dateEnd.Year, dateEnd.Month, dateEnd.Day, false);
        Assert.NotNull(trainings?.Value?.Planners);
        trainings.Value.Planners.Should().Contain(x => x.TrainingId == trainingId);
        var trainingFromAll = trainings.Value.Planners.FirstOrDefault(x => x.TrainingId == trainingId);
        Assert.NotNull(trainingFromAll);
        var planUser = trainingFromAll.PlanUsers.FirstOrDefault(x => x.UserId == DefaultUserId);
        Assert.NotNull(planUser?.VehicleId);
        planUser!.VehicleId.Should().Be(DefaultVehicle);
        MockAuthenticatedUser(ScheduleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        MockAuthenticatedUser(VehicleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
    }

    [Fact]
    public async Task TrainingsForAllDefaultVehicleNotLinkedTest()
    {
        var dateStart = DateTime.Today.AddMonths(12).AddHours(21);
        var dateEnd = DateTime.Today.AddMonths(12).AddHours(15);
        var trainingId = await PrepareAssignedTraining(dateStart, dateEnd);
        var defVeh = await AddVehicle("TrainingsForAllDefaultVehicleNotLinked", "forall", true, true);
        var updateLinkResponse = await VehicleController.UpdateLinkVehicleTraining(new DrogeLinkVehicleTraining
        {
            RoosterTrainingId = trainingId,
            VehicleId = defVeh,
            IsSelected = false,
        });
        Assert.True(updateLinkResponse?.Value?.Success);
        var trainings = await ScheduleController.ForAll(dateStart.Month, dateStart.Year, dateStart.Month, dateStart.Day, dateEnd.Year, dateEnd.Month, dateEnd.Day, false);
        Assert.NotNull(trainings?.Value?.Planners);
        trainings.Value.Planners.Should().Contain(x => x.TrainingId == trainingId);
        var trainingFromAll = trainings.Value.Planners.FirstOrDefault(x => x.TrainingId == trainingId);
        Assert.NotNull(trainingFromAll);
        var planUser = trainingFromAll.PlanUsers.FirstOrDefault(x => x.UserId == DefaultUserId);
        Assert.NotNull(planUser?.VehicleId);
        planUser!.VehicleId.Should().Be(DefaultVehicle);

        MockAuthenticatedUser(ScheduleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        MockAuthenticatedUser(VehicleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
    }

    [Fact]
    public async Task CorrectVehicleOnDahboardTest()
    {
        var dateStart = DateTime.Today.AddMonths(12).AddHours(21);
        var dateEnd = DateTime.Today.AddMonths(12).AddHours(15);
        var trainingId = await PrepareAssignedTraining(dateStart, dateEnd);
        var defVeh = await AddVehicle("default vehicle", "forall", true, true);
        var updateLinkResponse = await VehicleController.UpdateLinkVehicleTraining(new DrogeLinkVehicleTraining
        {
            RoosterTrainingId = trainingId,
            VehicleId = defVeh,
            IsSelected = false,
        });
        Assert.True(updateLinkResponse?.Value?.Success);
        var trainingsOnDashboard = await ScheduleController.GetScheduledTrainingsForUser();
        trainingsOnDashboard.Value?.Trainings.Should().Contain(x => x.TrainingId == trainingId);
        var thisTraining = trainingsOnDashboard.Value!.Trainings.FirstOrDefault(x => x.TrainingId == trainingId);
        var planUser = thisTraining!.PlanUsers.FirstOrDefault(x => x.UserId == DefaultUserId);
        planUser.Should().NotBeNull();
        planUser!.VehicleId.Should().Be(DefaultVehicle);

        MockAuthenticatedUser(ScheduleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        MockAuthenticatedUser(VehicleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
    }

    [Fact]
    public async Task GetPlannedTrainingByIdTest()
    {
        var dateStart = DateTime.Today.AddMonths(12).AddHours(21);
        var dateEnd = DateTime.Today.AddMonths(12).AddHours(15);
        var user1 = await AddUser("user1_fortest");
        var trainingId = await PrepareAssignedTraining(dateStart, dateEnd);
        var plannedTraining = await ScheduleController.GetPlannedTrainingById(trainingId);
        Assert.NotNull(plannedTraining?.Value?.Training?.PlanUsers);
        plannedTraining.Value.Training.PlanUsers.Should().Contain(x => x.UserId == DefaultUserId);
        plannedTraining.Value.Training.PlanUsers.Should().NotContain(x => x.UserId == user1);
        var userFromTraining = plannedTraining.Value.Training.PlanUsers.FirstOrDefault(x => x.UserId == DefaultUserId);
        Assert.NotNull(userFromTraining);
        userFromTraining.Name.Should().Be(USER_NAME);
        userFromTraining.UserFunctionId.Should().Be(DefaultFunction);
        MockAuthenticatedUser(ScheduleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        MockAuthenticatedUser(VehicleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
    }

    [Fact]
    public async Task GetTrainingsForAllIncludeUnAssigned()
    {
        var dateStart = DateTime.Today.AddDays(1).AddHours(21);
        var dateEnd = DateTime.Today.AddDays(1).AddHours(15);
        var user1 = await AddUser("user1_fortest");
        MockAuthenticatedUser(ScheduleController, user1, DefaultCustomerId);
        MockAuthenticatedUser(VehicleController, user1, DefaultCustomerId);
        await AddHoliday("GetTrainingsForAllIncludeUnAssigned");
        var trainingId = await PrepareAssignedTraining(dateStart, dateEnd);
        var trainings = await ScheduleController.ForAll(dateStart.Month, dateStart.Year, dateStart.Month, dateStart.Day, dateEnd.Year, dateEnd.Month, dateEnd.Day, true);
        Assert.NotNull(trainings.Value?.Planners);
        Assert.NotEmpty(trainings.Value.Planners);
        trainings.Value.Planners.Should().Contain(x => x.TrainingId == trainingId);
        var training = trainings.Value.Planners.FirstOrDefault(x => x.TrainingId == trainingId);
        Assert.NotNull(training);
        Assert.NotEmpty(training.PlanUsers);
        training.PlanUsers.Should().Contain(x => x.UserId == DefaultUserId);
        training.PlanUsers.Should().Contain(x => x.UserId == user1);
        MockAuthenticatedUser(ScheduleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        MockAuthenticatedUser(VehicleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
    }

    [Fact]
    public async Task GetTrainingsForAllNotIncludeUnAssigned()
    {
        var dateStart = DateTime.Today.AddDays(1).AddHours(21);
        var dateEnd = DateTime.Today.AddDays(1).AddHours(15);
        var user1 = await AddUser("user1_fortest");
        MockAuthenticatedUser(ScheduleController, user1, DefaultCustomerId);
        MockAuthenticatedUser(VehicleController, user1, DefaultCustomerId);
        await AddHoliday("GetTrainingsForAllNotIncludeUnAssigned");
        var trainingId = await PrepareAssignedTraining(dateStart, dateEnd);
        var trainings = await ScheduleController.ForAll(dateStart.Month, dateStart.Year, dateStart.Month, dateStart.Day, dateEnd.Year, dateEnd.Month, dateEnd.Day, false);
        Assert.NotNull(trainings.Value?.Planners);
        Assert.NotEmpty(trainings.Value.Planners);
        trainings.Value.Planners.Should().Contain(x => x.TrainingId == trainingId);
        var training = trainings.Value.Planners.FirstOrDefault(x => x.TrainingId == trainingId);
        Assert.NotNull(training);
        Assert.NotEmpty(training.PlanUsers);
        training.PlanUsers.Should().Contain(x => x.UserId == DefaultUserId);
        training.PlanUsers.Should().NotContain(x => x.UserId == user1);
        var user = training.PlanUsers.FirstOrDefault(x => x.UserId == DefaultUserId);
        user!.VehicleId.Should().Be(DefaultVehicle);
        MockAuthenticatedUser(ScheduleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        MockAuthenticatedUser(VehicleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
    }

    private async Task<Guid> PrepareAssignedTraining(DateTime dateStart, DateTime dateEnd)
    {
        MockAuthenticatedUser(ScheduleController, DefaultUserId, DefaultCustomerId);
        MockAuthenticatedUser(VehicleController, DefaultUserId, DefaultCustomerId);
        var trainingId = await AddTraining("TrainingsForAll", false, dateStart, dateEnd);
        var training = (await ScheduleController.GetTrainingById(trainingId)).Value?.Training;
        Assert.NotNull(training);
        var body = new OtherScheduleUserRequest
        {
            TrainingId = trainingId,
            UserId = DefaultUserId,
            Assigned = true,
            Training = training,
        };
        var PutAssigned = await ScheduleController.PutAssignedUser(body);
        PutAssigned?.Value?.Success.Should().BeTrue();
        var updateLinkResponse = await VehicleController.UpdateLinkVehicleTraining(new DrogeLinkVehicleTraining
        {
            RoosterTrainingId = trainingId,
            VehicleId = DefaultVehicle,
            IsSelected = true,
        });
        Assert.True(updateLinkResponse?.Value?.Success);
        return trainingId;
    }
}
