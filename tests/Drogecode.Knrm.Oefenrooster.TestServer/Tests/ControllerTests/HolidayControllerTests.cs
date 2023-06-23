using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.TestServer.Mocks.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class HolidayControllerTests : BaseTest
{
    private readonly IDateTimeServiceMock _dateTimeServiceMock;
    public HolidayControllerTests(
        ScheduleController scheduleController,
        IDateTimeService dateTimeServiceMock,
        UserController userController,
        FunctionController functionController,
        HolidayController holidayController) : base(scheduleController, userController, functionController, holidayController)
    {
        _dateTimeServiceMock = (IDateTimeServiceMock)dateTimeServiceMock;
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
        result.Value.Put.ValidFrom.Value.Date.Should().Be(DateTime.Today);
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
        _dateTimeServiceMock.SetMockDateTime(DateTime.Now.AddDays(-5));
        var holiday = new Holiday
        {
            Description = "DeleteFullPastTest",
            ValidFrom = DateTime.Today.AddDays(-4),
            ValidUntil = DateTime.Today.AddDays(-1),
        };
        var resultPut = await HolidayController.PutHolidayForUser(holiday);
        Assert.NotNull(resultPut?.Value?.Put);
        Assert.True(resultPut.Value.Success);
        _dateTimeServiceMock.SetMockDateTime(null);
        var result = await HolidayController.Delete(resultPut.Value.Put.Id);
        Assert.NotNull(result?.Value?.Success);
        Assert.False(result.Value.Success);
        var resultGet = await HolidayController.Get(resultPut.Value.Put.Id);
        Assert.NotNull(resultGet?.Value?.Holiday);
        Assert.True(resultGet.Value.Success);
        resultGet.Value.Holiday.ValidFrom.Should().Be(holiday.ValidFrom);
        resultGet.Value.Holiday.ValidUntil.Should().Be(holiday.ValidUntil);
    }

    [Fact]
    public async Task DeleteStartPastTest()
    {
        _dateTimeServiceMock.SetMockDateTime(DateTime.Now.AddDays(-5));
        var holiday = new Holiday
        {
            Description = "DeleteStartPastTest",
            ValidFrom = DateTime.Today.AddDays(-4),
            ValidUntil = DateTime.Today.AddDays(2),
        };
        var resultPut = await HolidayController.PutHolidayForUser(holiday);
        Assert.NotNull(resultPut?.Value?.Put);
        Assert.True(resultPut.Value.Success);
        _dateTimeServiceMock.SetMockDateTime(null);
        var result = await HolidayController.Delete(resultPut.Value.Put.Id);
        Assert.NotNull(result?.Value?.Success);
        Assert.True(result.Value.Success);
        var resultGet = await HolidayController.Get(resultPut.Value.Put.Id);
        Assert.NotNull(resultGet?.Value?.Holiday?.ValidUntil);
        Assert.True(resultGet.Value.Success);
        resultGet.Value.Holiday.ValidFrom.Should().Be(holiday.ValidFrom);
        resultGet.Value.Holiday.ValidUntil.Value.Date.Should().Be(DateTime.Today);
    }
}
