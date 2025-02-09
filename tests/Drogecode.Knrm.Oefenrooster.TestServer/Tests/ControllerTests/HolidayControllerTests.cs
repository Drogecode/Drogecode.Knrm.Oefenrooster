using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class HolidayControllerTests : BaseTest
{
    public HolidayControllerTests(
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
    public async Task AddHolidayTest()
    {
        var result = await HolidayController.PutHolidayForUser(new Holiday
        {
            Description = "AddHolidayTest",
            ValidFrom = DateTime.Today,
            ValidUntil = DateTime.Today.AddDays(2),
        });
        Assert.NotNull(result?.Value?.Put);
        result.Value.Put.ValidFrom.Should().Be(DateTime.Today);
        result.Value.Put.ValidUntil.Should().Be(DateTime.Today.AddDays(2));
    }
    [Fact]
    public async Task AddHolidayPastFromTest()
    {
        var result = await HolidayController.PutHolidayForUser(new Holiday
        {
            Description = "AddHolidayPastFromTest",
            ValidFrom = DateTime.Today.AddDays(-1),
            ValidUntil = DateTime.Today.AddDays(2),
        });
        Assert.NotNull(result?.Value?.Put);
        Assert.NotNull(result.Value.Put.ValidFrom);
        result.Value.Put.ValidFrom!.Value.Date.Should().Be(DateTime.Today);
        result.Value.Put.ValidUntil.Should().Be(DateTime.Today.AddDays(2));
    }
    [Fact]
    public async Task AddHolidayPastFromAndTillTest()
    {
        var result = await HolidayController.PutHolidayForUser(new Holiday
        {
            Description = "AddHolidayPastFromAndTillTest",
            ValidFrom = DateTime.Today.AddDays(-3),
            ValidUntil = DateTime.Today.AddDays(-1),
        });
        Assert.NotNull(result?.Value?.Success);
        Assert.False(result.Value.Success);
    }
    [Fact]
    public async Task AddHolidayTillBeforeFromTest()
    {
        var result = await HolidayController.PutHolidayForUser(new Holiday
        {
            Description = "AddHolidayTillBeforeFromTest",
            ValidFrom = DateTime.Today.AddDays(3),
            ValidUntil = DateTime.Today.AddDays(1),
        });
        Assert.NotNull(result?.Value?.Success);
        Assert.False(result.Value.Success);
    }

    [Fact]
    public async Task GetTest()
    {
        var result = await HolidayController.Get(DefaultHoliday);
        Assert.NotNull(result?.Value?.Holiday);
        Assert.True(result.Value.Success);
        result.Value.Holiday.Id.Should().Be(DefaultHoliday);
        result.Value.Holiday.Description.Should().Be(HOLIDAY_DEFAULT);
    }

    [Fact]
    public async Task GetAllTest()
    {
        var newHoliday = await AddHoliday("GetAllTest");
        var result = await HolidayController.GetAll();
        Assert.NotNull(result?.Value?.Holidays);
        result.Value.Holidays.Should().Contain(x => x.Id == DefaultHoliday);
        result.Value.Holidays.Should().Contain(x => x.Id == newHoliday);
    }

    [Fact]
    public async Task GetAllFutureTest()
    {
        var mockedDateTime = new DateTime(2024, 9, 4, 15, 0, 0);
        DateTimeServiceMock.SetMockDateTime(mockedDateTime.AddDays(-10));
        var newHoliday1 = await AddHoliday("GetAllFutureTest1", 11, 20);
        var newHoliday2 = await AddHoliday("GetAllFutureTest2", 3, 15);
        var newHoliday3 = await AddHoliday("GetAllFutureTest3", 6, 8);
        var newHoliday4 = await AddHoliday("GetAllFutureTest4", 50, 60);
        var newHoliday5 = await AddHoliday("GetAllFutureTest5", 30, 50);
        DateTimeServiceMock.SetMockDateTime(mockedDateTime);
        var result = await HolidayController.GetAllFuture(30, false);
        Assert.NotNull(result?.Value?.Holidays);
        result.Value.Holidays.Should().NotContain(x => x.Id == DefaultHoliday);
        result.Value.Holidays.Should().Contain(x => x.Id == newHoliday1);
        result.Value.Holidays.Should().Contain(x => x.Id == newHoliday2);
        result.Value.Holidays.Should().NotContain(x => x.Id == newHoliday3);
        result.Value.Holidays.Should().NotContain(x => x.Id == newHoliday4);
        result.Value.Holidays.Should().Contain(x => x.Id == newHoliday5);
    }

    [Fact]
    public async Task DeleteTest()
    {
        var result = await HolidayController.Delete(DefaultHoliday);
        Assert.NotNull(result?.Value?.Success);
        Assert.True(result.Value.Success);
        var resultGet = await HolidayController.Get(DefaultHoliday);
        Assert.NotNull(resultGet?.Value?.Success);
        Assert.False(resultGet.Value.Success);
    }

    [Fact]
    public async Task DeleteFullPastTest()
    {
        var mockedDateTime = new DateTime(2024, 9, 4, 15, 0, 0);
        DateTimeServiceMock.SetMockDateTime(mockedDateTime);
        var holiday = new Holiday
        {
            Description = "DeleteFullPastTest",
            ValidFrom = mockedDateTime.AddDays(1),
            ValidUntil = mockedDateTime.AddDays(5),
        };
        var resultPut = await HolidayController.PutHolidayForUser(holiday);
        Assert.NotNull(resultPut?.Value?.Put);
        Assert.True(resultPut.Value.Success);
        DateTimeServiceMock.SetMockDateTime(mockedDateTime.AddDays(10));
        var result = await HolidayController.Delete(resultPut.Value.Put.Id);
        Assert.NotNull(result?.Value?.Success);
        Assert.False(result!.Value!.Success);
        var resultGet = await HolidayController.Get(resultPut.Value.Put.Id);
        Assert.NotNull(resultGet?.Value?.Holiday);
        Assert.True(resultGet.Value.Success);
        resultGet.Value.Holiday.ValidFrom.Should().Be(holiday.ValidFrom);
        resultGet.Value.Holiday.ValidUntil.Should().Be(holiday.ValidUntil);
    }

    [Fact]
    public async Task DeleteStartPastTest()
    {
        var mockedDateTime = new DateTime(2024, 9, 4, 15, 0, 0);
        DateTimeServiceMock.SetMockDateTime(mockedDateTime);
        var holiday = new Holiday
        {
            Description = "DeleteStartPastTest",
            ValidFrom = mockedDateTime.AddDays(1),
            ValidUntil = DateTime.Today.AddDays(5),
        };
        var resultPut = await HolidayController.PutHolidayForUser(holiday);
        Assert.NotNull(resultPut?.Value?.Put);
        Assert.True(resultPut.Value.Success);
        DateTimeServiceMock.SetMockDateTime(mockedDateTime.AddDays(10));
        var result = await HolidayController.Delete(resultPut.Value.Put.Id);
        Assert.NotNull(result?.Value?.Success);
        Assert.True(result.Value.Success);
        var resultGet = await HolidayController.Get(resultPut.Value.Put.Id);
        Assert.NotNull(resultGet?.Value?.Holiday?.ValidUntil);
        Assert.True(resultGet.Value.Success);
        resultGet.Value.Holiday.ValidFrom.Should().Be(holiday.ValidFrom);
        resultGet.Value.Holiday.ValidUntil.Value.Date.Should().Be(mockedDateTime.AddDays(10).Date);
    }
}
