using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Controllers.Obsolite;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Server.Services;
using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.TestServer.Mocks.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.DependencyInjection.Logging;

namespace Drogecode.Knrm.Oefenrooster.TestServer;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddDbContext<DataContext>(c => c.UseInMemoryDatabase("MyXunitDb"));

        services.AddScoped<IDateTimeService, DateTimeServiceMock>();

        services.AddScoped<IDayItemService, DayItemService>();
        services.AddScoped<IMonthItemService, MonthItemService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IConfigurationService, ConfigurationService>();
        services.AddScoped<IFunctionService, FunctionService>();
        services.AddScoped<IHolidayService, HolidayService>();
        services.AddScoped<IPreComService, PreComService>();
        services.AddScoped<IGraphService, GraphServiceMock>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<ITrainingTypesService, TrainingTypesService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IUserSettingService, UserSettingService>();
        services.AddScoped<ICustomerSettingService, CustomerSettingService>();
        services.AddScoped<IDefaultScheduleService, DefaultScheduleService>();

        services.AddScoped<DayItemController>();
        services.AddScoped<MonthItemController>();
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

        services.AddLogging(lb => lb.AddXunitOutput());
    }
}
