using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
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

        services.AddHttpClient();
        services.AddScoped<IDayItemService, DayItemService>();
        services.AddScoped<IMonthItemService, MonthItemService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IConfigurationService, ConfigurationService>();
        services.AddScoped<IFunctionService, FunctionService>();
        services.AddScoped<IHolidayService, HolidayService>();
        services.AddScoped<ILinkUserRoleService, LinkUserRoleService>();
        services.AddScoped<IPreComService, PreComService>();
        services.AddScoped<IReportActionService, ReportActionService>();
        services.AddScoped<IReportTrainingService, ReportTrainingService>();
        services.AddScoped<IGraphService, GraphServiceMock>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<ITrainingTypesService, TrainingTypesService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IUserSettingService, UserSettingService>();
        services.AddScoped<IUserLinkedMailsService, UserLinkedMailsService>();
        services.AddScoped<ICustomerSettingService, CustomerSettingService>();
        services.AddScoped<IDefaultScheduleService, DefaultScheduleService>();

        services.AddScoped<DayItemController>();
        services.AddScoped<MonthItemController>();
        services.AddScoped<ConfigurationController>();
        services.AddScoped<FunctionController>();
        services.AddScoped<HolidayController>();
        services.AddScoped<PreComController>();
        services.AddScoped<ReportActionController>();
        services.AddScoped<ReportTrainingController>();
        services.AddScoped<ScheduleController>();
        services.AddScoped<TrainingTypesController>();
        services.AddScoped<UserController>();
        services.AddScoped<UserRoleController>();
        services.AddScoped<UserLinkedMailsController>();
        services.AddScoped<VehicleController>();
        services.AddScoped<DefaultScheduleController>();

        services.AddScoped<PreComHub>();
        services.AddScoped<RefreshHub>();

        services.AddLogging(lb => lb.AddXunitOutput());
    }
}
