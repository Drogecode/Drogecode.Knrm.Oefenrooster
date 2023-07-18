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
        List<DrogeUser>? users = new List<DrogeUser>
        {
            new DrogeUser
            {
                Id = userId1,
                Name = "user1",
                UserFunctionId = Function1Id
            }
        };
        List<UserTrainingCounter>? trainingCounters = new List<UserTrainingCounter>
        {
            new UserTrainingCounter
            {
                UserId = userId1,
                Count = 1
            }
        };
       var cut = RenderComponent<ScheduleCounter>(parameter => parameter
        .Add(p => p.Users, users)
        .Add(p => p.Functions, Functions)
        .Add(p => p.UserTrainingCounter, trainingCounters));
        cut.Markup.Should().Contain("user1 : 1");
    }
    private void Localize(IStringLocalizer<ScheduleCounter> L1)
    {
        Services.AddSingleton(L1);
    }
}
