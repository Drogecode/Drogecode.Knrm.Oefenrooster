using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Components;

public sealed partial class CalendarBaseCard : IDisposable
{
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Parameter, EditorRequired] public TrainingAdvance Training { get; set; } = default!;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback OnClickCallback { get; set; }
    [Parameter] public EventCallback OnClickSettings { get; set; }
    [Parameter] public string? ContentClass { get; set; }
    private CancellationTokenSource _cls = new();

    private string HeaderClass
    {
        get
        {
            switch (Training.TrainingType)
            {
                case TrainingType.EHBO:
                    return "DrogeCode-card-header-ehbo";
                case TrainingType.OneOnOne:
                    return "DrogeCode-card-header-one_on_one";
                case TrainingType.FireBrigade:
                    return "DrogeCode-card-header-fire-brigade";
                case TrainingType.HRB:
                    return "DrogeCode-card-header-hrb";
                case TrainingType.Default:
                default:
                    return "DrogeCode-card-header-default";
            }
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
