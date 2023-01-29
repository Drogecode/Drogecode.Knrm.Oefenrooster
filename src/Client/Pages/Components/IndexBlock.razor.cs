using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Components;

public sealed partial class IndexBlock : IDisposable
{
    [Inject] private IStringLocalizer<IndexBlock> L { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Parameter, EditorRequired] public Training Training { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private bool _updating;
    private Color ColorHeader
    {
        get
        {
            switch (Training.Availabilty)
            {
                case Availabilty.Available:
                    return Color.Success;
                case Availabilty.NotAvailable:
                    return Color.Error;
                case Availabilty.Maybe:
                    return Color.Warning;
                case Availabilty.None:
                default:
                    return Color.Inherit;
            }
        }
    }

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
