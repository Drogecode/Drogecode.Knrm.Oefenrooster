using Drogecode.Knrm.Oefenrooster.Playwright.Pages;
using Microsoft.Extensions.Configuration;

namespace Drogecode.Knrm.Oefenrooster.Playwright;

public class BaseTest : PageTest
{
    private readonly IConfigurationRoot _appSettings;
    protected readonly string BaseUrl;
    
    protected string UserName;
    protected string UserPassword;
    
    public BaseTest()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json", optional: false)
            .AddJsonFile("appsettings.withsecrets.json", optional: true)
            .Build();

        _appSettings = config;
        BaseUrl = config.GetValue<string>("BaseUrl")!;
    }
    
    [SetUp]
    public virtual async Task SetUp()
    {
        await BaseSetUp(true);
    }

    protected async Task BaseSetUp(bool login)
    {
        UserName = _appSettings.GetValue<string>("Users:Basic:Name")!;
        UserPassword = _appSettings.GetValue<string>("Users:Basic:Password")!;
        if (login)
        {
            var loginPage = new LoginPage(Page, BaseUrl);
            await loginPage.Login(UserName, UserPassword);
        }
    }
}