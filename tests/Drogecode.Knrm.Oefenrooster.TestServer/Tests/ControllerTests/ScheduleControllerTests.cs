using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
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
    private readonly ScheduleController _scheduleController;

    public ScheduleControllerTests(ScheduleController scheduleController
        , UserController userController) : base(userController)
    {
        _scheduleController = scheduleController;
        MockAuthenticatedUser(_scheduleController);
    }

    [Fact]
    public async Task AddTrainingTest()
    {
        var body = new EditTraining
        {
            Name = "Test",
            Date = DateTime.UtcNow,
            TimeStart = new TimeOnly(12, 50).ToTimeSpan(),
            TimeEnd = new TimeOnly(13, 40).ToTimeSpan(),
        };
        var result = await _scheduleController.AddTraining(body);
        Assert.NotNull(result?.Value?.NewId);
        Assert.True(result.Value.Success);
        var tody = DateTime.Today;
        var yesterday = DateTime.Today.AddDays(-1);
        var tomorrow = DateTime.Today.AddDays(1);
        var getResult = await _scheduleController.ForAll(tody.Month, yesterday.Year, yesterday.Month, yesterday.Day, tomorrow.Year, tomorrow.Month, tomorrow.Day);
        Assert.NotNull(getResult?.Value?.Success);
        Assert.True(getResult.Value.Success);
    }

    [Fact]
    public async Task test()
    {
        var result = await _scheduleController.GetTrainingTypes();
        Assert.NotNull(result);
    }
}
