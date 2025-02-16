using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.MappersTests;

public class ReportActionMapperTests : BaseTest
{
    public ReportActionMapperTests(TestService testService) : base(testService)
    {
    }

    [Fact]
    public async Task ToDbReportActionTest()
    {
        var spAction = new SharePointAction()
        {
            Id = Guid.NewGuid(),
            OdataEtag = "01026485-d3e6-4ff6-bfac-e1da6704e900,4",
            Number = 1
        };
        var dbAction = spAction.ToDbReportAction(Tester.DefaultCustomerId);
        dbAction.Should().NotBeNull();
        dbAction.CustomerId.Should().Be(Tester.DefaultCustomerId);
    }
}