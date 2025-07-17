using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Errors;
using Drogecode.Knrm.Oefenrooster.SharedForTests.Helpers;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class DefaultScheduleControllerTests : BaseTest
{
    public DefaultScheduleControllerTests(TestService testService) : base(testService)
    {
    }

    [Fact]
    public async Task PutTest()
    {
        var body = new DefaultSchedule
        {
            RoosterTrainingTypeId = Tester.DefaultTrainingType,
            WeekDay = DayOfWeek.Tuesday,
            TimeStart = new TimeOnly(11, 0).ToTimeSpan(),
            TimeEnd = new TimeOnly(14, 0).ToTimeSpan(),
            ValidFromDefault = DateTime.Today,
            ValidUntilDefault = DateTime.Today.AddDays(7),
            CountToTrainingTarget = false,
            Order = 55
        };
        var result = await Tester.DefaultScheduleController.PutDefaultSchedule(body);
        Assert.NotNull(result?.Value?.NewId);
    }

    [Fact]
    public async Task PatchTest()
    {
        var allDefaultScheduleResponse = await Tester.DefaultScheduleController.GetAllDefaultSchedule();
        Assert.NotNull(allDefaultScheduleResponse.Value?.DefaultSchedules);
        Assert.NotEmpty(allDefaultScheduleResponse.Value.DefaultSchedules);
        allDefaultScheduleResponse.Value.DefaultSchedules.Should().Contain(x => x.Id == Tester.DefaultDefaultSchedule);
        var defaultSchedule = allDefaultScheduleResponse.Value.DefaultSchedules.First(x => x.Id == Tester.DefaultDefaultSchedule);
        Assert.NotNull(defaultSchedule);
        defaultSchedule.Order.Should().Be(10);
        defaultSchedule.WeekDay.Should().Be(DayOfWeek.Monday);
        defaultSchedule.Order = 20;
        defaultSchedule.WeekDay = DayOfWeek.Tuesday;

        var patchResult = await Tester.DefaultScheduleController.PatchDefaultSchedule(defaultSchedule);
        allDefaultScheduleResponse = await Tester.DefaultScheduleController.GetAllDefaultSchedule();
        Assert.NotNull(allDefaultScheduleResponse.Value?.DefaultSchedules);
        Assert.NotEmpty(allDefaultScheduleResponse.Value.DefaultSchedules);
        allDefaultScheduleResponse.Value.DefaultSchedules.Should().Contain(x => x.Id == Tester.DefaultDefaultSchedule);
        var defaultSchedulePatched = allDefaultScheduleResponse.Value.DefaultSchedules.First(x => x.Id == Tester.DefaultDefaultSchedule);
        Assert.NotNull(defaultSchedule);
        defaultSchedule.Order.Should().Be(20);
        defaultSchedule.WeekDay.Should().Be(DayOfWeek.Tuesday);
    }

    [Fact]
    public async Task PutExistingFailsTest()
    {
        var body = new DefaultSchedule
        {
            Id = Tester.DefaultDefaultSchedule,
            RoosterTrainingTypeId = Tester.DefaultTrainingType,
            WeekDay = DayOfWeek.Tuesday,
            TimeStart = new TimeOnly(11, 0).ToTimeSpan(),
            TimeEnd = new TimeOnly(14, 0).ToTimeSpan(),
            ValidFromDefault = DateTime.Today,
            ValidUntilDefault = DateTime.Today.AddDays(7),
            CountToTrainingTarget = false,
            Order = 55
        };
        var result = await Tester.DefaultScheduleController.PutDefaultSchedule(body);
        Assert.NotNull(result?.Value);
        result.Value.Error.Should().Be(PutError.IdAlreadyExists);
        result.Value.Success.Should().BeFalse();
    }

    [Fact]
    public async Task PatchForUserTest()
    {
        var body = new PatchDefaultUserSchedule
        {
            DefaultId = Tester.DefaultDefaultSchedule,
            Availability = Availability.Available,
            ValidFromUser = DateTime.Today,
            ValidUntilUser = DateTime.Today.AddDays(7),
        };
        var result = await Tester.DefaultScheduleController.PatchDefaultScheduleForUser(body);
        Assert.NotNull(result?.Value?.Patched?.UserDefaultAvailableId);
        result!.Value!.Patched!.Availability.Should().Be(Availability.Available);
        result!.Value!.Patched!.ValidFromUser.Should().Be(DateTime.Today);
        result!.Value!.Patched!.ValidUntilUser.Should().Be(DateTime.Today.AddDays(7));
    }

    [Fact]
    public async Task PatchExistingStartedInPastTest()
    {
        Tester.DateTimeProviderMock.SetMockDateTime(DateTime.Now.AddDays(-5));
        var body = new PatchDefaultUserSchedule
        {
            DefaultId = Tester.DefaultDefaultSchedule,
            Availability = Availability.Available,
            ValidFromUser = Tester.DateTimeProviderMock.Today(),
            ValidUntilUser = Tester.DateTimeProviderMock.Today().AddDays(7),
        };
        var resultOrignal = await Tester.DefaultScheduleController.PatchDefaultScheduleForUser(body);
        Assert.NotNull(resultOrignal?.Value?.Patched?.UserDefaultAvailableId);
        Tester.DateTimeProviderMock.SetMockDateTime(null);
        body = new PatchDefaultUserSchedule
        {
            UserDefaultAvailableId = resultOrignal!.Value!.Patched!.UserDefaultAvailableId,
            DefaultId = Tester.DefaultDefaultSchedule,
            Availability = Availability.Available,
            ValidFromUser = Tester.DateTimeProviderMock.Today(),
            ValidUntilUser = Tester.DateTimeProviderMock.Today().AddDays(7),
        };
        var resultPatched = await Tester.DefaultScheduleController.PatchDefaultScheduleForUser(body);
        Assert.NotNull(resultPatched?.Value?.Patched?.UserDefaultAvailableId);
        resultPatched!.Value!.Patched!.UserDefaultAvailableId.Should().NotBe(resultOrignal!.Value!.Patched!.UserDefaultAvailableId!.Value);
    }

    [Fact]
    public async Task PatchExistingStartedInFutureTest()
    {
        var body = new PatchDefaultUserSchedule
        {
            DefaultId = Tester.DefaultDefaultSchedule,
            Availability = Availability.Available,
            ValidFromUser = Tester.DateTimeProviderMock.Today().AddDays(7),
            ValidUntilUser = Tester.DateTimeProviderMock.Today().AddDays(14),
        };
        var resultOrignal = await Tester.DefaultScheduleController.PatchDefaultScheduleForUser(body);
        Assert.NotNull(resultOrignal?.Value?.Patched?.UserDefaultAvailableId);
        body = new PatchDefaultUserSchedule
        {
            UserDefaultAvailableId = resultOrignal!.Value!.Patched!.UserDefaultAvailableId,
            DefaultId = Tester.DefaultDefaultSchedule,
            Availability = Availability.Maybe,
            ValidFromUser = Tester.DateTimeProviderMock.Today().AddDays(7),
            ValidUntilUser = Tester.DateTimeProviderMock.Today().AddDays(19),
        };
        var resultPatched = await Tester.DefaultScheduleController.PatchDefaultScheduleForUser(body);
        Assert.NotNull(resultPatched?.Value?.Patched?.UserDefaultAvailableId);
        resultPatched!.Value!.Patched!.UserDefaultAvailableId.Should().Be(resultOrignal!.Value!.Patched!.UserDefaultAvailableId);
        resultPatched!.Value!.Patched!.Availability.Should().Be(Availability.Maybe);
        resultPatched!.Value!.Patched!.ValidFromUser.Should().Be(DateTime.Today.AddDays(7));
        resultPatched!.Value!.Patched!.ValidUntilUser.Should().Be(DateTime.Today.AddDays(19));
    }

    [Fact]
    public async Task GetAllGroupsTest()
    {
        var result = await Tester.DefaultScheduleController.GetAllGroups();
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
        var result = await Tester.DefaultScheduleController.PutGroup(body);
        Assert.NotNull(result.Value?.Group?.Id);
        var resultAllGroups = await Tester.DefaultScheduleController.GetAllGroups();
        resultAllGroups.Value!.Groups!.Should().Contain(x => x.Id == result.Value!.Group!.Id);
    }

    [Fact]
    public async Task GetAllForDefaultGroupTest()
    {
        var idDefaultGroup = (await Tester.DefaultScheduleController.GetAllGroups()).Value!.Groups!.FirstOrDefault(x => x.IsDefault)!.Id;
        var allForGroup = await Tester.DefaultScheduleController.GetAllByGroupId(idDefaultGroup);
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
        var newGroup = (await Tester.DefaultScheduleController.PutGroup(body)).Value!.Group;

        var defaultSchedule = new PatchDefaultUserSchedule
        {
            GroupId = newGroup!.Id,
            UserDefaultAvailableId = null,
            DefaultId = Tester.DefaultDefaultSchedule,
            Assigned = false,
            Availability = Availability.Available,
            ValidFromUser = newGroup!.ValidFrom,
            ValidUntilUser = newGroup!.ValidUntil,
        };
        var patchResult = await Tester.DefaultScheduleController.PatchDefaultScheduleForUser(defaultSchedule);
        Assert.NotNull(patchResult.Value?.Patched?.UserDefaultAvailableId);

        var allForGroup = await Tester.DefaultScheduleController.GetAllByGroupId(newGroup.Id);
        Assert.NotNull(allForGroup.Value?.DefaultSchedules);
        Assert.NotEmpty(allForGroup.Value.DefaultSchedules);
        allForGroup.Value!.DefaultSchedules!.FirstOrDefault(x => x.Id == Tester.DefaultDefaultSchedule)!.UserSchedules.Should()
            .Contain(x => x.UserDefaultAvailableId == patchResult.Value!.Patched!.UserDefaultAvailableId);

        var idDefaultGroup = (await Tester.DefaultScheduleController.GetAllGroups()).Value!.Groups!.FirstOrDefault(x => x.IsDefault)!.Id;
        var allForDefaultGroup = await Tester.DefaultScheduleController.GetAllByGroupId(idDefaultGroup);
        allForDefaultGroup.Value!.DefaultSchedules!.FirstOrDefault(x => x.Id == Tester.DefaultDefaultSchedule)!.UserSchedules.Should()
            .NotContain(x => x.UserDefaultAvailableId == patchResult.Value!.Patched!.UserDefaultAvailableId);
    }

    [Fact]
    public async Task ConflictingDefaultsTest()
    {
        Tester.MockAuthenticatedUser(Tester.DefaultScheduleController, Tester.DefaultUserId, Tester.DefaultCustomerId);
        Tester.MockAuthenticatedUser(Tester.ScheduleController, Tester.DefaultUserId, Tester.DefaultCustomerId);
        var idDefaultGroup = (await Tester.DefaultScheduleController.GetAllGroups()).Value!.Groups!.FirstOrDefault(x => x.IsDefault)!.Id;
        var body = new DefaultGroup
        {
            Name = "ConflictingDefaultsTest",
            ValidFrom = new DateTime(2020, 6, 12),
            ValidUntil = new DateTime(2020, 12, 13),
            IsDefault = false,
        };
        var newGroup = (await Tester.DefaultScheduleController.PutGroup(body)).Value!.Group;
        var defaultSchedule = new PatchDefaultUserSchedule
        {
            GroupId = idDefaultGroup,
            UserDefaultAvailableId = null,
            DefaultId = Tester.DefaultDefaultSchedule,
            Assigned = false,
            Availability = Availability.NotAvailable,
            ValidFromUser = new DateTime(2020, 1, 12)
        };
        var patchResult = await Tester.DefaultScheduleController.PatchDefaultScheduleForUser(defaultSchedule);
        defaultSchedule.GroupId = newGroup!.Id;
        defaultSchedule.Availability = Availability.Available;
        patchResult = await Tester.DefaultScheduleController.PatchDefaultScheduleForUser(defaultSchedule);

        // default default
        var scheduledTraining = await Tester.ScheduleController.GetPlannedTrainingForDefaultDate(new DateTime(2020, 2, 10), Tester.DefaultDefaultSchedule);
        scheduledTraining.Value?.Training.Should().NotBeNull();
        scheduledTraining.Value!.Training!.PlanUsers.Should().NotBeNull();
        var user = scheduledTraining.Value.Training.PlanUsers.FirstOrDefault(x => x.UserId == Tester.DefaultUserId);
        user.Should().NotBeNull();
        user!.Availability.Should().Be(Availability.NotAvailable);

        // group default
        scheduledTraining = await Tester.ScheduleController.GetPlannedTrainingForDefaultDate(new DateTime(2020, 8, 10), Tester.DefaultDefaultSchedule);
        scheduledTraining.Value?.Training.Should().NotBeNull();
        scheduledTraining.Value!.Training!.PlanUsers.Should().NotBeNull();
        user = scheduledTraining.Value.Training.PlanUsers.FirstOrDefault(x => x.UserId == Tester.DefaultUserId);
        user.Should().NotBeNull();
        user!.Availability.Should().Be(Availability.Available);
        Tester.MockAuthenticatedUser(Tester.DefaultScheduleController, DefaultSettingsHelperMock.IdDefaultUserForTests, Tester.DefaultCustomerId);
        Tester.MockAuthenticatedUser(Tester.ScheduleController, DefaultSettingsHelperMock.IdDefaultUserForTests, Tester.DefaultCustomerId);
    }
}