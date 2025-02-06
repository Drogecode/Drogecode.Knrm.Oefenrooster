using Bunit.TestDoubles;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Drogecode.Knrm.Oefenrooster.TestClient.Mocks;
using Drogecode.Knrm.Oefenrooster.TestClient.Mocks.MockClients;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Drogecode.Knrm.Oefenrooster.TestClient;


public abstract class BlazorTestBase : TestContext
{
    public BlazorTestBase()
    {
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("TEST USER");
        authContext.SetClaims(new Claim("http://schemes.random.net/identity/upn", "TEST USER"));
        authContext.SetRoles(AccessesNames.AUTH_scheduler_description_read);

        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<NavigationManager>(new MockNavigationManager());
        Services.AddMudServices(options =>
        {
            options.SnackbarConfiguration.ShowTransitionDuration = 0;
            options.SnackbarConfiguration.HideTransitionDuration = 0;
            options.PopoverOptions.CheckForPopoverProvider = false;
        });
        Services.AddScoped(sp => new HttpClient());
        Services.AddOptions();

        this.AddBlazoredLocalStorage();
        this.AddBlazoredSessionStorage();


        Services.AddScoped<IDayItemClient, DayItemClient>();
        Services.AddScoped<IMonthItemClient, MonthItemClient>();
        Services.AddScoped<IConfigurationClient, MockConfigurationClient>();
        Services.AddScoped<ICustomerSettingsClient, CustomerSettingsClient>();
        Services.AddScoped<IDefaultScheduleClient, DefaultScheduleClient>();
        Services.AddScoped<IFunctionClient, FunctionClient>();
        Services.AddScoped<IHolidayClient, HolidayClient>();
        Services.AddScoped<IPreComClient, PreComClient>();
        Services.AddScoped<IScheduleClient, ScheduleClient>();
        Services.AddScoped<ISharePointClient, SharePointClient>();
        Services.AddScoped<IScheduleClient, ScheduleClient>();
        Services.AddScoped<ITrainingTypesClient, MockTrainingTypesClient>();
        Services.AddScoped<IAuthenticationClient, MockAuthenticationClient>();
        Services.AddScoped<IUserClient, UserClient>();
        Services.AddScoped<IVehicleClient, VehicleClient>();

        Services.AddScoped<DayItemRepository>();
        Services.AddScoped<ConfigurationRepository>();
        Services.AddScoped<CustomerSettingRepository>();
        Services.AddScoped<DefaultScheduleRepository>();
        Services.AddScoped<FunctionRepository>();
        Services.AddScoped<HolidayRepository>();
        Services.AddScoped<PreComRepository>();
        Services.AddScoped<ScheduleRepository>();
        Services.AddScoped<SharePointRepository>();
        Services.AddScoped<TrainingTypesRepository>();
        Services.AddScoped<UserRepository>();
        Services.AddScoped<VehicleRepository>();

        Services.AddSingleton<ILocalStorageExpireService, LocalStorageExpireService>();
        Services.AddSingleton<ISessionExpireService, SessionExpireService>();
        Services.AddSingleton<IOfflineService, OfflineService>();
        
        Services.AddScoped<CustomStateProvider>();
        Services.AddScoped<AuthenticationStateProvider>(c => c.GetRequiredService<CustomStateProvider>());
    }

    internal static Guid Function1Id = Guid.NewGuid();
    internal static Guid Function2Id = Guid.NewGuid();
    internal static Guid Function3Id = Guid.NewGuid();
    internal static Guid Vehicle1Id = Guid.NewGuid();
    internal static Guid Vehicle2Id = Guid.NewGuid();
    internal List<DrogeFunction>? Functions = new List<DrogeFunction>
         {
             new DrogeFunction
             {
                 Id = Function1Id,
                 Name = "Test function 1",
                 Order = 1,
                 Active = true,
                 TrainingTarget = 2,
             },
             new DrogeFunction
             {
                 Id = Function2Id,
                 Name = "Test function 2",
                 Order = 2,
                 Active = true,
                 TrainingTarget = 0,
             },
             new DrogeFunction
             {
                 Id = Function3Id,
                 Name = "Test function 3",
                 Order = 2,
                 Active = false,
                 TrainingTarget = 2,
             }
         };
    internal PlannedTraining Training = new PlannedTraining
    {
        Name = "xUnit meets bUnit",
        PlanUsers = new List<PlanUser>
            {
                new PlanUser
                {
                    UserFunctionId = Function2Id,
                    PlannedFunctionId = Function1Id,
                    Name = "test user 1",
                    Assigned = true,
                },
                new PlanUser
                {
                    UserFunctionId = Function1Id,
                    PlannedFunctionId = Function1Id,
                    Name = "test user 2",
                    Assigned = false,
                    VehicleId = Vehicle2Id,
                },
                new PlanUser
                {
                    UserFunctionId = Function3Id,
                    PlannedFunctionId = Function3Id,
                    Name = "test user 3",
                    Assigned = true,
                },
                new PlanUser
                {
                    UserFunctionId = Function2Id,
                    PlannedFunctionId = Function3Id,
                    Name = "test user 4",
                    Assigned = true,
                    VehicleId = Vehicle1Id,
                },
                new PlanUser
                {
                    UserFunctionId = Function3Id,
                    PlannedFunctionId = Function1Id,
                    Name = "test user 5",
                    Assigned = true,
                    VehicleId = Vehicle2Id,
                },
            }
    };

    internal List<DrogeVehicle>? Vehicles = new List<DrogeVehicle>
    {
        new DrogeVehicle
        {
            Id = Vehicle1Id,
            IsDefault = true,
            Name = "Vehicle 1 default",
        },
        new DrogeVehicle
        {
            Id = Vehicle2Id,
            IsDefault = false,
            Name = "Vehicle 2 not default",
        },
        new DrogeVehicle
        {
            Id = Guid.NewGuid(),
            IsDefault = false,
            Name = "Vehicle 3 not selected",
        }
    };

    internal void LocalizeA(IStringLocalizer stringLocalizer, string name)
    {
        A.CallTo(() => stringLocalizer[name]).Returns(new LocalizedString(name, name));
    }
}
