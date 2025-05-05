using Drogecode.Knrm.Oefenrooster.Playwright.Pages;
using Microsoft.Extensions.Configuration;

namespace Drogecode.Knrm.Oefenrooster.Playwright.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class LoginTests : BaseTest
    {
        [Test]
        public async Task LoginToKeyCloak()
        {
            var userName = _appSettings.GetValue<string>("Users:Basic:Name");
            var userPassword = _appSettings.GetValue<string>("Users:Basic:Password");
            await Page.GotoAsync("https://localhost:44327/");
            await Page.Locator("id=username").FillAsync(userName!);
            await Page.Locator("id=password").FillAsync(userPassword!);
            await Page.Locator("id=kc-login").ClickAsync();
            await Expect(Page.GetByTestId("dashboard-username")).ToContainTextAsync("Welkom Playwright Basic");
        }

        [Test]
        public async Task LoginWithTestPage()
        {
            var userName = _appSettings.GetValue<string>("Users:Basic:Name");
            var userPassword = _appSettings.GetValue<string>("Users:Basic:Password");
            var loginPage = new LoginPage(Page);
            await loginPage.Login(userName!, userPassword!);
            await Expect(Page.GetByTestId("dashboard-username")).ToContainTextAsync("Welkom Playwright Basic");
        }
    }
}