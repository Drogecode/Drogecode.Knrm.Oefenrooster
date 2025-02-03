﻿using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;
using Microsoft.AspNetCore.Components.Authorization;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Shared.Layout;

public class UpdateCheckerTest : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void ListTest([Frozen] IStringLocalizer<UpdateChecker> L1)
    {
        Services.AddScoped<AuthenticationStateProvider, CustomStateProvider>();
        Localize(L1);
        
        Services.AddScoped<AuthenticationStateProvider, CustomStateProvider>();
        var cut = RenderComponent<UpdateChecker>();
    }

    private void Localize(IStringLocalizer<UpdateChecker> L1)
    {
        Services.AddSingleton(L1);

        A.CallTo(() => L1["Click to reload"]).Returns(new LocalizedString("Click to reload", "Click to reload"));
        A.CallTo(() => L1["Update available"]).Returns(new LocalizedString("Update available", "Update available"));
    }
}