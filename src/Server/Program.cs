using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Server.Services;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Extensions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<ICalendarItemService, CalendarItemService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IDefaultScheduleService, DefaultScheduleService>();
builder.Services.AddScoped<IFunctionService, FunctionService>();
builder.Services.AddScoped<IGraphService, GraphService>();
builder.Services.AddScoped<IPreComService, PreComService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddDbContextPool<DataContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("postgresDB")));
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

#if DEBUG
// Only run in debug because it fails on the azure app service! (and is not necessary)
var groupNames = new List<string>
{
    "CalendarItem",
    "Configuration",
    "Function",
    "Schedule",
    "User",
    "Vehicle",
    "SharePoint",
    "DefaultSchedule",
    "PreCom"
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
app.MapFallbackToFile("index.html");

app.Run();
