using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components.Dialogs;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;
using Microsoft.AspNetCore.Components;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Pages.Configuration.Components;

public class TrainingTypeDialogTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task TrainingTypeDialogRenderTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<TrainingTypeDialog> L2)
    {
        Localize(L1, L2);
        IRenderedComponent<MudDialogProvider> comp = await RenderTestDialogComponent("Test TrainingTypeDialogRenderTest");
        comp.Markup.Should().Contain("Test TrainingTypeDialogRenderTest");
        comp.Markup.Should().Contain("Select Color");
        comp.Markup.Should().Contain("Count To Training Target");
        comp.Markup.Should().NotContain("Description is required");
    }
    [Theory]
    [AutoFakeItEasyData]
    public async Task ClickButtonTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<TrainingTypeDialog> L2)
    {
        Localize(L1, L2);
        IRenderedComponent<MudDialogProvider> comp = await RenderTestDialogComponent("Test ClickButtonTest");
        comp.Markup.Should().Contain("Test ClickButtonTest");
        comp.Markup.Should().Contain(Icons.Material.Filled.CheckBoxOutlineBlank);
        comp.Markup.Should().NotContain(Icons.Material.Filled.CheckBox);
        var button = comp.Find("#CountToTrainingTarget");
        button.Change(new ChangeEventArgs().Value = true);
        button = comp.Find("#IsDefault");
        button.Change(new ChangeEventArgs().Value = true);
        button = comp.Find("#IsActive");
        button.Change(new ChangeEventArgs().Value = true);
        comp.Markup.Should().NotContain(Icons.Material.Filled.CheckBoxOutlineBlank);
        comp.Markup.Should().Contain(Icons.Material.Filled.CheckBox);
    }

    private async Task<IRenderedComponent<MudDialogProvider>> RenderTestDialogComponent(string name)
    {
        var today = DateTime.Today;
        var planner = new PlannerTrainingType
        {
            Name = name,
        };
        var param = new DialogParameters<TrainingTypeDialog>() {
            { x => x.TrainingType, planner },
        };
        var comp = RenderComponent<MudDialogProvider>();
        comp.Markup.Trim().Should().BeEmpty();
        var service = Services.GetService<IDialogService>() as DialogService;
        service.Should().NotBe(null);

        IDialogReference? dialogReference = null;
        await comp.InvokeAsync(() => dialogReference = service!.Show<TrainingTypeDialog>(string.Empty, param));
        dialogReference.Should().NotBe(null);
        comp.Find("div.mud-dialog-container").Should().NotBe(null);
        return comp;
    }

    private void Localize(IStringLocalizer<App> L1, IStringLocalizer<TrainingTypeDialog> L2)
    {
        Services.AddSingleton(L1);
        Services.AddSingleton(L2);
        A.CallTo(() => L1["Cancel"]).Returns(new LocalizedString("Cancel", "Cancel"));
        A.CallTo(() => L1["Ok"]).Returns(new LocalizedString("Ok", "Ok"));

        A.CallTo(() => L2["Description"]).Returns(new LocalizedString("Description", "Description"));
        A.CallTo(() => L2["Description is required"]).Returns(new LocalizedString("Description is required", "Description is required"));
        A.CallTo(() => L2["Light background"]).Returns(new LocalizedString("Light background", "Light background"));
        A.CallTo(() => L2["Select Color"]).Returns(new LocalizedString("Select Color", "Select Color"));
        A.CallTo(() => L2["Light text"]).Returns(new LocalizedString("Light text", "Light text"));
        A.CallTo(() => L2["Dark background"]).Returns(new LocalizedString("Dark background", "Dark background"));
        A.CallTo(() => L2["Dark text"]).Returns(new LocalizedString("Dark text", "Dark text"));
        A.CallTo(() => L2["Description"]).Returns(new LocalizedString("Description", "Description"));
        A.CallTo(() => L2["CountToTrainingTarget"]).Returns(new LocalizedString("CountToTrainingTarget", "Count To Training Target"));
        A.CallTo(() => L2["IsDefault"]).Returns(new LocalizedString("IsDefault", "IsDefault"));
        A.CallTo(() => L2["IsActive"]).Returns(new LocalizedString("IsActive", "IsActive"));
    }
}
