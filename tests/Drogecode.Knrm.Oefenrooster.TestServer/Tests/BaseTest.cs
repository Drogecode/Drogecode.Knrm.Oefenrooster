using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests;

public class BaseTest : IAsyncLifetime
{
    protected const string USER_NAME = "xUnit user";
    protected const string FUNCTION_DEFAULT = "xUnit default function";
    protected const string HOLIDAY_DEFAULT = "xUnit default holiday";
    protected Guid UserId { get; private set; }
    protected Guid DefaultFunction { get; private set; }
    protected Guid DefaultHoliday { get; private set; }
    protected readonly UserController UserController;
    protected readonly FunctionController FunctionController;
    protected readonly HolidayController HolidayController;
    public BaseTest(UserController userController,
        FunctionController functionController,
        HolidayController holidayController)
    {
        UserController = userController;
        FunctionController = functionController;
        HolidayController = holidayController;
        MockAuthenticatedUser(userController);
        MockAuthenticatedUser(functionController);
        MockAuthenticatedUser(holidayController);

    }

    public async Task InitializeAsync()
    {
        UserId = await AddUser(USER_NAME);
        DefaultFunction = await AddFunction(FUNCTION_DEFAULT, true);
        DefaultHoliday = await AddHoliday(HOLIDAY_DEFAULT);
    }

    protected async Task<Guid> AddUser(string name)
    {
        var result = await UserController.AddUser(new DrogeUser
        {
            Name = name
        });
        Assert.NotNull(result?.Value?.UserId);
        Assert.True(result.Value.Success);
        return result.Value.UserId ?? throw new Exception("Failed to get UserId for new test user");
    }

    protected async Task<Guid> AddFunction(string name, bool isDefault)
    {
        var result = await FunctionController.AddFunction(new DrogeFunction
        {
            Name = name,
            TrainingTarget = 2,
            Active = true,
            Default = isDefault
        });
        Assert.NotNull(result?.Value?.NewFunction);
        return result.Value.NewFunction.Id;
    }

    protected async Task<Guid> AddHoliday(string description)
    {
        var result = await HolidayController.PutHolidayForUser(new Holiday { 
            Description = description,
            ValidFrom = DateTime.Today,
            ValidUntil = DateTime.Today.AddDays(2),
        });
        Assert.NotNull(result?.Value?.Put?.Id);
        return result.Value.Put.Id;

    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected void MockAuthenticatedUser(ControllerBase controller)
    {
        var claims = new List<Claim>
        {
            new Claim("name", USER_NAME),
            new Claim("preferred_username","xUnit@drogecode.nl"),
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", DefaultSettingsHelper.IdTaco.ToString()),
            new Claim("http://schemas.microsoft.com/identity/claims/tenantid", DefaultSettingsHelper.KnrmHuizenId.ToString()),
        };
        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var fakeUser = new ClaimsPrincipal(claimsIdentity);
        var context = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = fakeUser
            }
        };
        // Then set it to controller before executing test
        controller.ControllerContext = context;
    }
}
