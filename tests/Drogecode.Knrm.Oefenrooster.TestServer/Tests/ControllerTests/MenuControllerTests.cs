using Drogecode.Knrm.Oefenrooster.TestServer.Seeds;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class MenuControllerTests : BaseTest
{
    public MenuControllerTests(TestService testService) : base(testService)
    {
    }

    public override async Task InitializeAsync()
    {
        // Seed until I build the complete crud for menu's
        await base.InitializeAsync();
        SeedMenu.Seed(Tester.DataContext, Tester.DefaultCustomerId);
    }

    [Fact]
    public async Task GetMenuTest()
    {
        var result = await Tester.MenuController.GetAll();
        Assert.NotNull(result?.Value);
        result.Value.Success.Should().BeTrue();
        result.Value.TotalCount.Should().BeGreaterThanOrEqualTo(3);
        result.Value.Menus.Should().Contain(x => x.Url == "https://dorus1824.sharepoint.com");
    }
}