﻿using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Server.Services;
using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.TestServer.Mocks.Services;
using Drogecode.Knrm.Oefenrooster.TestServer.Mocks.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Drogecode.Knrm.Oefenrooster.TestServer;
public class Setup : Xunit.Di.Setup
{
    protected override void Configure()
    {
        ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
            var reloadOnChange = hostingContext.Configuration.GetValue("hostBuilder:reloadConfigOnChange", true);

            if (hostingContext.HostingEnvironment.IsDevelopment())
                config.AddUserSecrets<Setup>(true, reloadOnChange);
        });

        ConfigureServices((context, services) =>
        {
            services.AddMemoryCache();
            services.AddDbContext<DataContext>(c => c.UseInMemoryDatabase("MyXunitDb"));

            services.AddSingleton<IDateTimeService, DateTimeServiceMock>();

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

            services.AddScoped<CalendarItemController>();
            services.AddScoped<ConfigurationController>();
            services.AddScoped<FunctionController>();
            services.AddScoped<HolidayController>();
            services.AddScoped<PreComController>();
            services.AddScoped<ScheduleController>();
            services.AddScoped<TrainingTypesController>();
            services.AddScoped<UserController>();
            services.AddScoped<VehicleController>();

            services.AddScoped<PreComHub>();

            services.AddScoped(typeof(ILogger<>), typeof(NullLogger<>));
        });
    }
}