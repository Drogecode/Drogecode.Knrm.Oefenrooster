using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class ScheduleCounter
{
    [Inject] private IStringLocalizer<ScheduleCounter> L { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeUser>? Users { get; set; }
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; }
    [Parameter, EditorRequired] public List<UserTrainingCounter>? UserTrainingCounter { get; set; }
}
