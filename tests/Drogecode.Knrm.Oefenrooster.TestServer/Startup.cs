﻿using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Server.Services;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.TestServer.Mocks.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Drogecode.Knrm.Oefenrooster.TestServer;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddDbContext<DataContext>(c => c.UseInMemoryDatabase("MyXunitDb"));

        services.AddScoped<IDateTimeService, DateTimeServiceMock>();

        services.AddScoped<ICalendarItemService, CalendarItemService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IConfigurationService, ConfigurationService>();
        services.AddScoped<IFunctionService, FunctionService>();
        services.AddScoped<IHolidayService, HolidayService>();
        services.AddScoped<IPreComService, PreComService>();
        services.AddScoped<IGraphService, GraphService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<ITrainingTypesService, TrainingTypesService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IUserSettingService, UserSettingService>();
        services.AddScoped<ICustomerSettingService, CustomerSettingService>();
        services.AddScoped<IDefaultScheduleService, DefaultScheduleService>();

        services.AddScoped<CalendarItemController>();
        services.AddScoped<ConfigurationController>();
        services.AddScoped<FunctionController>();
        services.AddScoped<HolidayController>();
        services.AddScoped<PreComController>();
        services.AddScoped<ScheduleController>();
        services.AddScoped<TrainingTypesController>();
        services.AddScoped<UserController>();
        services.AddScoped<VehicleController>();
        services.AddScoped<DefaultScheduleController>();

        services.AddScoped<PreComHub>();

        services.AddScoped(typeof(ILogger<>), typeof(NullLogger<>));
    }
}
