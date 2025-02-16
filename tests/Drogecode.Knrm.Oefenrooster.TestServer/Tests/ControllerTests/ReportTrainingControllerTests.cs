using System.Text.Json;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportTraining;
using Drogecode.Knrm.Oefenrooster.TestServer.Seeds;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class ReportTrainingControllerTests : BaseTest
{
    public ReportTrainingControllerTests(TestService testService) : base(testService)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        SeedReportTraining.Seed(Tester.DataContext, Tester.DefaultCustomerId);
    }

    [Fact]
    public async Task GetReportActionsCurrentUserTest()
    {
        var getResult = await Tester.ReportTrainingController.GetLastTrainingsForCurrentUser(10, 0);
        Assert.NotNull(getResult.Value?.Trainings);
        Assert.NotEmpty(getResult.Value.Trainings);
        Assert.True(getResult.Value.Success);
        getResult.Value.Trainings.Count.Should().BeGreaterThanOrEqualTo(2);
        var prevStart = DateTime.MaxValue;
        foreach (var action in getResult.Value.Trainings)
        {
            action.Start.Should().BeBefore(prevStart);
            prevStart = action.Start;
        }
    }

    [Fact]
    public async Task GetReportActionsAllUsersTest()
    {
        var emptyList = JsonSerializer.Serialize(new List<Guid>());
        var getResult = await Tester.ReportTrainingController.GetLastTrainings(emptyList, 10, 0);
        Assert.NotNull(getResult.Value?.Trainings);
        Assert.NotEmpty(getResult.Value.Trainings);
        Assert.True(getResult.Value.Success);
        getResult.Value.Trainings.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task AnalyzeYearChartsAllForAllUsers3YearsTest()
    {
        var request = new AnalyzeTrainingRequest()
        {
            Users = new List<Guid?> {  },
            Years = 3
        };
        var getResult = await Tester.ReportTrainingController.AnalyzeYearChartsAll(request);
        Assert.NotNull(getResult.Value?.Years);
        Assert.True(getResult.Value.Success);
        getResult.Value.Years.Should().HaveCount(3);
        getResult.Value.TotalCount.Should().Be(3*3);
        getResult.Value.Years.Should().Contain(x => x.Year == 2022);
        getResult.Value.Years.Should().Contain(x => x.Year == 2020);
        getResult.Value.Years.Should().Contain(x => x.Year == 2019);
        getResult.Value.Years.Should().NotContain(x => x.Year == 2018);
    }

    [Fact]
    public async Task GetReportActionsUnknownUserTest()
    {
        var unknownUser = JsonSerializer.Serialize(new List<Guid> { Guid.NewGuid() });
        var getResult = await Tester.ReportTrainingController.GetLastTrainings(unknownUser, 10, 0);
        Assert.NotNull(getResult.Value?.Trainings);
        Assert.True(getResult.Value.Success);
        getResult.Value.Trainings.Should().BeEmpty();
    }
}