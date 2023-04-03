using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public sealed partial class CalendarBaseCard : IDisposable
{
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Parameter, EditorRequired] public TrainingAdvance Training { get; set; } = default!;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback OnClickCallback { get; set; }
    [Parameter] public EventCallback OnClickSettings { get; set; }
    [Parameter] public EventCallback OnClickHistory { get; set; }
    [Parameter] public string? ContentClass { get; set; }
    [Parameter] public string Width { get; set; } = "200px";
    [Parameter] public bool ReplaceEmtyName { get; set; }
    private CancellationTokenSource _cls = new();

    public void Dispose()
    {
        _cls.Cancel();
    }
}
