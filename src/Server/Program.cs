using Drogecode.Knrm.Oefenrooster.Server.Background;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Server.Services;
using Drogecode.Knrm.Oefenrooster.Shared.Services;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using HealthChecks.UI.Client;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});

var dbConnectionString = builder.Configuration.GetConnectionString("postgresDB");

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

builder.Services.AddDbContextPool<DataContext>(options => options.UseNpgsql(dbConnectionString));
builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions
{
    ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"],
});

builder.Services.AddHttpClient();
builder.Services.AddSingleton<PreComHub>();
builder.Services.AddSingleton<RefreshHub>();
builder.Services.AddSingleton<IDateTimeService, DateTimeService>();
builder.Services.AddScoped<IGraphService, GraphService>();

builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IDayItemService, DayItemService>();
builder.Services.AddScoped<IMonthItemService, MonthItemService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IDefaultScheduleService, DefaultScheduleService>();
builder.Services.AddScoped<IFunctionService, FunctionService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<IPreComService, PreComService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITrainingTypesService, TrainingTypesService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IUserSettingService, UserSettingService>();
builder.Services.AddScoped<ICustomerSettingService, CustomerSettingService>();

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
    "User",
    "Vehicle",
    "SharePoint",
    "Report",
    "DefaultSchedule",
    "Holiday",
    "PreCom",
    "TrainingTypes",
    "CustomerSettings",
    "UserSettings"
};
var runningInContainers = string.Equals(builder.Configuration["DOTNET_RUNNING_IN_CONTAINER"], "true");
if (!runningInContainers)
{
    // This does not work in containers.
    builder.Services.AddSwaggerGen(c =>
    {
        //c.UseInlineDefinitionsForEnums();
        c.CustomOperationIds(d => d.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor ? controllerActionDescriptor.MethodInfo.Name : d.ActionDescriptor.AttributeRouteInfo?.Name);
        groupNames.ForEach(x => c.SwaggerDoc(x, new OpenApiInfo { Title = x.Split('.').LastOrDefault(), Version = "v1" }));

        c.DocInclusionPredicate((controllerName, apiDescription) =>
        {
            return !string.IsNullOrEmpty(apiDescription.GroupName) && string.CompareOrdinal(controllerName, apiDescription.GroupName) == 0 && groupNames.Contains(apiDescription.GroupName);
        });
    });
}
#endif

var app = builder.Build();
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

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.MapHealthChecks("/api/_health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        dbContext.Database.Migrate();
    }
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
        var asstring = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(Path.Combine("../ClientGenerator/OpenAPIs", fileName), asstring);
    }
}
#endif

app.MapRazorPages();
app.MapControllers();
app.MapHub<PreComHub>("/hub/precomhub");
app.MapHub<RefreshHub>("/hub/refresh");
app.MapFallbackToFile("index.html");
app.Run();
