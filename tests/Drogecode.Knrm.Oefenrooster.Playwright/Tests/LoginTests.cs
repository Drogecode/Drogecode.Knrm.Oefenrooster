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
            try
            {
            Page.SetDefaultTimeout(60000); // 60 seconds
            await TestContext.Out.WriteLineAsync($"Starting LoginTest: {BaseUrl.Length}, {UserName.Length}, {UserPassword.Length}");
            await Page.GotoAsync(BaseUrl);
            await Expect(Page.Locator(".kc-logo-text")).ToContainTextAsync("Keycloak");
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
            await Expect(Page.GetByTestId("dashboard-username")).ToContainTextAsync("Playwright Basic");
            }
            catch (Exception ex)
            {
                // Dump page for debugging
                Console.WriteLine("‼️ Error during login or authorization");
                var html = await Page.ContentAsync();
                var path = "playwright-debug";
                Directory.CreateDirectory(path);
                await File.WriteAllTextAsync(Path.Combine(path, "page.html"), html);
                await Page.ScreenshotAsync(new() { Path = Path.Combine(path, "screenshot.png") });
                Console.WriteLine($"❌ Debug written to: {Path.GetFullPath(path)}");
                await TestContext.Out.WriteLineAsync($"Page URL: {Page.Url}");
                await TestContext.Out.WriteLineAsync(html);

                throw new Exception("Login or authorization failed. Debug output saved.", ex);
            }
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