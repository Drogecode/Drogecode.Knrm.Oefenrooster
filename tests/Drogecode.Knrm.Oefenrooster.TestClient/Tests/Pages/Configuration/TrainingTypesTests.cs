using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components.Dialogs;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;
using Drogecode.Knrm.Oefenrooster.TestClient.Mocks.MockClients;
using Drogecode.Knrm.Oefenrooster.TestClient.TestPages.Pages.Configuration;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Pages.Configuration;

public class TrainingTypesTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void TrainingTypeRenderTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<TrainingTypes> L2, [Frozen] IStringLocalizer<TrainingTypeDialog> L3)
    {
        Localize(L1, L2, L3);

        var today = DateTime.Today;
        var cut = RenderComponent<TrainingTypes>();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("CountToTrainingTarget"), TimeSpan.FromSeconds(2));
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("GetTrainingTypesAsync Mock default"), TimeSpan.FromSeconds(2));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void TrainingTypeEditTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<TrainingTypes> L2, [Frozen] IStringLocalizer<TrainingTypeDialog> L3)
    {
        Localize(L1, L2, L3);

        var cut = RenderComponent<TrainingTypesTestPage>();
        cut.WaitForElement($"#edit-trainingtype-{MockTrainingTypesClient.DEFAULT_PLANNER_TYPE}", TimeSpan.FromSeconds(2));
        cut.Find($"#edit-trainingtype-{MockTrainingTypesClient.DEFAULT_PLANNER_TYPE}").Click();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Edit training type"), TimeSpan.FromSeconds(2));
    }

    private void Localize(IStringLocalizer<App> L1, IStringLocalizer<TrainingTypes> L2, IStringLocalizer<TrainingTypeDialog> L3)
    {
        Services.AddSingleton(L1);
        Services.AddSingleton(L2);
        Services.AddSingleton(L3);
        LocalizeA(L1, "Cancel");
        LocalizeA(L1, "Ok");

        LocalizeA(L2, "Training types");
        LocalizeA(L2, "Add and change training type");
        LocalizeA(L2, "Add new training type");
        LocalizeA(L2, "Order");
        LocalizeA(L2, "Description");
        LocalizeA(L2, "Color light");
        LocalizeA(L2, "Color dark");
        LocalizeA(L2, "CountToTrainingTarget");
        LocalizeA(L2, "IsDefault");
        LocalizeA(L2, "Edit training type");

        LocalizeA(L3, "Description");
        LocalizeA(L3, "Description is required");
        LocalizeA(L3, "Light background");
        LocalizeA(L3, "Select Color");
        LocalizeA(L3, "Light text");
        LocalizeA(L3, "Dark background");
        LocalizeA(L3, "Dark text");
    }
}
