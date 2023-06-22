using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class HolidayControllerTests : BaseTest
{
    public HolidayControllerTests(UserController userController,
    FunctionController functionController,
    HolidayController holidayController) : base(userController, functionController, holidayController)
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
        var newHoliday = await AddHoliday("GetAll");
        var result = await HolidayController.GetAll();
        Assert.NotNull(result?.Value?.Holidays);
        result.Value.Holidays.Should().Contain(x=>x.Id == DefaultHoliday);
        result.Value.Holidays.Should().Contain(x=>x.Id == newHoliday);
    }

    [Fact]
    public async Task DeleteTest()
    {
        var result = await HolidayController.Delete(DefaultHoliday);
        Assert.NotNull(result?.Value?.Success);
        Assert.True(result.Value.Success);
    }
}
