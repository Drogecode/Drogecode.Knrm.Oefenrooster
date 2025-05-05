using Microsoft.Extensions.Configuration;

namespace Drogecode.Knrm.Oefenrooster.Playwright;

public class BaseTest : PageTest
{
    protected readonly IConfigurationRoot _appSettings;
    protected readonly string _baseUrl;

    protected BaseTest()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json", optional: false)
            .AddJsonFile("appsettings.withsecrets.json", optional: true)
            .Build();

        _appSettings = config;
        _baseUrl = config.GetValue<string>("BaseUrl")!;
    }
}