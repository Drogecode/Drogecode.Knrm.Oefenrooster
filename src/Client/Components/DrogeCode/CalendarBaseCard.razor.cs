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
    [Parameter] public AvailabilitySetBy SetBy { get; set; }
    [Parameter] public string? ContentClass { get; set; }
    [Parameter] public string Width { get; set; } = "200px";
    [Parameter] public string? MinWidth { get; set; }
    [Parameter] public string? MaxWidth { get; set; }
    [Parameter] public bool ReplaceEmptyName { get; set; }
    [Parameter] public bool ShowDate { get; set; } = true;
    [Parameter] public bool ShowDayOfWeek { get; set; } = false;
    [Parameter] public bool ShowPastBody { get; set; } = true;
    [Parameter] public string MoreMessage { get; set; } = "Show more";
    private int _iconCount;
    private bool _showAllIcons = false;

    protected override void OnParametersSet()
    {
        _iconCount = 0;
        switch (SetBy)
        {
            case AvailabilitySetBy.DefaultAvailable:
            case AvailabilitySetBy.Holiday:
                _iconCount++;
                break;
        }
        if (OnClickHistory.HasDelegate)
            _iconCount++;
        if (OnClickSettings.HasDelegate)
            _iconCount++;
    }

    public void Dispose()
    {
    }
}
