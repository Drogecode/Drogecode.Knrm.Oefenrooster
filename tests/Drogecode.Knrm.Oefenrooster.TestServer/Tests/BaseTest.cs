using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
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
    protected const string TRAINING_DEFAULT = "xUnit default training";
    protected const string TRAINING_TYPE_DEFAULT = "xUnit default training type";
    protected Guid UserId { get; private set; }
    protected Guid DefaultFunction { get; private set; }
    protected Guid DefaultHoliday { get; private set; }
    protected Guid DefaultTraining { get; private set; }
    protected Guid DefaultAssignedTraining { get; private set; }
    protected Guid DefaultTrainingType { get; private set; }
    protected readonly ScheduleController ScheduleController;
    protected readonly UserController UserController;
    protected readonly FunctionController FunctionController;
    protected readonly HolidayController HolidayController;
    protected readonly TrainingTypesController TrainingTypesController;
    public BaseTest(
        ScheduleController scheduleController,
        UserController userController,
        FunctionController functionController,
        HolidayController holidayController,
        TrainingTypesController trainingTypesController)
    {
        ScheduleController = scheduleController;
        UserController = userController;
        FunctionController = functionController;
        HolidayController = holidayController;
        TrainingTypesController = trainingTypesController;
        MockAuthenticatedUser(scheduleController);
        MockAuthenticatedUser(userController);
        MockAuthenticatedUser(functionController);
        MockAuthenticatedUser(holidayController);
        MockAuthenticatedUser(trainingTypesController);

    }

    public async Task InitializeAsync()
    {
        DefaultFunction = await AddFunction(FUNCTION_DEFAULT, true);
        UserId = await AddUser(USER_NAME);
        DefaultHoliday = await AddHoliday(HOLIDAY_DEFAULT);
        DefaultTraining = await AddTraining(TRAINING_DEFAULT, false);
        DefaultAssignedTraining = await AssignTrainingToUser(DefaultTraining, UserId, true);
        DefaultTrainingType = await AddTrainingType(TRAINING_TYPE_DEFAULT, 20);
    }

    protected async Task<Guid> AddUser(string name)
    {
        var result = await UserController.AddUser(new DrogeUser
        {
            Name = name,
            UserFunctionId = DefaultFunction,
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
            ValidFrom = DateTime.Today.AddDays(1),
            ValidUntil = DateTime.Today.AddDays(2),
        });
        Assert.NotNull(result?.Value?.Put?.Id);
        return result.Value.Put.Id;
    }

    protected async Task<Guid> AddTraining(string name, bool countToTrainingTarget)
    {
        var body = new EditTraining
        {
            Name = name,
            Date = DateTime.UtcNow,
            CountToTrainingTarget = countToTrainingTarget,
            TimeStart = new TimeOnly(12, 50).ToTimeSpan(),
            TimeEnd = new TimeOnly(13, 40).ToTimeSpan(),
        };
        var result = await ScheduleController.AddTraining(body);
        Assert.NotNull(result?.Value?.NewId);
        return result.Value.NewId;
    }
    protected async Task<Guid> AssignTrainingToUser(Guid trainingId, Guid userId, bool assigned)
    {
        var user = (await UserController.GetById(userId))!.Value!.User;
        Assert.NotNull(user);
        var body = new PatchAssignedUserRequest
        {
            TrainingId = trainingId,
            User = new PlanUser
            {
                UserId = user.Id,
                Assigned = assigned,
                UserFunctionId = DefaultFunction
            },
            Training = new TrainingAdvance
            {
                TrainingId = trainingId
            }
        };
        var result = await ScheduleController.PatchAssignedUser(body);
        Assert.NotNull(result?.Value?.IdPatched);
        return result.Value.IdPatched.Value;
    }

    protected async Task<Guid> AddTrainingType(string name, int order)
    {
        var body = new PlannerTrainingType
        {
            Name = name,
            ColorLight = "#bdbdbd",
            ColorDark = "#ffffff4c",
            Order = order,
            CountToTrainingTarget = false,
            IsDefault = false,
            IsActive = true,
        };
        var result = await TrainingTypesController.PostNewTrainingType(body);
        Assert.NotNull(result?.Value?.NewId);
        Assert.True(result.Value.Success);
        return result.Value.NewId.Value;
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
