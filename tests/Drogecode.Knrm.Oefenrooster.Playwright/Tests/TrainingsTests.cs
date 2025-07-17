namespace Drogecode.Knrm.Oefenrooster.Playwright.Tests;

public class TrainingsTests : BaseTest
{
    [SetUp]
    public override async Task SetUp()
    {
        await base.SetUp();
        await Expect(Page.GetByTestId("nav-add-training")).ToBeHiddenAsync();
        await Page.GetByTestId("nav-schedule").ClickAsync();
    }

    [Test]
    public async Task AddScheduleToPageTest()
    {
        await Expect(Page.GetByTestId("nav-add-training")).ToContainTextAsync("Oefening", _timeout);
        await Expect(Page.GetByTestId("Omschrijving - Markdown")).ToHaveCountAsync(0);
        await Page.GetByTestId("nav-add-training").ClickAsync();
        await Expect(Page.GetByTestId("training-name")).ToBeVisibleAsync();
    }
}