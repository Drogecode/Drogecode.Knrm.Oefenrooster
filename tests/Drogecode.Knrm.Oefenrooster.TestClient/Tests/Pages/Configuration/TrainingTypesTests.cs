using AutoFixture.Xunit2;
using Bunit;
using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;
using Drogecode.Knrm.Oefenrooster.TestClient.Mocks;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Pages.Configuration;

public class TrainingTypesTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void DateShrinkTodayTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<TrainingTypes> L2)
    {
        Services.AddSingleton(L1);
        Services.AddSingleton(L2);
        Services.AddMockServices();
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

        var today = DateTime.Today;
        var cut = RenderComponent<TrainingTypes>();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("CountToTrainingTarget"), TimeSpan.FromSeconds(2));
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("GetTrainingTypesAsync Mock default"), TimeSpan.FromSeconds(2));
    }
}
