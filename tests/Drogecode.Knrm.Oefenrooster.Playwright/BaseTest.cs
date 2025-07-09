using Drogecode.Knrm.Oefenrooster.Playwright.Pages;
using Microsoft.Extensions.Configuration;
using NUnit.Framework.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.Playwright;

public abstract class BaseTest : PageTest
{
    private readonly IConfigurationRoot _appSettings;
    private List<string> _consoleLogs = new();
    protected readonly string BaseUrl;

    protected string UserName;
    protected string UserPassword;

    protected BaseTest()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.withsecrets.json", optional: true)
            .Build();

        _appSettings = config;
        BaseUrl = ReadSetting("Playwright__BaseUrl")!;
        if (!BaseUrl.EndsWith('/'))
        {
            BaseUrl += '/';
        }
        Console.WriteLine($"BaseUrl = `{BaseUrl}`");
    }

    [SetUp]
    public virtual async Task SetUp()
    {
        await BaseSetUp(true);
    }
    
    [TearDown]
    public async Task TearDown()
    {
        var outcome = TestContext.CurrentContext.Result.Outcome.Status;
        if (outcome == TestStatus.Failed)
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var dir = Path.Combine("playwright-debug", testName);
            Directory.CreateDirectory(dir);

            // Screenshot
            await Page.ScreenshotAsync(new() { Path = Path.Combine(dir, "screenshot.png") });

            // HTML dump
            var html = await Page.ContentAsync();
            await File.WriteAllTextAsync(Path.Combine(dir, "page.html"), html);

            // Console log
            await File.WriteAllLinesAsync(Path.Combine(dir, "console.log"), _consoleLogs);

            Console.WriteLine($"❌ Artifacts for '{testName}' written to {dir}");
        }
    }

    protected async Task BaseSetUp(bool login)
    {
        UserName = ReadSetting("Playwright__Users__Basic__Name");
        UserPassword = ReadSetting("Playwright__Users__Basic__Password");
        Page.Console += (_, msg) => _consoleLogs.Add($"[{msg.Type}] {msg.Text}");
        if (login)
        {
            await Page.GotoAsync(BaseUrl);
            var loginPage = new LoginPage(Page);
            await loginPage.Login(UserName, UserPassword);
        }
    }

    protected string ReadSetting(string variableName)
    {
        var setting = Environment.GetEnvironmentVariable(variableName);
        if (!string.IsNullOrEmpty(setting))
            return setting;
        variableName = variableName.Replace("__", ":");
        setting = _appSettings.GetValue<string>(variableName);
        if (!string.IsNullOrEmpty(setting))
            return setting;
        Assert.Fail($"Setting {variableName} not found");
        throw new Exception($"Setting {variableName} not found");
    }
}