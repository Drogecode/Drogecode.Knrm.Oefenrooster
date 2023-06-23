using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class ScheduleCard : IDisposable
{
    [Inject] private IStringLocalizer<ScheduleCard> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    [Inject] private ISnackbar SnackbarService { get; set; } = default!;
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter, EditorRequired] public PlannedTraining Planner { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeUser>? Users { get; set; }
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; }
    [Parameter, EditorRequired] public List<DrogeVehicle>? Vehicles { get; set; }
    [Parameter, EditorRequired] public List<PlannerTrainingType>? TrainingTypes { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string Width { get; set; } = "100%";
    [Parameter] public bool ReplaceEmtyName { get; set; }
    [Parameter] public bool ShowDate { get; set; }
    private RefreshModel _refreshModel = new();
    private bool _updating;

    protected override void OnParametersSet()
    {
        _refreshModel.RefreshRequested += RefreshMe;
    }

    private void OpenScheduleDialog()
    {
        var parameters = new DialogParameters {
                { "Planner", Planner },
                { "Refresh", _refreshModel },
                { "Users", Users },
                { "Functions", Functions},
                { "Vehicles", Vehicles }
            };
        DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        _dialogProvider.Show<ScheduleDialog>(L["Schedule people for this training"], parameters, options);
    }

    private void OpenConfigDialog()
    {
        var parameters = new DialogParameters {
            { "Planner", Planner },
            { "Refresh", _refreshModel },
            { "Vehicles", Vehicles },
            { "Global", Global },
            { "TrainingTypes", TrainingTypes }
        };
        var options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        _dialogProvider.Show<EditTrainingDialog>(L["Configure training"], parameters, options);
    }

    private void OpenHistoryDialog()
    {
        var parameters = new DialogParameters
        {
        };
        var options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        _dialogProvider.Show<TrainingHistoryDialog>(L["Edit history"], parameters, options);
    }

    private void RefreshMe()
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        _refreshModel.RefreshRequested -= RefreshMe;
    }
}
