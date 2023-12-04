﻿using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.TestServer.Mocks.Services.Interfaces;
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
    protected readonly IDateTimeServiceMock DateTimeServiceMock;
    protected const string USER_NAME = "xUnit user";
    protected const string FUNCTION_DEFAULT = "xUnit default function";
    protected const string HOLIDAY_DEFAULT = "xUnit default holiday";
    protected const string TRAINING_DEFAULT = "xUnit default training";
    protected const string TRAINING_TYPE_DEFAULT = "xUnit default training type";
    protected const string TRAINING_CALENDAR_MONTH = "xUnit month item";
    protected const string TRAINING_CALENDAR_DAY = "xUnit day item";
    protected const string VEHICLE_DEFAULT = "xUnit default vehicle";
    protected Guid DefaultCustomerId { get; private set; }
    protected Guid DefaultUserId { get; private set; }
    protected Guid DefaultFunction { get; private set; }
    protected Guid DefaultHoliday { get; private set; }
    protected Guid DefaultTraining { get; private set; }
    protected Guid DefaultAssignedTraining { get; private set; }
    protected Guid DefaultTrainingType { get; private set; }
    protected Guid DefaultCalendarMonthItem { get; private set; }
    protected Guid DefaultCalendarDayItem { get; private set; }
    protected Guid DefaultVehicle { get; private set; }
    protected Guid DefaultDefaultSchedule { get; private set; }

    protected readonly DataContext DataContext;

    protected readonly ScheduleController ScheduleController;
    protected readonly UserController UserController;
    protected readonly FunctionController FunctionController;
    protected readonly HolidayController HolidayController;
    protected readonly TrainingTypesController TrainingTypesController;
    protected readonly CalendarItemController CalendarItemController;
    protected readonly PreComController PreComController;
    protected readonly VehicleController VehicleController;
    protected readonly DefaultScheduleController DefaultScheduleController;
    public BaseTest(
        DataContext dataContext,
        IDateTimeService dateTimeService,
        ScheduleController scheduleController,
        UserController userController,
        FunctionController functionController,
        HolidayController holidayController,
        TrainingTypesController trainingTypesController,
        CalendarItemController calendarItemController,
        PreComController preComController,
        VehicleController vehicleController,
        DefaultScheduleController defaultScheduleController)
    {
        DataContext = dataContext;
        DateTimeServiceMock = (IDateTimeServiceMock)dateTimeService;

        ScheduleController = scheduleController;
        UserController = userController;
        FunctionController = functionController;
        HolidayController = holidayController;
        TrainingTypesController = trainingTypesController;
        CalendarItemController = calendarItemController;
        PreComController = preComController;
        VehicleController = vehicleController;
        DefaultScheduleController = defaultScheduleController;

        DefaultCustomerId = Guid.NewGuid();
        dataContext.Customers.Add(new DbCustomers
        {
            Id = DefaultCustomerId,
            Name = "xUnit customer",
            TimeZone = "Europe/Amsterdam",
            Created = DateTime.UtcNow
        });
        dataContext.SaveChanges();

        MockAuthenticatedUser(scheduleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        MockAuthenticatedUser(userController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        MockAuthenticatedUser(functionController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        MockAuthenticatedUser(holidayController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        MockAuthenticatedUser(trainingTypesController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        MockAuthenticatedUser(calendarItemController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        MockAuthenticatedUser(preComController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        MockAuthenticatedUser(vehicleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
        MockAuthenticatedUser(defaultScheduleController, DefaultSettingsHelper.IdTaco, DefaultCustomerId);
    }

    public async Task InitializeAsync()
    {
        DefaultFunction = await AddFunction(FUNCTION_DEFAULT, true);
        DefaultUserId = await AddUser(USER_NAME);
        DefaultHoliday = await AddHoliday(HOLIDAY_DEFAULT);
        DefaultTraining = await AddTraining(TRAINING_DEFAULT, false);
        DefaultAssignedTraining = await AssignTrainingToUser(DefaultTraining, DefaultUserId, true);
        DefaultTrainingType = await AddTrainingType(TRAINING_TYPE_DEFAULT, 20);
        DefaultCalendarMonthItem = await AddCalendarMonthItem(TRAINING_CALENDAR_MONTH);
        DefaultCalendarDayItem = await AddCalendarDayItem(TRAINING_CALENDAR_DAY);
        DefaultVehicle = await AddVehicle(VEHICLE_DEFAULT);
        DefaultDefaultSchedule = await AddDefaultSchedule();
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
        var result = await HolidayController.PutHolidayForUser(new Holiday
        {
            Description = description,
            ValidFrom = DateTime.Today.AddDays(1),
            ValidUntil = DateTime.Today.AddDays(2),
        });
        Assert.NotNull(result?.Value?.Put?.Id);
        return result!.Value!.Put!.Id;
    }

    protected async Task<Guid> AddTraining(string name, bool countToTrainingTarget, DateTime? dateStart = null, DateTime? dateEnd = null, bool isPinned = false)
    {
        dateStart ??= DateTime.Today.AddHours(12).AddMinutes(50);
        dateEnd ??= DateTime.Today.AddHours(13).AddMinutes(40);
        var body = new PlannedTraining
        {
            Name = name,
            DateStart = dateStart.Value,
            DateEnd = dateEnd.Value,
            CountToTrainingTarget = countToTrainingTarget,
            IsPinned = isPinned,
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

    protected async Task<Guid> AddCalendarMonthItem(string name, short? month = null, short? year = null)
    {
        month ??= short.Parse(DateTime.Today.Month.ToString());
        var body = new RoosterItemMonth
        {
            Text = name,
            Type = Shared.Enums.CalendarItemType.Custom,
            Month = month.Value,
            Year = year
        };
        var result = await CalendarItemController.PutMonthItem(body);
        Assert.NotNull(result?.Value?.NewId);
        return result.Value.NewId;
    }

    protected async Task<Guid> AddCalendarDayItem(string name, DateTime? dateStart = null, Guid? userId = null)
    {
        dateStart ??= DateTime.Today.AddDays(7);
        var body = new RoosterItemDay
        {
            Text = name,
            Type = Shared.Enums.CalendarItemType.Custom,
            IsFullDay = true,
            DateStart = dateStart.Value,
        };
        if (userId != null)
        {
            body.LinkedUsers = [new RoosterItemDayLinkedUsers { UserId = userId.Value }];
        }
        var result = await CalendarItemController.PutDayItem(body);
        Assert.NotNull(result?.Value?.NewId);
        return result.Value.NewId;
    }
    protected async Task<Guid> AddVehicle(string name, string code = "xUnit", bool isDefault = false, bool isActive = true)
    {
        var vehicle = new DrogeVehicle
        {
            Name = name,
            Code = code,
            IsDefault = isDefault,
            IsActive = isActive,
        };
        var result = await VehicleController.PutVehicle(vehicle);
        Assert.NotNull(result?.Value);
        return result!.Value!.Value;
    }

    protected async Task<Guid> AddDefaultSchedule()
    {
        var body = new DefaultSchedule
        {
            RoosterTrainingTypeId = DefaultTrainingType,
            WeekDay = DayOfWeek.Monday,
            TimeStart = new TimeOnly(11, 0),
            TimeEnd = new TimeOnly(14, 0),
            ValidFromDefault = DateTime.Today,
            ValidUntilDefault = DateTime.Today.AddDays(7),
            CountToTrainingTarget = false,
            Order = 10
        };
        var result = await DefaultScheduleController.PutDefaultSchedule(body);
        Assert.NotNull(result?.Value?.DefaultSchedule?.Id);
        return result!.Value!.DefaultSchedule!.Id;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected void MockAuthenticatedUser(ControllerBase controller, Guid userId, Guid customerId, List<string>? roles = null)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "xUnit@drogecode.nl"),
            new Claim("FullName", USER_NAME),
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userId.ToString()),
            new Claim("http://schemas.microsoft.com/identity/claims/tenantid", customerId.ToString()),
        };
        if (roles is not null)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }
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
