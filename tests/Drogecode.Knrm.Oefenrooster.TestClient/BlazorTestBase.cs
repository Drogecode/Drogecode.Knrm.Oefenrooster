﻿using Bunit.TestDoubles;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.TestClient.Mocks.MockClients;
using MudBlazor.Services;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.TestClient;


public abstract class BlazorTestBase : TestContext
{
    public BlazorTestBase()
    {

        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("TEST USER");
        authContext.SetClaims(new Claim("http://schemes.random.net/identity/upn", "TEST USER"));

        Services.AddMudServices(options =>
        {
            options.SnackbarConfiguration.ShowTransitionDuration = 0;
            options.SnackbarConfiguration.HideTransitionDuration = 0;
        });
        Services.AddScoped(sp => new HttpClient());
        Services.AddOptions();

        this.AddBlazoredLocalStorage();
        this.AddBlazoredSessionStorage();


        Services.AddScoped<ICalendarItemClient, CalendarItemClient>();
        Services.AddScoped<IConfigurationClient, ConfigurationClient>();
        Services.AddScoped<IDefaultScheduleClient, DefaultScheduleClient>();
        Services.AddScoped<IFunctionClient, FunctionClient>();
        Services.AddScoped<IHolidayClient, HolidayClient>();
        Services.AddScoped<IPreComClient, PreComClient>();
        Services.AddScoped<IScheduleClient, ScheduleClient>();
        Services.AddScoped<ISharePointClient, SharePointClient>();
        Services.AddScoped<ITrainingTypesClient, MockTrainingTypesClient>();
        Services.AddScoped<IUserClient, UserClient>();
        Services.AddScoped<IVehicleClient, VehicleClient>();

        Services.AddScoped<CalendarItemRepository>();
        Services.AddScoped<ConfigurationRepository>();
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
        Services.AddSingleton<IOfflineService, OfflineService>();
    }

    internal static Guid Function1Id = Guid.NewGuid();
    internal static Guid Function2Id = Guid.NewGuid();
    internal static Guid Function3Id = Guid.NewGuid();
    internal List<DrogeFunction>? Functions = new List<DrogeFunction>
         {
             new DrogeFunction
             {
                 Id = Function1Id,
                 Name = "Test function 1",
                 Order = 1,
                 Active = true,
             },
             new DrogeFunction
             {
                 Id = Function2Id,
                 Name = "Test function 2",
                 Order = 2,
                 Active = true,
             },
             new DrogeFunction
             {
                 Id = Function3Id,
                 Name = "Test function 3",
                 Order = 2,
                 Active = false,
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
                },
                new PlanUser
                {
                    UserFunctionId = Function3Id,
                    PlannedFunctionId = Function1Id,
                    Name = "test user 5",
                    Assigned = true,
                },
            }
    };
}
