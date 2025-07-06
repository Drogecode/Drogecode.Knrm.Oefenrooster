using Drogecode.Knrm.Oefenrooster.Playwright.Pages;
using Microsoft.Extensions.Configuration;

namespace Drogecode.Knrm.Oefenrooster.Playwright;

public abstract class BaseTest : PageTest
{
    private readonly IConfigurationRoot _appSettings;
    protected readonly string BaseUrl;

    protected string UserName;
    protected string UserPassword;

    protected BaseTest()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json", optional: false)
            .AddJsonFile("appsettings.withsecrets.json", optional: true)
            .Build();

        _appSettings = config;
        BaseUrl = ReadSetting("Playwright:BaseUrl")!;
        if (!BaseUrl.EndsWith('/'))
        {
            BaseUrl += '/';
        }
    }

    [SetUp]
    public virtual async Task SetUp()
    {
        await BaseSetUp(true);
    }

    protected async Task BaseSetUp(bool login)
    {
        UserName = ReadSetting("Playwright:Users:Basic:Name");
        UserPassword = ReadSetting("Playwright:Users:Basic:Password");
        if (login)
        {
            var loginPage = new LoginPage(Page, BaseUrl);
            await loginPage.Login(UserName, UserPassword);
        }
    }

    protected string ReadSetting(string variableName)
    {
        var setting = Environment.GetEnvironmentVariable(variableName);
        if (!string.IsNullOrEmpty(setting))
            return setting;
        setting = _appSettings.GetValue<string>(variableName);
        if (!string.IsNullOrEmpty(setting))
            return setting;
        Assert.Fail($"Setting {variableName} not found");
        throw new Exception($"Setting {variableName} not found");
    }
}