using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class ScheduleCard : IDisposable
{
    [Inject] private IStringLocalizer<ScheduleCard> L { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    [Inject] private ISnackbar SnackbarService { get; set; } = default!;
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter, EditorRequired] public PlannedTraining Planner { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeUser>? Users { get; set; }
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; }
    [Parameter, EditorRequired] public List<DrogeVehicle>? Vehicles { get; set; }
    [Parameter, EditorRequired] public string Style { get; set; } = default!;
    private RefreshModel _refreshModel = new();
    private bool _updating;

    protected override void OnParametersSet()
    {
        _refreshModel.RefreshRequested += RefreshMe;
    }

    private void OpenScheduleDialog()
    {
        if (Planner.IsCreated)
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
        else
        {
            SnackbarService.Add(L["Requires one person to set their availability"]);
        }
    }

    private void OpenConfigDialog()
    {
        var parameters = new DialogParameters {
            { "Planner", Planner },
            { "Refresh", _refreshModel },
            { "Vehicles", Vehicles },
            { "Global", Global }
        };
        var options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        _dialogProvider.Show<EditTrainingDialog>(L["Configure training"], parameters, options);
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
