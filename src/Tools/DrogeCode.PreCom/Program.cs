using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.PreCom;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

try
{
    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var builder = new ConfigurationBuilder();
    builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environmentName}.json", optional: true);
    IConfiguration config = builder.Build();
    using HttpClient client = new();

    using var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder
            .AddFilter("Microsoft", LogLevel.Warning)
            .AddFilter("System", LogLevel.Warning)
            .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
            .AddConsole();
    });

    ILogger logger = loggerFactory.CreateLogger<Program>();
    var preComClient = new PreComClient(client, "drogecode", logger);
    var preComUser = config.GetValue<string>("PreCom:User");
    var preComPassword = config.GetValue<string>("PreCom:Password");

    await preComClient.Login(preComUser, preComPassword);
    var preComWorker = new PreComWorker(preComClient, logger, new DateTimeService());
    var problems = await preComWorker.Work(NextRunMode.TodayTomorrow);

    logger.LogInformation(problems.ToString());
}
catch (Exception ex)
{
#if DEBUG
    Debugger.Break();
#endif
}