﻿using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Services;
using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;

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

            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<IFunctionService, FunctionService>();
            services.AddScoped<IGraphService, GraphService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IVehicleService, VehicleService>();

            services.AddScoped<ConfigurationController>();
            services.AddScoped<ScheduleController>();
            services.AddScoped<UserController>();

            services.AddScoped(typeof(ILogger<>), typeof(NullLogger<>));
        });
    }
}