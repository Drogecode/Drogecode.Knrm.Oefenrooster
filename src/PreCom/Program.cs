using Drogecode.Knrm.Oefenrooster.PreCom;
using Drogecode.Knrm.Oefenrooster.PreCom.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddFilter("Microsoft", LogLevel.Warning)
        .AddFilter("System", LogLevel.Warning)
        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
        .AddConsole();
});
ILogger logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("Example log message", logger);

try
{
    var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json").AddEnvironmentVariables().Build();
    using HttpClient client = new();
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
    client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

    var preComClient = new PreComClient(client, "drogecode", logger);
    var sleeper = new Sleeper(logger);
    int failerCount = 0;
    DateTime lastRun = DateTime.MinValue;
    NextRunMode nextRunMode = NextRunMode.NextWeek;

    while (true)
    {
        try
        {
            logger.LogInformation("Next run of type '{NextRunMode}'", nextRunMode);
            if (nextRunMode != NextRunMode.None)
            {
                var user = configuration.GetRequiredSection("PreCom")["User"];
                var passwoord = configuration.GetRequiredSection("PreCom")["Passwoord"];
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(passwoord))
                {
                    logger.LogError("user name '{userNull}' or passwoord '{passwoordNull}' null or empty", string.IsNullOrEmpty(user), string.IsNullOrEmpty(passwoord));
                    break;
                }
                await preComClient.Login(user, passwoord);
                var worker = new PreComWorker(preComClient, logger);
                var message = await worker.Work(nextRunMode);
                if (!string.IsNullOrEmpty(message))
                {
                    // Send message frouw WhatsApp
                }
            }
            else
            {
                await sleeper.SleepShort();
            }

            lastRun = DateTime.Now;
            failerCount = 0;
        }
        catch (Exception ex) // Only place to catch unspecified exceptions!
        {
            logger.LogError(ex, "failer in main loop");
            failerCount *= failerCount;
            if (failerCount == 0)
                failerCount = 1;
        }
        finally
        {
            if (failerCount > 0)
            {
                var sleepCount = (failerCount) * 60;
                while (sleepCount > 0)
                {
                    await Task.Delay(1000);
                    sleepCount--;
                }
            }
            nextRunMode = await sleeper.Sleep(lastRun);
        }
    }

}
catch (Exception ex)
{
    logger.LogError(ex, "failer in base!");
}