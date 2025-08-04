using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode.Dialogs;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;
using Drogecode.Knrm.Oefenrooster.TestClient.Mocks;
using Drogecode.Knrm.Oefenrooster.TestClient.TestPages.Components.DrogeCode.Dialogs;
using RichardSzalay.MockHttp;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Components.DrogeCode.Dialogs;

public class TrainingMessageDialogTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void NoDescriptionSetTest([Frozen] IStringLocalizer<TrainingMessageDialog> L1)
    {
        Localize(L1);

        var trainingId = Guid.NewGuid();
        var training = new TrainingAdvance
        {
            TrainingId = trainingId
        };
        MockHttp.When($"http://localhost/api/Schedule/{trainingId.ToString()}/description").RespondJson(new GetDescriptionByTrainingIdResponse
        {
            Success = false,
            Description = null,
        });
        var cut = RenderComponent<TrainingMessageDialogTestPage>(parameter => parameter.Add(p => p.Training, training));
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("No description found"), TimeSpan.FromSeconds(2));
    }
    
    [Theory]
    [AutoFakeItEasyData]
    public void HasDescriptionTest([Frozen] IStringLocalizer<TrainingMessageDialog> L1)
    {
        Localize(L1);

        var trainingId = Guid.NewGuid();
        var description = "Basic description";
        var training = new TrainingAdvance
        {
            TrainingId = trainingId
        };
        MockHttp.When($"http://localhost/api/Schedule/{trainingId.ToString()}/description").RespondJson(new GetDescriptionByTrainingIdResponse
        {
            Success = true,
            Description = description,
        });
        var cut = RenderComponent<TrainingMessageDialogTestPage>(parameter => parameter.Add(p => p.Training, training));
        cut.WaitForAssertion(() => cut.Markup.Should().Contain(description), TimeSpan.FromSeconds(2));
    }
    
    private void Localize(IStringLocalizer<TrainingMessageDialog> L1)
    {
        Services.AddSingleton(L1);
        
        LocalizeA(L1, "No description found");
    }
}