using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;
using Drogecode.Knrm.Oefenrooster.TestClient.Mocks.MockClients;

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

        var today = DateTime.Today;
        var cut = RenderComponent<TrainingTypes>();
        cut.WaitForElement($"#edit-trainingtype-{MockTrainingTypesClient.DEFAULT_PLANNER_TYPE}", TimeSpan.FromSeconds(2));
        cut.Find($"#edit-trainingtype-{MockTrainingTypesClient.DEFAULT_PLANNER_TYPE}").Click();
    }

    private void Localize(IStringLocalizer<App> L1, IStringLocalizer<TrainingTypes> L2, IStringLocalizer<TrainingTypeDialog> L3)
    {
        Services.AddSingleton(L1);
        Services.AddSingleton(L2);
        Services.AddSingleton(L3);
        A.CallTo(() => L1["Cancel"]).Returns(new LocalizedString("Cancel", "Cancel"));
        A.CallTo(() => L1["Ok"]).Returns(new LocalizedString("Ok", "Ok"));

        A.CallTo(() => L2["Training types"]).Returns(new LocalizedString("Training types", "Training types"));
        A.CallTo(() => L2["Add and change training type"]).Returns(new LocalizedString("Add and change training type", "Add and change training type"));
        A.CallTo(() => L2["Add new training type"]).Returns(new LocalizedString("Add new training type", "Add new training type"));
        A.CallTo(() => L2["Order"]).Returns(new LocalizedString("Order", "Order"));
        A.CallTo(() => L2["Description"]).Returns(new LocalizedString("Description", "Description"));
        A.CallTo(() => L2["Color light"]).Returns(new LocalizedString("Color light", "Color light"));
        A.CallTo(() => L2["Color dark"]).Returns(new LocalizedString("Color dark", "Color dark"));
        A.CallTo(() => L2["CountToTrainingTarget"]).Returns(new LocalizedString("CountToTrainingTarget", "CountToTrainingTarget"));
        A.CallTo(() => L2["IsDefault"]).Returns(new LocalizedString("IsDefault", "IsDefault"));
        A.CallTo(() => L2["Edit training type"]).Returns(new LocalizedString("Edit training type", "Edit training type"));

        A.CallTo(() => L3["Description"]).Returns(new LocalizedString("Description", "Description"));
        A.CallTo(() => L3["Description is required"]).Returns(new LocalizedString("Description is required", "Description is required"));
        A.CallTo(() => L3["Light background"]).Returns(new LocalizedString("Light background", "Light background"));
        A.CallTo(() => L3["Select Color"]).Returns(new LocalizedString("Select Color", "Select Color"));
        A.CallTo(() => L3["Light text"]).Returns(new LocalizedString("Light text", "Light text"));
        A.CallTo(() => L3["Dark background"]).Returns(new LocalizedString("Dark background", "Dark background"));
        A.CallTo(() => L3["Dark text"]).Returns(new LocalizedString("Dark text", "Dark text"));
        A.CallTo(() => L3["Description"]).Returns(new LocalizedString("Description", "Description"));
        A.CallTo(() => L3["Description"]).Returns(new LocalizedString("Description", "Description"));
        A.CallTo(() => L3["Description"]).Returns(new LocalizedString("Description", "Description"));
    }
}
