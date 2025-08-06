using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.PreCom;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


// This tool is an assistance for debugging and not intended for deployment to production.
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

    if (true)
    {
        await preComClient.Login(preComUser, preComPassword);
        var futureProblems = new FutureProblems(preComClient, logger, new DateTimeProvider());
        var problems = await futureProblems.Work(NextRunMode.NextWeek);
        logger.LogInformation(problems.ToString());
    }

    if (false)
    {
        var preComWorker = new AvailabilityForUser(preComClient, logger, new DateTimeProvider());
        var piket = await preComWorker.Get([37398, 29539, 7443], DateTime.Today.AddDays(3));

        var userGroups = await preComClient.GetAllUserGroups();
        var groupInfo = await preComClient.GetAllFunctions(userGroups[0].GroupID, DateTime.Today);
        var schipper = groupInfo.ServiceFuntions.FirstOrDefault(x => x.Label.Equals("KNRM schipper"));
        var opstapper = groupInfo.ServiceFuntions.FirstOrDefault(x => x.Label.Equals("KNRM opstapper"));
        var aankOpstapper = groupInfo.ServiceFuntions.FirstOrDefault(x => x.Label.Equals("KNRM Aank. Opstapper"));

        var userId = opstapper?.Users.FirstOrDefault(x => x.FullName == "HUI Taco Droogers")?.UserID ?? -1; //37398
    }
    
    //await preComClient.TestGetA(userId, userGroups[0].GroupID, DateTime.Today, DateTime.Today.AddDays(7));
    //await preComClient.TestGetB(userGroups[0].GroupID, DateTime.Today, DateTime.Today.AddDays(3));
    //await preComClient.TestGetC(DateTime.Today, DateTime.Today.AddDays(3));
}
catch (Exception ex)
{
#if DEBUG
    Debugger.Break();
#endif
}