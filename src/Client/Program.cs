using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.JSInterop;
using MudBlazor.Services;
using MudExtensions.Services;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomStateProvider>();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
});
builder.Services.AddMudExtensions();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();

builder.Services.TryAddScoped<IAuthenticationClient, AuthenticationClient>();
builder.Services.TryAddScoped<ICalendarItemClient, CalendarItemClient>();
builder.Services.TryAddScoped<IConfigurationClient, ConfigurationClient>();
builder.Services.TryAddScoped<ICustomerSettingsClient, CustomerSettingsClient>();
builder.Services.TryAddScoped<IDefaultScheduleClient, DefaultScheduleClient>();
builder.Services.TryAddScoped<IFunctionClient, FunctionClient>();
builder.Services.TryAddScoped<IHolidayClient, HolidayClient>();
builder.Services.TryAddScoped<IPreComClient, PreComClient>();
builder.Services.TryAddScoped<IScheduleClient, ScheduleClient>();
builder.Services.TryAddScoped<ISharePointClient, SharePointClient>();
builder.Services.TryAddScoped<ITrainingTypesClient, TrainingTypesClient>();
builder.Services.TryAddScoped<IUserClient, UserClient>();
builder.Services.TryAddScoped<IUserSettingsClient, UserSettingsClient>();
builder.Services.TryAddScoped<IVehicleClient, VehicleClient>();

builder.Services.TryAddScoped<CalendarItemRepository>();
builder.Services.TryAddScoped<ConfigurationRepository>();
builder.Services.TryAddScoped<DefaultScheduleRepository>();
builder.Services.TryAddScoped<FunctionRepository>();
builder.Services.TryAddScoped<HolidayRepository>();
builder.Services.TryAddScoped<PreComRepository>();
builder.Services.TryAddScoped<ScheduleRepository>();
builder.Services.TryAddScoped<SharePointRepository>();
builder.Services.TryAddScoped<TrainingTypesRepository>();
builder.Services.TryAddScoped<UserRepository>();
builder.Services.TryAddScoped<VehicleRepository>();

builder.Services.TryAddScoped<ILocalStorageExpireService, LocalStorageExpireService>();
builder.Services.TryAddScoped<IOfflineService, OfflineService>();

builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<CustomStateProvider>());
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});
builder.Services.AddHttpClient("Long", c =>
{
    c.Timeout = TimeSpan.FromSeconds(250);
});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var host = builder.Build();

CultureInfo culture;
var js = host.Services.GetRequiredService<IJSRuntime>();
var result = await js.InvokeAsync<string>("blazorCulture.get");

if (result != null)
{
    culture = new CultureInfo(result);
}
else
{
    culture = new CultureInfo("nl-NL");
    await js.InvokeVoidAsync("blazorCulture.set", "nl-NL");
}

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();
