using Drogecode.Knrm.Oefenrooster.Playwright.Pages;
using Microsoft.Extensions.Configuration;

namespace Drogecode.Knrm.Oefenrooster.Playwright.Tests;

public class TrainingsTests : BaseTest
{
    private string _userName;
    private string _userPassword;

    [SetUp]
    public async Task SetUp()
    {
        _userName = _appSettings.GetValue<string>("Users:Basic:Name")!;
        _userPassword = _appSettings.GetValue<string>("Users:Basic:Password")!;
        var loginPage = new LoginPage(Page, _baseUrl);
        await loginPage.Login(_userName, _userPassword);
    }

    [Test]
    public async Task GoToSchedulePageTest()
    {
        await Expect(Page.GetByTestId("nav-add-training")).ToBeHiddenAsync();
        await Page.GetByTestId("nav-schedule").ClickAsync();
        await Expect(Page.GetByTestId("nav-add-training")).ToContainTextAsync("Oefening");
    }
}