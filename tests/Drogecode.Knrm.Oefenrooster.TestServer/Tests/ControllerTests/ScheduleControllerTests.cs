using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class ScheduleControllerTests : BaseTest
{
    public ScheduleControllerTests(
        ScheduleController scheduleController,
        FunctionController functionController,
        UserController userController,
        HolidayController holidayController,
        TrainingTypesController trainingTypesController,
        CalendarItemController calendarItemController) :
        base(scheduleController, userController, functionController, holidayController, trainingTypesController, calendarItemController)
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
        var getResult = await ScheduleController.ForAll(tody.Month, yesterday.Year, yesterday.Month, yesterday.Day, tomorrow.Year, tomorrow.Month, tomorrow.Day);
        Assert.NotNull(getResult?.Value?.Success);
        Assert.True(getResult.Value.Success);
    }

    [Fact]
    public async Task FailAssignTrainingTest()
    {
        var user = (await UserController.GetById(UserId))!.Value!.User;
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
        var user = (await UserController.GetById(UserId))!.Value!.User;
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
        var result = await ScheduleController.AllTrainingsForUser(UserId);
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
        await AssignTrainingToUser(training1, UserId, true);
        await AssignTrainingToUser(training2, UserId, true);
        await AssignTrainingToUser(training3, UserId, true);
        await AssignTrainingToUser(training4, UserId, false);
        await AssignTrainingToUser(training5, UserId, true);
        await AssignTrainingToUser(training6, UserId, true);
        var result = await ScheduleController.AllTrainingsForUser(UserId);
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
            DateStart = DateTime.Today.AddHours(12).AddMinutes(50),
            DateEnd = DateTime.Today.AddHours(13).AddMinutes(40),
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
    public async Task PatchScheduleForUserTest()
    {
        var training = (await ScheduleController.GetTrainingById(DefaultTraining))?.Value?.Training;
        Assert.NotNull(training);
        training.Availabilty.Should().NotBe(Availabilty.Available);
        training.Availabilty = Availabilty.Available;
        var patchedResult = await ScheduleController.PatchScheduleForUser(training);
        Assert.NotNull(patchedResult?.Value);
        Assert.True(patchedResult.Value.Success);
        var trainingAfterPatch = (await ScheduleController.GetTrainingById(DefaultTraining))?.Value?.Training;
        Assert.NotNull(trainingAfterPatch);
        trainingAfterPatch.Availabilty.Should().Be(Availabilty.Available);
    }

    [Fact]
    public async Task PatchScheduleForUserPastTest()
    {
        var triningInPastId = await AddTraining("PatchScheduleForUserPastTest", false, DateTime.Today.AddDays(-1).AddHours(10), DateTime.Today.AddDays(-1).AddHours(12));
        var training = (await ScheduleController.GetTrainingById(triningInPastId))?.Value?.Training;
        Assert.NotNull(training);
        training.Availabilty.Should().NotBe(Availabilty.Available);
        training.Availabilty = Availabilty.Available;
        var patchedResult = await ScheduleController.PatchScheduleForUser(training);
        Assert.NotNull(patchedResult?.Value);
        Assert.False(patchedResult.Value.Success);
        var trainingAfterPatch = (await ScheduleController.GetTrainingById(DefaultTraining))?.Value?.Training;
        Assert.NotNull(trainingAfterPatch);
        trainingAfterPatch.Availabilty.Should().Be(null);
    }
}
