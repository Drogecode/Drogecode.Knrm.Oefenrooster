using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Pages.Planner.Components;

public class ScheduleCounterTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void HasNameTest([Frozen] IStringLocalizer<ScheduleCounter> L1)
    {
        Localize(L1);

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var userId3 = Guid.NewGuid();
        var userId4 = Guid.NewGuid();
        List<DrogeUser>? users = new List<DrogeUser>
        {
            new DrogeUser
            {
                Id = userId1,
                Name = "user1",
                UserFunctionId = Function1Id
            },
            new DrogeUser
            {
                Id = userId2,
                Name = "user2",
                UserFunctionId = Function2Id
            },
            new DrogeUser
            {
                Id = userId3,
                Name = "user3",
                UserFunctionId = Function1Id
            },
            new DrogeUser
            {
                Id = userId4,
                Name = "user4",
                UserFunctionId = Function2Id
            },
        };
        List<UserTrainingCounter>? trainingCounters = new List<UserTrainingCounter>
        {
            new UserTrainingCounter
            {
                UserId = userId1,
                Count = 1
            },
            new UserTrainingCounter
            {
                UserId = userId4,
                Count = 0
            },
        };
       var cut = RenderComponent<ScheduleCounter>(parameter => parameter
        .Add(p => p.Users, users)
        .Add(p => p.Functions, Functions)
        .Add(p => p.UserTrainingCounter, trainingCounters));
        cut.Markup.Should().Contain("user1 : 1");
        cut.Markup.Should().NotContain("user2 : No training");
        cut.Markup.Should().Contain("user3 : No training");
        cut.Markup.Should().NotContain("user4 : 0");
    }
    private void Localize(IStringLocalizer<ScheduleCounter> L1)
    {
        Services.AddSingleton(L1);

        A.CallTo(() => L1["No training"]).Returns(new LocalizedString("No training", "No training"));
    }
}
