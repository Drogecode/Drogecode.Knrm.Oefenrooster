using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ServiceTests;

public class UserLastCalendarUpdateServiceTests : BaseTest
{
    private readonly IUserLastCalendarUpdateService _userLastCalendarUpdateService;

    public UserLastCalendarUpdateServiceTests(
        IUserLastCalendarUpdateService userLastCalendarUpdateService,
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
            preComController, vehicleController, defaultScheduleController, reportActionController, reportTrainingController, userRoleController, userLinkedMailsController,
            reportActionSharedController,
            auditController)
    {
        _userLastCalendarUpdateService = userLastCalendarUpdateService;
    }

    [Fact]
    public async Task AddOrUpdateLastUpdateUserTest()
    {
        var result = await _userLastCalendarUpdateService.AddOrUpdateLastUpdateUser(DefaultCustomerId, DefaultUserId, CancellationToken.None);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetLastUpdateUsersJustSetTest()
    {
        try
        {
            var dummyDateTime = new DateTime(2020, 9, 4, 12, 8, 1);
            DateTimeServiceMock.SetMockDateTime(dummyDateTime);
            await _userLastCalendarUpdateService.AddOrUpdateLastUpdateUser(DefaultCustomerId, DefaultUserId, CancellationToken.None);
            DateTimeServiceMock.SetMockDateTime(dummyDateTime.AddMinutes(1));
            var result = await _userLastCalendarUpdateService.GetLastUpdateUsers(CancellationToken.None);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        finally
        {
            DateTimeServiceMock.SetMockDateTime(null);
        }
    }

    [Fact]
    public async Task GetLastUpdateUsersInPeriodTest()
    {
        try
        {
            var dummyDateTime = new DateTime(2020, 9, 4, 12, 8, 1);
            DateTimeServiceMock.SetMockDateTime(dummyDateTime);
            await _userLastCalendarUpdateService.AddOrUpdateLastUpdateUser(DefaultCustomerId, DefaultUserId, CancellationToken.None);
            DateTimeServiceMock.SetMockDateTime(dummyDateTime.AddMinutes(30));
            var result = await _userLastCalendarUpdateService.GetLastUpdateUsers(CancellationToken.None);
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }
        finally
        {
            DateTimeServiceMock.SetMockDateTime(null);
        }
    }

    [Fact]
    public async Task GetLastUpdateUsersAfterPeriodTest()
    {
        try
        {
            var dummyDateTime = new DateTime(2020, 9, 4, 12, 8, 1, DateTimeKind.Utc);
            DateTimeServiceMock.SetMockDateTime(dummyDateTime);
            await _userLastCalendarUpdateService.AddOrUpdateLastUpdateUser(DefaultCustomerId, DefaultUserId, CancellationToken.None);
            DateTimeServiceMock.SetMockDateTime(dummyDateTime.AddHours(2));
            var result = await _userLastCalendarUpdateService.GetLastUpdateUsers(CancellationToken.None);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        finally
        {
            DateTimeServiceMock.SetMockDateTime(null);
        }
    }
}