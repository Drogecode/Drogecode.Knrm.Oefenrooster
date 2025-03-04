namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class AuditControllerTests : BaseTest
{
    public AuditControllerTests(TestService testService) : base(testService)
    {
    }

    [Fact]
    public async Task GetAllTrainingsAuditTest()
    {
        var getResponse = await Tester.AuditController.GetAllTrainingsAudit(30, 0);
        Assert.NotNull(getResponse?.Value?.TrainingAudits);
        getResponse.Value.Success.Should().BeTrue();
        getResponse.Value.TrainingAudits.Should().NotBeNull();
    }
}