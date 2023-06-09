using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Components;

public sealed partial class IndexCard : IDisposable
{
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Parameter, EditorRequired] public DrogeUser? User { get; set; }
    [Parameter, EditorRequired] public Training Training { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; }
    private CancellationTokenSource _cls = new();

    protected override void OnParametersSet()
    {
        if (Training.Availabilty == Availabilty.None)
            Training.Availabilty = null;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
