using Drogecode.Knrm.Oefenrooster.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Globalization;
using Microsoft.JSInterop;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using MudBlazor;
using MudExtensions.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddHttpClient("Drogecode.Knrm.Oefenrooster.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
});
builder.Services.AddMudExtensions();

builder.Services.AddScoped<ConfigurationRepository>();
builder.Services.AddScoped<FunctionRepository>();
builder.Services.AddScoped<ScheduleRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<VehicleRepository>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Drogecode.Knrm.Oefenrooster.ServerAPI"));

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add(builder.Configuration.GetSection("ServerApi")["Scopes"]);
    options.ProviderOptions.LoginMode = "redirect";
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
