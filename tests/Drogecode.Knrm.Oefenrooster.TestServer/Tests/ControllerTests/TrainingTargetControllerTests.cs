using Drogecode.Knrm.Oefenrooster.TestServer.Seeds;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class TrainingTargetControllerTests(TestService testService) : BaseTest(testService)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        SeedTargets.Seed(Tester.DataContext, Tester.DefaultCustomerId);
    }

    [Fact]
    public async Task AllTrainingTargetsTest()
    {
        var result = await Tester.TrainingTargetController.AllTrainingTargets(10, 0, null);
        Assert.NotNull(result.Value?.TrainingSubjects);
        Assert.NotEmpty(result.Value.TrainingSubjects);
        Assert.True(result.Value.Success);
        foreach (var subject in result.Value.TrainingSubjects)
        {
            subject.TrainingSubjects.Should().NotBeEmpty();
            subject.TrainingTargets.Should().BeNull();
            foreach (var childSubject in subject.TrainingSubjects)
            {
                childSubject.TrainingSubjects.Should().BeNull();
                childSubject.TrainingTargets.Should().NotBeEmpty();
            }
        }
    }

    [Fact]
    public async Task AllTrainingTargetsForSubjectATest()
    {
        var result = await Tester.TrainingTargetController.AllTrainingTargets(10, 0, SeedTargets.TrainingTargetSubjectAlgemeneKennis);
        Assert.NotNull(result.Value?.TrainingSubjects);
        Assert.NotEmpty(result.Value.TrainingSubjects);
        Assert.True(result.Value.Success);
        result.Value.TrainingSubjects.Should().HaveCount(1);
        var subject = result.Value.TrainingSubjects.FirstOrDefault();
        Assert.NotNull(subject?.TrainingSubjects);
        subject.TrainingSubjects.Should().HaveCount(1);
        var childSubject = subject.TrainingSubjects.FirstOrDefault();
        Assert.NotNull(childSubject);
        childSubject.TrainingSubjects.Should().BeNull();
        childSubject.TrainingTargets.Should().NotBeEmpty();
    }
}