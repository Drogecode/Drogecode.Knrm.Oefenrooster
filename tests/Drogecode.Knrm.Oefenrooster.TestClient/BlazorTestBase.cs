using AutoFixture;
using Bunit;
using MudBlazor.Services;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using FluentAssertions.Common;
using Drogecode.Knrm.Oefenrooster.TestClient.Mocks.MockClients;

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
}
