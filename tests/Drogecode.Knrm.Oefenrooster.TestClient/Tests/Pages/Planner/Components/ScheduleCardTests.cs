using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Pages.Planner.Components;

public class ScheduleCardTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void HasNameTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<ScheduleCard> L2)
    {
        Localize(L1, L2);

        var training = new PlannedTraining { Name = "xUnit meets bUnit" };
        var cut = RenderComponent<ScheduleCard>(parameter => parameter.Add(p => p.Planner, training));
        cut.Markup.Should().Contain("xUnit meets bUnit");
        cut.Markup.Should().Contain("till with some more text to ensure it is replaced");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void ListTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<ScheduleCard> L2)
    {
        Localize(L1, L2);

        var function1Id = Guid.NewGuid();
        var function2Id = Guid.NewGuid();
        List<DrogeFunction>? functions = new List<DrogeFunction>
         {
             new DrogeFunction
             {
                 Id = function1Id,
                 Name = "Test function 1",
                 Order = 1,
                 Active = true,
             },
             new DrogeFunction
             {
                 Id = function2Id,
                 Name = "Test function 2",
                 Order = 2,
                 Active = true,
             }
         };
        var training = new PlannedTraining { 
            Name = "xUnit meets bUnit", 
            PlanUsers = new List<PlanUser> 
            {
                new PlanUser
                {
                    UserFunctionId = function2Id,
                    PlannedFunctionId = function1Id,
                    Name = "test user 1",
                    Assigned = true,
                }
            } 
        };
        var cut = RenderComponent<ScheduleCard>(parameter => parameter
        .Add(p => p.Planner, training)
        .Add(p => p.Functions, functions));
        cut.Markup.Should().Contain("test user 1");
        cut.Markup.Should().Contain("Test function 1");
        cut.Markup.Should().NotContain("Test function 2");
    }

    private void Localize(IStringLocalizer<App> L1, IStringLocalizer<ScheduleCard> L2)
    {
        Services.AddSingleton(L1);
        Services.AddSingleton(L2);

        A.CallTo(() => L1["till"]).Returns(new LocalizedString("till", "till with some more text to ensure it is replaced"));
    }
}
