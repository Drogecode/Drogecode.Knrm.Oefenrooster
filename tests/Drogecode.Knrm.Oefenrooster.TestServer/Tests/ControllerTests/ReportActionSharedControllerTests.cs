namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class ReportActionSharedControllerTests : BaseTest
{
    public ReportActionSharedControllerTests(TestService testService) : base(testService)
    {
    }

    [Fact]
    public async Task PutReportActionSharedTest()
    {
        var id = await Tester.AddActionShared(
            ["xUnit", "MoreTests"],
            ["unhealthy"],
            DateTime.UtcNow.AddDays(100),
            DateTime.UtcNow.AddDays(-50),
            DateTime.UtcNow.AddDays(-5));
        id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetMultiple()
    {
        var put1 = await Tester.AddActionShared([], [], DateTime.UtcNow.AddDays(1));
        var put2 = await Tester.AddActionShared([], [], DateTime.UtcNow.AddDays(-1));
        var put3 = await Tester.AddActionShared([], [], DateTime.UtcNow.AddDays(10));
        var getAll = await Tester.ReportActionSharedController.GetAllReportActionShared();
        Assert.NotNull(getAll.Value?.ReportActionSharedConfiguration);
        Assert.NotEmpty(getAll.Value.ReportActionSharedConfiguration);
        getAll.Value.Success.Should().BeTrue();
        getAll.Value.TotalCount.Should().Be(2);
        getAll.Value.ReportActionSharedConfiguration.Should().Contain(x => x.Id == put1);
        getAll.Value.ReportActionSharedConfiguration.Should().NotContain(x => x.Id == put2);
        getAll.Value.ReportActionSharedConfiguration.Should().Contain(x => x.Id == put3);
    }
    
    [Fact]
    public async Task GetMultipleAndDeleteOne()
    {
        var put1 = await Tester.AddActionShared([], [], DateTime.UtcNow.AddDays(1));
        var put2 = await Tester.AddActionShared([], [], DateTime.UtcNow.AddDays(-1));
        var put3 = await Tester.AddActionShared([], [], DateTime.UtcNow.AddDays(10));
        var getAll = await Tester.ReportActionSharedController.GetAllReportActionShared();
        Assert.NotNull(getAll.Value?.ReportActionSharedConfiguration);
        Assert.NotEmpty(getAll.Value.ReportActionSharedConfiguration);
        getAll.Value.Success.Should().BeTrue();
        getAll.Value.TotalCount.Should().Be(2);
        getAll.Value.ReportActionSharedConfiguration.Should().Contain(x => x.Id == put1);
        getAll.Value.ReportActionSharedConfiguration.Should().NotContain(x => x.Id == put2);
        getAll.Value.ReportActionSharedConfiguration.Should().Contain(x => x.Id == put3);
        
        var delete3 = await Tester.ReportActionSharedController.DeleteReportActionShared(put3);
        Assert.NotNull(delete3.Value?.Success);
        Assert.True(delete3.Value.Success);
        var getAllAgain = await Tester.ReportActionSharedController.GetAllReportActionShared();
        Assert.NotNull(getAllAgain.Value?.ReportActionSharedConfiguration);
        Assert.NotEmpty(getAllAgain.Value.ReportActionSharedConfiguration);
        getAllAgain.Value.Success.Should().BeTrue();
        getAllAgain.Value.TotalCount.Should().Be(1);
        getAllAgain.Value.ReportActionSharedConfiguration.Should().Contain(x => x.Id == put1);
        getAllAgain.Value.ReportActionSharedConfiguration.Should().NotContain(x => x.Id == put2);
        getAllAgain.Value.ReportActionSharedConfiguration.Should().NotContain(x => x.Id == put3);
    }
}