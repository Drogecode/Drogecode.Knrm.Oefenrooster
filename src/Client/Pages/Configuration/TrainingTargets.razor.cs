using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public partial class TrainingTargets : ComponentBase
{
    [Inject, NotNull] private IStringLocalizer<TrainingTargets>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
}