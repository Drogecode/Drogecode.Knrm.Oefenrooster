using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Errors;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class DefaultScheduleControllerTests : BaseTest
{
    public DefaultScheduleControllerTests(
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
    public async Task PutTest()
    {
        var body = new DefaultSchedule
        {
            RoosterTrainingTypeId = DefaultTrainingType,
            WeekDay = DayOfWeek.Tuesday,
            TimeStart = new TimeOnly(11, 0),
            TimeEnd = new TimeOnly(14, 0),
            ValidFromDefault = DateTime.Today,
            ValidUntilDefault = DateTime.Today.AddDays(7),
            CountToTrainingTarget = false,
            Order = 55
        };
        var result = await DefaultScheduleController.PutDefaultSchedule(body);
        Assert.NotNull(result?.Value?.DefaultSchedule?.Id);
    }

    [Fact]
    public async Task PutExistingFailsTest()
    {
        var body = new DefaultSchedule
        {
            Id = DefaultDefaultSchedule,
            RoosterTrainingTypeId = DefaultTrainingType,
            WeekDay = DayOfWeek.Tuesday,
            TimeStart = new TimeOnly(11, 0),
            TimeEnd = new TimeOnly(14, 0),
            ValidFromDefault = DateTime.Today,
            ValidUntilDefault = DateTime.Today.AddDays(7),
            CountToTrainingTarget = false,
            Order = 55
        };
        var result = await DefaultScheduleController.PutDefaultSchedule(body);
        Assert.NotNull(result?.Value);
        result.Value.Error.Should().Be(PutError.IdAlreadyExists);
        result.Value.Success.Should().BeFalse();
    }

    [Fact]
    public async Task PatchForUserTest()
    {
        var body = new PatchDefaultUserSchedule
        {
            DefaultId = DefaultDefaultSchedule,
            Availability = Availability.Available,
            ValidFromUser = DateTime.Today,
            ValidUntilUser = DateTime.Today.AddDays(7),
        };
        var result = await DefaultScheduleController.PatchDefaultScheduleForUser(body);
        Assert.NotNull(result?.Value?.Patched?.UserDefaultAvailableId);
        result!.Value!.Patched!.Availability.Should().Be(Availability.Available);
        result!.Value!.Patched!.ValidFromUser.Should().Be(DateTime.Today);
        result!.Value!.Patched!.ValidUntilUser.Should().Be(DateTime.Today.AddDays(7));
    }

    [Fact]
    public async Task PatchExistingStartedInPastTest()
    {
        DateTimeServiceMock.SetMockDateTime(DateTime.Now.AddDays(-5));
        var body = new PatchDefaultUserSchedule
        {
            DefaultId = DefaultDefaultSchedule,
            Availability = Availability.Available,
            ValidFromUser = DateTimeServiceMock.Today(),
            ValidUntilUser = DateTimeServiceMock.Today().AddDays(7),
        };
        var resultOrignal = await DefaultScheduleController.PatchDefaultScheduleForUser(body);
        Assert.NotNull(resultOrignal?.Value?.Patched?.UserDefaultAvailableId);
        DateTimeServiceMock.SetMockDateTime(null);
        body = new PatchDefaultUserSchedule
        {
            UserDefaultAvailableId = resultOrignal!.Value!.Patched!.UserDefaultAvailableId,
            DefaultId = DefaultDefaultSchedule,
            Availability = Availability.Available,
            ValidFromUser = DateTimeServiceMock.Today(),
            ValidUntilUser = DateTimeServiceMock.Today().AddDays(7),
        };
        var resultPatched = await DefaultScheduleController.PatchDefaultScheduleForUser(body);
        Assert.NotNull(resultPatched?.Value?.Patched?.UserDefaultAvailableId);
        resultPatched!.Value!.Patched!.UserDefaultAvailableId.Should().NotBe(resultOrignal!.Value!.Patched!.UserDefaultAvailableId!.Value);
    }

    [Fact]
    public async Task PatchExistingStartedInFutureTest()
    {
        var body = new PatchDefaultUserSchedule
        {
            DefaultId = DefaultDefaultSchedule,
            Availability = Availability.Available,
            ValidFromUser = DateTimeServiceMock.Today().AddDays(7),
            ValidUntilUser = DateTimeServiceMock.Today().AddDays(14),
        };
        var resultOrignal = await DefaultScheduleController.PatchDefaultScheduleForUser(body);
        Assert.NotNull(resultOrignal?.Value?.Patched?.UserDefaultAvailableId);
        body = new PatchDefaultUserSchedule
        {
            UserDefaultAvailableId = resultOrignal!.Value!.Patched!.UserDefaultAvailableId,
            DefaultId = DefaultDefaultSchedule,
            Availability = Availability.Maybe,
            ValidFromUser = DateTimeServiceMock.Today().AddDays(7),
            ValidUntilUser = DateTimeServiceMock.Today().AddDays(19),
        };
        var resultPatched = await DefaultScheduleController.PatchDefaultScheduleForUser(body);
        Assert.NotNull(resultPatched?.Value?.Patched?.UserDefaultAvailableId);
        resultPatched!.Value!.Patched!.UserDefaultAvailableId.Should().Be(resultOrignal!.Value!.Patched!.UserDefaultAvailableId);
        resultPatched!.Value!.Patched!.Availability.Should().Be(Availability.Maybe);
        resultPatched!.Value!.Patched!.ValidFromUser.Should().Be(DateTime.Today.AddDays(7));
        resultPatched!.Value!.Patched!.ValidUntilUser.Should().Be(DateTime.Today.AddDays(19));
    }

    [Fact]
    public async Task GetAllGroupsTest()
    {
        var result = await DefaultScheduleController.GetAllGroups();
        Assert.NotNull(result.Value?.Groups);
        result.Value.Groups.Count.Should().Be(1);
        result.Value.Groups.FirstOrDefault()!.IsDefault.Should().BeTrue();
    }

    [Fact]
    public async Task PutGroupTest()
    {
        var body = new DefaultGroup
        {
            Name = "PutGroupTest",
            ValidFrom = DateTime.Today.AddDays(4),
            ValidUntil = DateTime.Today.AddDays(9),
            IsDefault = false,
        };
        var result = await DefaultScheduleController.PutGroup(body);
        Assert.NotNull(result.Value?.Group?.Id);
        var resultAllGroups = await DefaultScheduleController.GetAllGroups();
        resultAllGroups.Value!.Groups!.Should().Contain(x => x.Id == result.Value!.Group!.Id);
    }

    [Fact]
    public async Task GetAllForDefaultGroupTest()
    {
        var idDefaultGroup = (await DefaultScheduleController.GetAllGroups()).Value!.Groups!.FirstOrDefault(x => x.IsDefault)!.Id;
        var allForGroup = await DefaultScheduleController.GetAllByGroupId(idDefaultGroup);
        Assert.NotEmpty(allForGroup.Value!.DefaultSchedules!);
    }

    [Fact]
    public async Task PutScheduleToNewGroupTest()
    {
        var body = new DefaultGroup
        {
            Name = "PutScheduleToNewGroupTest",
            ValidFrom = DateTime.Today.AddDays(4),
            ValidUntil = DateTime.Today.AddDays(9),
            IsDefault = false,
        };
        var newGroup = (await DefaultScheduleController.PutGroup(body)).Value!.Group;

        var defaultSchedule = new PatchDefaultUserSchedule
        {
            GroupId = newGroup!.Id,
            UserDefaultAvailableId = null,
            DefaultId = DefaultDefaultSchedule,
            Assigned = false,
            Availability = Availability.Available,
            ValidFromUser = newGroup!.ValidFrom,
            ValidUntilUser = newGroup!.ValidUntil,
        };
        var patchResult = await DefaultScheduleController.PatchDefaultScheduleForUser(defaultSchedule);
        Assert.NotNull(patchResult.Value?.Patched?.UserDefaultAvailableId);

        var allForGroup = await DefaultScheduleController.GetAllByGroupId(newGroup.Id);
        allForGroup.Value!.DefaultSchedules!.FirstOrDefault(x => x.Id == DefaultDefaultSchedule)!.UserSchedules.Should().Contain(x => x.UserDefaultAvailableId == patchResult.Value!.Patched!.UserDefaultAvailableId);

        var idDefaultGroup = (await DefaultScheduleController.GetAllGroups()).Value!.Groups!.FirstOrDefault(x => x.IsDefault)!.Id;
        var allForDefaultGroup = await DefaultScheduleController.GetAllByGroupId(idDefaultGroup);
        allForDefaultGroup.Value!.DefaultSchedules!.FirstOrDefault(x => x.Id == DefaultDefaultSchedule)!.UserSchedules.Should().NotContain(x => x.UserDefaultAvailableId == patchResult.Value!.Patched!.UserDefaultAvailableId);
    }

    [Fact]
    public async Task ConflictingDefaultsTest()
    {
        MockAuthenticatedUser(DefaultScheduleController, DefaultUserId, DefaultCustomerId);
        MockAuthenticatedUser(ScheduleController, DefaultUserId, DefaultCustomerId);
        var idDefaultGroup = (await DefaultScheduleController.GetAllGroups()).Value!.Groups!.FirstOrDefault(x => x.IsDefault)!.Id;
        var body = new DefaultGroup
        {
            Name = "ConflictingDefaultsTest",
            ValidFrom = new DateTime(2020, 6, 12),
            ValidUntil = new DateTime(2020, 12, 13),
            IsDefault = false,
        };
        var newGroup = (await DefaultScheduleController.PutGroup(body)).Value!.Group;
        var defaultSchedule = new PatchDefaultUserSchedule
        {
            GroupId = idDefaultGroup,
            UserDefaultAvailableId = null,
            DefaultId = DefaultDefaultSchedule,
            Assigned = false,
            Availability = Availability.NotAvailable,
            ValidFromUser = new DateTime(2020, 1, 12)
        };
        var patchResult = await DefaultScheduleController.PatchDefaultScheduleForUser(defaultSchedule);
        defaultSchedule.GroupId = newGroup!.Id;
        defaultSchedule.Availability = Availability.Available;
        patchResult = await DefaultScheduleController.PatchDefaultScheduleForUser(defaultSchedule);

        // default default
        var scheduledTraining = await ScheduleController.GetPlannedTrainingForDefaultDate(new DateTime(2020, 2, 10), DefaultDefaultSchedule);
        scheduledTraining.Value?.Training.Should().NotBeNull();
        scheduledTraining.Value!.Training!.PlanUsers.Should().NotBeNull();
        var user = scheduledTraining.Value.Training.PlanUsers.FirstOrDefault(x => x.UserId == DefaultUserId);
        user.Should().NotBeNull();
        user!.Availability.Should().Be(Availability.NotAvailable);

        // group default
        scheduledTraining = await ScheduleController.GetPlannedTrainingForDefaultDate(new DateTime(2020, 8, 10), DefaultDefaultSchedule);
        scheduledTraining.Value?.Training.Should().NotBeNull();
        scheduledTraining.Value!.Training!.PlanUsers.Should().NotBeNull();
        user = scheduledTraining.Value.Training.PlanUsers.FirstOrDefault(x => x.UserId == DefaultUserId);
        user.Should().NotBeNull();
        user!.Availability.Should().Be(Availability.Available);
        MockAuthenticatedUser(DefaultScheduleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        MockAuthenticatedUser(ScheduleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
    }
}
