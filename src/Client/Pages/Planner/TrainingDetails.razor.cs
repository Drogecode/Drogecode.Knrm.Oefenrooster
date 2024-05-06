using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;

public partial class TrainingDetails : ComponentBase
{
    [Inject] private IStringLocalizer<TrainingDetails> L { get; set; } = default!;
    [Parameter] public Guid? Id { get; set; }
}