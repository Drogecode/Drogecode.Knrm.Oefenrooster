using Drogecode.Knrm.Oefenrooster.Playwright.Pages;
using Microsoft.Extensions.Configuration;

namespace Drogecode.Knrm.Oefenrooster.Playwright.Tests;

public class TrainingsTests : BaseTest
{
    [Test]
    public async Task GoToSchedulePageTest()
    {
        await Expect(Page.GetByTestId("nav-add-training")).ToBeHiddenAsync();
        await Page.GetByTestId("nav-schedule").ClickAsync();
        await Expect(Page.GetByTestId("nav-add-training")).ToContainTextAsync("Oefening", _timeout);
    }
}