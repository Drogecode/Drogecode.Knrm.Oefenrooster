using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Server.Services;
using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.SharedForTests.Services;
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
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IFunctionService, FunctionService>();
        services.AddScoped<IHolidayService, HolidayService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<ILinkUserRoleService, LinkUserRoleService>();
        services.AddScoped<IPreComService, PreComService>();
        services.AddScoped<IReportActionSharedService, ReportActionSharedService>();
        services.AddScoped<IReportActionService, ReportActionService>();
        services.AddScoped<IReportTrainingService, ReportTrainingService>();
        services.AddScoped<IGraphService, GraphServiceMock>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<ITrainingTypesService, TrainingTypesService>();
        services.AddScoped<IUserGlobalService, UserGlobalService>();
        services.AddScoped<IUserLastCalendarUpdateService, UserLastCalendarUpdateService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IUserSettingService, UserSettingService>();
        services.AddScoped<IUserLinkedMailsService, UserLinkMailsService>();
        services.AddScoped<IUserLinkCustomerService, UserLinkCustomerService>();
        services.AddScoped<ICustomerSettingService, CustomerSettingService>();
        services.AddScoped<IDefaultScheduleService, DefaultScheduleService>();

        services.AddScoped<AuditController>();
        services.AddScoped<DayItemController>();
        services.AddScoped<MonthItemController>();
        services.AddScoped<ConfigurationController>();
        services.AddScoped<CustomerController>();
        services.AddScoped<FunctionController>();
        services.AddScoped<HolidayController>();
        services.AddScoped<MenuController>();
        services.AddScoped<PreComController>();
        services.AddScoped<ReportActionSharedController>();
        services.AddScoped<ReportActionController>();
        services.AddScoped<ReportTrainingController>();
        services.AddScoped<ScheduleController>();
        services.AddScoped<TrainingTypesController>();
        services.AddScoped<UserGlobalController>();
        services.AddScoped<UserController>();
        services.AddScoped<UserRoleController>();
        services.AddScoped<UserLinkMailsController>();
        services.AddScoped<UserLinkCustomerController>();
        services.AddScoped<VehicleController>();
        services.AddScoped<DefaultScheduleController>();

        services.AddScoped<PreComHub>();
        services.AddScoped<RefreshHub>();

        services.AddScoped<TestService>();

        services.AddLogging(lb => lb.AddXunitOutput());
    }
}
