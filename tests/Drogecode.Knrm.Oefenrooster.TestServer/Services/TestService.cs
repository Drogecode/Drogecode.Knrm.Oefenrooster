﻿using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportActionShared;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserGlobal;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;
using Drogecode.Knrm.Oefenrooster.SharedForTests.Helpers;
using Drogecode.Knrm.Oefenrooster.SharedForTests.Providers.Interfaces;
using Drogecode.Knrm.Oefenrooster.TestServer.Seeds;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Services;

public class TestService : IAsyncLifetime
{
    internal readonly IDateTimeProviderMock DateTimeProviderMock;
    internal const string USER_NAME = "xUnit user";
    internal const string USER_ROLE_NAME = "xUnit user role";
    internal const string FUNCTION_DEFAULT = "xUnit default function";
    internal const string HOLIDAY_DEFAULT = "xUnit default holiday";
    internal const string TRAINING_DEFAULT = "xUnit default training";
    internal const string TRAINING_TYPE_DEFAULT = "xUnit default training type";
    internal const string TRAINING_CALENDAR_MONTH = "xUnit month item";
    internal const string TRAINING_CALENDAR_DAY = "xUnit day item";
    internal const string VEHICLE_DEFAULT = "xUnit default vehicle";
    internal Guid DefaultCustomerId { get; private set; }
    internal Guid SecondaryCustomerId { get; private set; }
    internal Guid DefaultUserId { get; private set; }
    internal Guid DefaultGlobalUserId { get; private set; }
    internal Guid DefaultUserRoleId { get; private set; }
    internal Guid DefaultFunction { get; private set; }
    internal Guid DefaultHoliday { get; private set; }
    internal Guid DefaultTraining { get; private set; }
    internal Guid DefaultAssignedTraining { get; private set; }
    internal Guid DefaultTrainingType { get; private set; }
    internal Guid DefaultCalendarMonthItem { get; private set; }
    internal Guid DefaultCalendarDayItem { get; private set; }
    internal Guid DefaultVehicle { get; private set; }
    internal Guid DefaultDefaultSchedule { get; private set; }

    internal readonly DataContext DataContext;

    internal readonly ScheduleController ScheduleController;
    internal readonly UserController UserController;
    internal readonly FunctionController FunctionController;
    internal readonly LicenseController LicenseController;
    internal readonly HolidayController HolidayController;
    internal readonly TrainingTypesController TrainingTypesController;
    internal readonly DayItemController DayItemController;
    internal readonly MonthItemController MonthItemController;
    internal readonly PreComController PreComController;
    internal readonly VehicleController VehicleController;
    internal readonly DefaultScheduleController DefaultScheduleController;
    internal readonly ReportActionController ReportActionController;
    internal readonly ReportTrainingController ReportTrainingController;
    internal readonly UserRoleController UserRoleController;
    internal readonly UserLinkMailsController UserLinkMailsController;
    internal readonly ReportActionSharedController ReportActionSharedController;
    internal readonly AuditController AuditController;
    internal readonly MenuController MenuController;
    internal readonly CustomerController CustomerController;
    internal readonly UserLinkCustomerController UserLinkCustomerController;
    internal readonly UserGlobalController UserGlobalController;

    public TestService(
        DataContext dataContext,
        IDateTimeProvider dateTimeProvider,
        ScheduleController scheduleController,
        UserController userController,
        FunctionController functionController,
        LicenseController licenseController,
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
        UserLinkMailsController userLinkMailsController,
        ReportActionSharedController reportActionSharedController,
        AuditController auditController,
        MenuController menuController,
        CustomerController customerController,
        UserLinkCustomerController userLinkCustomerController,
        UserGlobalController userGlobalController)
    {
        DataContext = dataContext;
        DateTimeProviderMock = (IDateTimeProviderMock)dateTimeProvider;

        ScheduleController = scheduleController;
        UserController = userController;
        FunctionController = functionController;
        LicenseController = licenseController;       
        HolidayController = holidayController;
        TrainingTypesController = trainingTypesController;
        DayItemController = dayItemController;
        MonthItemController = monthItemController;
        PreComController = preComController;
        VehicleController = vehicleController;
        DefaultScheduleController = defaultScheduleController;
        ReportActionController = reportActionController;
        ReportTrainingController = reportTrainingController;
        UserRoleController = userRoleController;
        UserLinkMailsController = userLinkMailsController;
        ReportActionSharedController = reportActionSharedController;
        AuditController = auditController;
        MenuController = menuController;
        CustomerController = customerController;
        UserLinkCustomerController = userLinkCustomerController;
        UserGlobalController = userGlobalController;
    }

    public async Task InitializeAsync()
    {
        DateTimeProviderMock.SetMockDateTime(null);
        DefaultCustomerId = Guid.NewGuid();
        SecondaryCustomerId = Guid.NewGuid();
        SeedCustomer.Seed(DataContext, DefaultCustomerId);
        SeedCustomer.Seed(DataContext, SecondaryCustomerId);

        var defaultRoles = new List<string>
        {
            AccessesNames.AUTH_scheduler_other,
        };
        MockAuthenticatedUser(ScheduleController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(UserController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(FunctionController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(LicenseController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(HolidayController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(TrainingTypesController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(DayItemController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(MonthItemController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(PreComController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(VehicleController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(DefaultScheduleController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(ReportActionSharedController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(ReportActionController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(ReportTrainingController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(UserRoleController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(UserLinkMailsController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(AuditController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(MenuController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(CustomerController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(UserLinkCustomerController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);
        MockAuthenticatedUser(UserGlobalController, DefaultSettingsHelperMock.IdDefaultUserForTests, DefaultCustomerId, defaultRoles);

        DefaultFunction = await AddFunction(FUNCTION_DEFAULT, true);
        DefaultUserRoleId = await AddUserRole(USER_ROLE_NAME);
        DefaultUserId = await AddUser(USER_NAME);
        DefaultGlobalUserId = await AddGlobalUser(USER_NAME);
        DefaultHoliday = await AddHoliday(HOLIDAY_DEFAULT);
        DefaultTraining = await AddTraining(TRAINING_DEFAULT, false);
        DefaultAssignedTraining = await AssignTrainingToUser(DefaultTraining, DefaultUserId, true);
        DefaultTrainingType = await AddTrainingType(TRAINING_TYPE_DEFAULT, 20);
        DefaultCalendarMonthItem = await AddCalendarMonthItem(TRAINING_CALENDAR_MONTH, 3, 2020);
        DefaultCalendarDayItem = await AddCalendarDayItem(TRAINING_CALENDAR_DAY);
        DefaultVehicle = await AddVehicle(VEHICLE_DEFAULT);
        DefaultDefaultSchedule = await AddDefaultSchedule();
    }

    internal async Task<Guid> AddUser(string name)
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

    internal async Task<Guid> AddGlobalUser(string name)
    {
        var result = await UserGlobalController.PutGlobalUser(new DrogeUserGlobal
        {
            Name = name,
        });
        Assert.NotNull(result?.Value?.NewId);
        Assert.True(result.Value.Success);
        return result.Value.NewId ?? throw new Exception("Failed to get NewId for new test global user");
    }

    internal async Task<Guid> AddFunction(string name, bool isDefault)
    {
        var result = await FunctionController.AddFunction(new DrogeFunction
        {
            Name = name,
            TrainingTarget = 2,
            Active = true,
            Default = isDefault
        });
        Assert.NotNull(result?.Value?.NewId);
        return result.Value.NewId.Value;
    }

    internal async Task<Guid> AddUserRole(string name)
    {
        var result = await UserRoleController.NewUserRole(new DrogeUserRole
        {
            Name = name,
            AUTH_scheduler_other = true,
        });
        Assert.NotNull(result?.Value?.NewId);
        return result.Value.NewId.Value;
    }

    internal async Task<Guid> AddHoliday(string description, int from = 1, int until = 2)
    {
        var result = await HolidayController.PutHolidayForUser(new Holiday
        {
            Description = description,
            ValidFrom = DateTimeProviderMock.Today().AddDays(from),
            ValidUntil = DateTimeProviderMock.Today().AddDays(until),
        });
        Assert.NotNull(result?.Value?.Put?.Id);
        return result!.Value!.Put!.Id;
    }

    internal async Task<Guid> AddTraining(string name, bool countToTrainingTarget, DateTime? dateStart = null, DateTime? dateEnd = null, bool isPinned = false, string? description = null)
    {
        dateStart ??= DateTimeProviderMock.Today().AddHours(12).AddMinutes(50);
        dateEnd ??= DateTimeProviderMock.Today().AddHours(13).AddMinutes(40);
        var body = new PlannedTraining
        {
            Name = name,
            DateStart = dateStart.Value,
            DateEnd = dateEnd.Value,
            CountToTrainingTarget = countToTrainingTarget,
            IsPinned = isPinned,
            Description = description,
        };
        var result = await ScheduleController.AddTraining(body);
        Assert.NotNull(result?.Value?.NewId);
        return result.Value.NewId;
    }

    internal async Task<Guid> AssignTrainingToUser(Guid trainingId, Guid userId, bool assigned)
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

    internal async Task<Guid> AddTrainingType(string name, int order)
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

    internal async Task<Guid> AddCalendarMonthItem(string name, short? month = null, short? year = null)
    {
        month ??= short.Parse(DateTime.Today.Month.ToString());
        var body = new RoosterItemMonth
        {
            Text = name,
            Type = Shared.Enums.CalendarItemType.Custom,
            Month = month.Value,
            Year = year
        };
        var result = await MonthItemController.PutItem(body);
        Assert.NotNull(result?.Value?.NewId);
        return result.Value.NewId;
    }

    internal async Task<Guid> AddCalendarDayItem(string name, DateTime? dateStart = null, DateTime? dateEnd = null, Guid? userId = null)
    {
        dateStart ??= DateTime.Today.AddDays(7);
        var body = new RoosterItemDay
        {
            Text = name,
            Type = Shared.Enums.CalendarItemType.Custom,
            IsFullDay = true,
            DateStart = dateStart,
            DateEnd = dateEnd,
        };
        if (userId != null)
        {
            body.LinkedUsers = [new RoosterItemDayLinkedUsers { UserId = userId.Value }];
        }

        var result = await DayItemController.PutDayItem(body);
        Assert.NotNull(result?.Value?.NewId);
        return result.Value.NewId;
    }

    internal async Task<Guid> AddVehicle(string name, string code = "xUnit", bool isDefault = false, bool isActive = true)
    {
        var vehicle = new DrogeVehicle
        {
            Name = name,
            Code = code,
            IsDefault = isDefault,
            IsActive = isActive,
            Order = 4,
        };
        var result = await VehicleController.PutVehicle(vehicle);
        Assert.NotNull(result.Value?.NewId);
        return result.Value.NewId.Value;
    }

    internal async Task<Guid> AddDefaultSchedule()
    {
        var body = new DefaultSchedule
        {
            RoosterTrainingTypeId = DefaultTrainingType,
            WeekDay = DayOfWeek.Monday,
            TimeStart = new TimeOnly(11, 0).ToTimeSpan(),
            TimeEnd = new TimeOnly(14, 0).ToTimeSpan(),
            ValidFromDefault = DateTime.Today,
            ValidUntilDefault = DateTime.Today.AddDays(14),
            CountToTrainingTarget = false,
            Order = 10
        };
        var result = await DefaultScheduleController.PutDefaultSchedule(body);
        Assert.NotNull(result?.Value?.NewId);
        return result.Value!.NewId.Value;
    }

    internal async Task<Guid> AddActionShared(List<string>? types, List<string>? search, DateTime? validUntil = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var body = new ReportActionSharedConfiguration
        {
            SelectedUsers = [DefaultSettingsHelperMock.IdDefaultUserForTests],
            Types = types,
            Search = search,
            ValidUntil = validUntil,
            StartDate = startDate,
            EndDate = endDate,
        };
        var result = await ReportActionSharedController.PutReportActionShared(body);
        Assert.NotNull(result.Value?.NewId);
        result.Value.Success.Should().BeTrue();
        return result.Value.NewId.Value;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    internal void MockAuthenticatedUser(ControllerBase controller, Guid userId, Guid customerId, List<string>? roles = null)
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