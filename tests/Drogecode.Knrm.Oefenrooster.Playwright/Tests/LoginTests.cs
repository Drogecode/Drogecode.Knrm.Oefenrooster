using Drogecode.Knrm.Oefenrooster.Playwright.Pages;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;

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
            Page.SetDefaultTimeout(60000); // 60 seconds
            await Page.GotoAsync(BaseUrl);
            await Expect(Page.Locator(".kc-logo-text")).ToContainTextAsync("Keycloak", _timeout);
            await Expect(Page.Locator("id=username")).ToBeEmptyAsync();
            await Expect(Page.Locator("id=password")).ToBeEmptyAsync();
            await Expect(Page.Locator("id=kc-login")).ToBeEnabledAsync();
            await Page.FillAsync("input[name='username']", UserName);
            await Page.FillAsync("input[name='password']", UserPassword);
            await Page.Locator("id=kc-login").ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Expect(Page.GetByText("Invalid username or password.")).ToHaveCountAsync(0);
            await Expect(Page.GetByText("Dit is de landingspagina voor de vrijwilligers van KNRM Huizen & Huizer Reddingsbrigade.")).ToHaveCountAsync(0);
            await Expect(Page.GetByText("No access")).ToHaveCountAsync(0);
            await Expect(Page.GetByText("Geen toegang")).ToHaveCountAsync(0);
            await Expect(Page.GetByTestId("dashboard-username")).ToContainTextAsync("Playwright Basic", _timeout);
        }

        [Test]
        public async Task LoginWithTestPage()
        {
            await Page.GotoAsync(BaseUrl);
            var loginPage = new LoginPage(Page);
            await loginPage.Login(UserName, UserPassword);
            await Expect(Page.GetByTestId("dashboard-username")).ToContainTextAsync("Playwright Basic", _timeout);
        }
    }
}