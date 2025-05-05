using Drogecode.Knrm.Oefenrooster.Playwright.Pages;
using Microsoft.Extensions.Configuration;

namespace Drogecode.Knrm.Oefenrooster.Playwright.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class LoginTests : BaseTest
    {
        private string _userName;
        private string _userPassword;
        [SetUp]
        public void SetUp()
        {
            _userName = _appSettings.GetValue<string>("Users:Basic:Name")!;
            _userPassword = _appSettings.GetValue<string>("Users:Basic:Password")!;
        }

        [Test]
        public async Task LoginToKeyCloak()
        {
            await Page.GotoAsync(_baseUrl);
            await Page.Locator("id=username").FillAsync(_userName);
            await Page.Locator("id=password").FillAsync(_userPassword);
            await Page.Locator("id=kc-login").ClickAsync();
            await Expect(Page.GetByTestId("dashboard-username")).ToContainTextAsync("Welkom Playwright Basic");
        }

        [Test]
        public async Task LoginWithTestPage()
        {
            var loginPage = new LoginPage(Page, _baseUrl);
            await loginPage.Login(_userName, _userPassword);
            await Expect(Page.GetByTestId("dashboard-username")).ToContainTextAsync("Welkom Playwright Basic");
        }
    }
}