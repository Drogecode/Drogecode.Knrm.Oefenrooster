using Drogecode.Knrm.Oefenrooster.Playwright.Pages;
using Microsoft.Extensions.Configuration;

namespace Drogecode.Knrm.Oefenrooster.Playwright.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class LoginTests : BaseTest
    {
        private readonly IConfigurationRoot _appSettings;

        [SetUp]
        public override async Task SetUp()
        {
            // Override to prevent auto login.
           await BaseSetUp(false);
        }
        
        [Test]
        public async Task LoginToKeyCloak()
        {
            await Page.GotoAsync(BaseUrl);
            await Page.Locator("id=username").FillAsync(UserName);
            await Page.Locator("id=password").FillAsync(UserPassword);
            await Page.Locator("id=kc-login").ClickAsync();
            await Expect(Page.GetByTestId("dashboard-username")).ToContainTextAsync("Playwright Basic" + BaseUrl);
        }

        [Test]
        public async Task LoginWithTestPage()
        {
            var loginPage = new LoginPage(Page, BaseUrl);
            await loginPage.Login(UserName, UserPassword);
            await Expect(Page.GetByTestId("dashboard-username")).ToContainTextAsync("Playwright Basic");
        }
    }
}