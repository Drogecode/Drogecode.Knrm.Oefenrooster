using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;
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
    public async Task AddTrainingSubjectTest()
    {
        const string name = "AddTrainingSubjectTest";
        var body = new TrainingSubject
        {
            Order = 5,
            Name = name,
        };
        var result = await Tester.TrainingTargetController.PutNewSubject(body);
        Assert.NotNull(result.Value?.NewId);
        Assert.True(result.Value.Success);
        result.Value.NewId.Should().NotBe(Guid.Empty);
        var allTargets = await Tester.TrainingTargetController.AllTrainingTargets(10, 0, null);
        allTargets.Value!.TrainingSubjects.Should().Contain(x => x.Id == result.Value.NewId && x.Name == name);
    }

    [Fact]
    public async Task AddTrainingTargetTest()
    {
        const string name = "AddTrainingTargetTest";
        const string url = "https://www.drogecode.nl";
        const string urlDescription = "Compas";
        var body = new TrainingTarget
        {
            Order = 5,
            Name = name,
            SubjectId = SeedTargets.TrainingTargetSubjectAlgemeneKennis,
            Url = url,
            UrlDescription = urlDescription,
            Type = TrainingTargetType.Exercise,
            Group = TrainingTargetGroup.TwoPersons,
        };
        var result = await Tester.TrainingTargetController.PutNewTarget(body);
        Assert.NotNull(result.Value?.NewId);
        Assert.True(result.Value.Success);
        var allTargets = await Tester.TrainingTargetController.AllTrainingTargets(10, 0, null);
        var subject = allTargets.Value!.TrainingSubjects!.FirstOrDefault(x => x.Id == SeedTargets.TrainingTargetSubjectAlgemeneKennis);
        Assert.NotNull(subject);
        subject.TrainingTargets.Should().Contain(x => x.Id == result.Value.NewId && x.Name == name);
        var target = subject.TrainingTargets.FirstOrDefault(x => x.Id == result.Value.NewId);
        Assert.NotNull(target);
        target.SubjectId.Should().Be(SeedTargets.TrainingTargetSubjectAlgemeneKennis);
        target.Url.Should().Be(url);
        target.UrlDescription.Should().Be(urlDescription);
        target.Type.Should().Be(TrainingTargetType.Exercise);
        target.Group.Should().Be(TrainingTargetGroup.TwoPersons);
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