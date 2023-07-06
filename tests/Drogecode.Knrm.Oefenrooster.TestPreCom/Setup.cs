using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Drogecode.Knrm.Oefenrooster.TestPreCom;
public class Setup : Xunit.Di.Setup
{
    protected override void Configure()
    {
        ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
            var reloadOnChange = hostingContext.Configuration.GetValue("hostBuilder:reloadConfigOnChange", true);

            if (hostingContext.HostingEnvironment.IsDevelopment())
                config.AddUserSecrets<Setup>(true, reloadOnChange);
        });

        ConfigureServices((context, services) =>
        {
            services.AddScoped(typeof(ILogger<>), typeof(NullLogger<>));
        });
    }
}