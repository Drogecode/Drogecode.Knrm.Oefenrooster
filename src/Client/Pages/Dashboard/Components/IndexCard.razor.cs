using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public sealed partial class IndexCard : IDisposable
{
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Parameter, EditorRequired] public DrogeUser? User { get; set; }
    [Parameter, EditorRequired] public PlannedTraining Training { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; }
    [Parameter, EditorRequired] public List<DrogeVehicle>? Vehicles { get; set; }
    [Parameter, EditorRequired] public List<PlannerTrainingType>? TrainingTypes { get; set; }
    [Parameter, EditorRequired] public List<DrogeUser>? Users { get; set; }
    [Parameter] public string? MinWidth { get; set; }
    [Parameter] public string? MaxWidth { get; set; }
    [Parameter] public bool ShowDayOfWeek { get; set; }
    private CancellationTokenSource _cls = new();
    private PlanUser? _planUser;

    protected override void OnParametersSet()
    {
        _planUser = Training.PlanUsers.FirstOrDefault(x => x.UserId == User?.Id);
        if (_planUser?.Availability == Availability.None)
            _planUser.Availability = null;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
