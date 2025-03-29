using Drogecode.Knrm.Oefenrooster.Server.Background;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Server.Services;
using Drogecode.Knrm.Oefenrooster.Shared.Services;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using HealthChecks.UI.Client;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System.Diagnostics;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine("Start oefenrooster");

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
});

// Add services to the container.
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
builder.Services.AddDataProtection().PersistKeysToDbContext<DataContext>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});


string? dbConnectionString;
var keyVaultUri = builder.Configuration.GetValue<string>("KEYVAULTURI");
KeyVaultHelper.KeyVaultUri = keyVaultUri;
Exception? potentialException = null;
string? messagePotentialException = null;
if (string.IsNullOrWhiteSpace(keyVaultUri))
{
    dbConnectionString = builder.Configuration.GetConnectionString("postgresDB");
    if (string.IsNullOrWhiteSpace(dbConnectionString))
    {
        var dbUserName = builder.Configuration.GetValue<string>("database:username");
        var dbPassword = builder.Configuration.GetValue<string>("database:password");
        var dbUri = builder.Configuration.GetValue<string>("database:FQDN");
        var dbName = builder.Configuration.GetValue<string>("database:name");
        var dbPort = builder.Configuration.GetValue<string>("database:port");
        dbConnectionString = $"host={dbUri};port={dbPort};database={dbName};username={dbUserName};password={dbPassword}";
    }
}
else
{
    try
    {
        var dbUserName = KeyVaultHelper.GetSecret("databaseLoginUser");
        var dbPassword = KeyVaultHelper.GetSecret("databaseLoginPassword");
        var dbUri = KeyVaultHelper.GetSecret("databaseFQDN");
        var dbName = KeyVaultHelper.GetSecret("databaseName");
        var dbPort = KeyVaultHelper.GetSecret("databasePort");
        var dbNameValue = dbName?.Value;
        if (string.IsNullOrWhiteSpace(dbNameValue))
        {
            Console.WriteLine($"dbUserName = {dbUserName?.Value}");
#if DEBUG
            dbNameValue = "OefenroosterDev";
#else
            dbNameValue = "OefenroosterAcc";
#endif
        }
        dbConnectionString = $"host={dbUri?.Value};port={dbPort?.Value ?? "5432" };database={dbNameValue};username={dbUserName?.Value};password={dbPassword?.Value}";
    }
    catch (Exception ex)
    {
        potentialException = ex;
        messagePotentialException = "Exception while constructing dbConnectionString";
        Console.WriteLine("Exception while constructing dbConnectionString");
        Console.WriteLine(ex);
        dbConnectionString = builder.Configuration.GetConnectionString("postgresDB");
    }
}
/*var fixedPolicy = "fixed";
builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: fixedPolicy, options =>
    {
        options.PermitLimit = 12;
        options.Window = TimeSpan.FromSeconds(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 12;
    }));*/
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddHealthChecks()
    //.AddCheck<DatabaseHealthCheck>("postgresDB")
    .AddNpgSql(dbConnectionString ?? "nevermind");
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddDbContextPool<DataContext>(options => { options.UseNpgsql(dbConnectionString); });
builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions
{
    ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"],
});
builder.Services.AddHttpClient();
builder.Services.AddSingleton<PreComHub>();
builder.Services.AddSingleton<RefreshHub>();
builder.Services.AddSingleton<ConfigurationHub>();
builder.Services.AddSingleton<IDateTimeService, DateTimeService>();
builder.Services.AddScoped<IGraphService, GraphService>();

builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IDayItemService, DayItemService>();
builder.Services.AddScoped<IMonthItemService, MonthItemService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IDefaultScheduleService, DefaultScheduleService>();
builder.Services.AddScoped<IFunctionService, FunctionService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<ILinkUserRoleService, LinkUserRoleService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IPreComService, PreComService>();
builder.Services.AddScoped<IReportActionSharedService, ReportActionSharedService>();
builder.Services.AddScoped<IReportActionService, ReportActionService>();
builder.Services.AddScoped<IReportTrainingService, ReportTrainingService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IUserGlobalService, UserGlobalService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserLinkedMailsService, UserLinkMailsService>();
builder.Services.AddScoped<ITrainingTypesService, TrainingTypesService>();
builder.Services.AddScoped<IUserLastCalendarUpdateService, UserLastCalendarUpdateService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IUserSettingService, UserSettingService>();
builder.Services.AddScoped<ICustomerSettingService, CustomerSettingService>();
builder.Services.AddScoped<IUserLinkCustomerService, UserLinkCustomerService>();

builder.Services.AddHostedService<Worker>();

#if DEBUG
// Only run in debug because it fails on the azure app service! (and is not necessary)
var groupNames = new List<string>
{
    "Audit",
    "Authentication",
    "DayItem",
    "MonthItem",
    "Configuration",
    "Function",
    "Schedule",
    "UserGlobal",
    "User",
    "UserRole",
    "UserLinkedMails",
    "Menu",
    "Vehicle",
    "SharePoint",
    "ReportActionShared",
    "ReportAction",
    "ReportTraining",
    "DefaultSchedule",
    "Holiday",
    "PreCom",
    "TrainingTypes",
    "CustomerSettings",
    "UserSettings",
    "LinkedCustomer",
    "Customer"
};
var runningInContainers = string.Equals(builder.Configuration["DOTNET_RUNNING_IN_CONTAINER"], "true");
if (!runningInContainers)
{
    // This does not work in containers.
    builder.Services.AddSwaggerGen(c =>
    {
        //c.UseInlineDefinitionsForEnums();
        c.CustomOperationIds(
            d => d.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor ? controllerActionDescriptor.MethodInfo.Name : d.ActionDescriptor.AttributeRouteInfo?.Name);
        groupNames.ForEach(x => c.SwaggerDoc(x, new OpenApiInfo { Title = x.Split('.').LastOrDefault(), Version = "v1" }));

        c.DocInclusionPredicate((controllerName, apiDescription) =>
        {
            return !string.IsNullOrEmpty(apiDescription.GroupName) && string.CompareOrdinal(controllerName, apiDescription.GroupName) == 0 && groupNames.Contains(apiDescription.GroupName);
        });
    });
}
#endif

var app = builder.Build();
app.Logger.LogInformation("Starting app oefenrooster");
if (potentialException is not null)
{
    app.Logger.LogError(potentialException, "Found except: {messagePotentialException}", messagePotentialException);
}

app.UseResponseCompression();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
/*app.UseRateLimiter();*/
app.MapHealthChecks("/api/_health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    await dbContext.Database.MigrateAsync();
}
catch (Exception ex)
{
#if DEBUG
    Debugger.Break();
#endif
    app.Logger.LogError(ex, "Database migration failed on startup.");
}

app.UseAuthentication();
app.UseAuthorization();

#if DEBUG
if (!runningInContainers)
{
    // This does not work in containers.
    // Only run in debug because it fails on the azure app service! (and is not necessary)
    app.UseSwagger();
    // Configure the Swagger generator and create a JSON file for each controller.
    var swaggerProvider = app.Services.GetRequiredService<ISwaggerProvider>();

    foreach (var controllerName in groupNames)
    {
        var swaggerDoc = swaggerProvider.GetSwagger(controllerName);
        var fileName = $"{controllerName}.json";
        using var stream = new MemoryStream();
        swaggerDoc.SerializeAsJson(stream, Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0);
        var asString = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(Path.Combine("../ClientGenerator/OpenAPIs", fileName), asString);
    }
}
#endif

app.MapRazorPages();
app.MapControllers()/*.RequireRateLimiting(fixedPolicy)*/;
app.MapHub<PreComHub>("/hub/precomhub");
app.MapHub<RefreshHub>("/hub/refresh");
app.MapHub<ConfigurationHub>("/hub/configuration");
app.MapFallbackToFile("index.html");
await app.RunAsync();