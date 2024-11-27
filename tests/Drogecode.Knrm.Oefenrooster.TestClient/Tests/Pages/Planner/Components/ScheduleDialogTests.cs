using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using FluentAssertions.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Pages.Planner.Components;

public class ScheduleDialogTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task ScheduleDialogRenderTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<ScheduleDialog> L2)
    {
        Localize(L1, L2);
        IRenderedComponent<MudDialogProvider> comp = await RenderTestDialogComponent("Test ScheduleDialogRenderTest");
    }

    private async Task<IRenderedComponent<MudDialogProvider>> RenderTestDialogComponent(string name)
    {
        var today = DateTime.Today;
        var planner = new PlannedTraining
        {
            Name = name,
        };
        var param = new DialogParameters<ScheduleDialog>
        {
            { x => x.Planner, planner },
        };
        var comp = RenderComponent<MudDialogProvider>();
        comp.Markup.Trim().Should().BeEmpty();
        var service = Services.GetRequiredService<IDialogService>();
        service.Should().NotBe(null);
        var dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service.ShowAsync<ScheduleDialog>(string.Empty, param));
        await comp.InvokeAsync(() => dialogReferenceLazy.Value);
        var dialogReference = await dialogReferenceLazy.Value;
        dialogReference.Should().NotBe(null);
        comp.Find("div.mud-dialog-container").Should().NotBe(null);
        return comp;
    }

    private void Localize(IStringLocalizer<App> L1, IStringLocalizer<ScheduleDialog> L2)
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
