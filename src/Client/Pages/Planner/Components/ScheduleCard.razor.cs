using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
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
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    [CascadingParameter] public DrogeCodeGlobal Global { get; set; } = default!;
    [CascadingParameter] public MainLayout MainLayout { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter, EditorRequired] public PlannedTraining Planner { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeUser>? Users { get; set; }
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; }
    [Parameter, EditorRequired] public List<DrogeVehicle>? Vehicles { get; set; }
    [Parameter, EditorRequired] public List<PlannerTrainingType>? TrainingTypes { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string Width { get; set; } = "100%";
    [Parameter] public string? MinWidth { get; set; }
    [Parameter] public string? MaxWidth { get; set; }
    [Parameter] public bool ReplaceEmtyName { get; set; }
    [Parameter] public bool ShowDate { get; set; }
    [Parameter] public bool ShowDayOfWeek { get; set; }
    [Parameter] public bool ShowPastBody { get; set; } = true;
    private RefreshModel _refreshModel = new();
    private bool _updating;
    private bool _isDelted;
    private bool _showHistory;

    protected override async Task OnParametersSetAsync()
    {
        _refreshModel.RefreshRequested += RefreshMe;
        Global.TrainingDeletedAsync += TrainingDeleted;
        if (AuthenticationState is not null)
        {
            var authState = await AuthenticationState;
            var user = authState?.User;
            if (user is not null)
            {
                _showHistory = user.IsInRole(AccessesNames.AUTH_scheduler_history);
            }
        }
    }

    private void OpenScheduleDialog()
    {
        var parameters = new DialogParameters<ScheduleDialog>
        {
            { x => x.Planner, Planner },
            { x => x.Refresh, _refreshModel },
            { x => x.Users, Users },
            { x => x.Functions, Functions },
            { x => x.Vehicles, Vehicles },
            { x => x.MainLayout, MainLayout },
        };

        DialogOptions options = new DialogOptions()
        {
            MaxWidth = MudBlazor.MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        _dialogProvider.Show<ScheduleDialog>(L["Schedule people for this training"], parameters, options);
    }

    private void OpenConfigDialog()
    {
        var parameters = new DialogParameters<EditTrainingDialog>
        {
            { x=>x.Planner, Planner },
            { x=>x.Refresh, _refreshModel },
            { x=>x.Vehicles, Vehicles },
            { x=>x.Global, Global },
            { x=>x.TrainingTypes, TrainingTypes }
        };
        var options = new DialogOptions()
        {
            MaxWidth = MudBlazor.MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        _dialogProvider.Show<EditTrainingDialog>(L["Configure training"], parameters, options);
    }

    private void OpenHistoryDialog()
    {
        var parameters = new DialogParameters<TrainingHistoryDialog>
        {
            { x=>x.TrainingId, Planner.TrainingId},
            { x=>x.Users, Users},
        };
        var options = new DialogOptions()
        {
            MaxWidth = MudBlazor.MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        _dialogProvider.Show<TrainingHistoryDialog>(L["Edit history"], parameters, options);
    }

    private async Task TrainingDeleted(Guid id)
    {
        if (id == Planner.TrainingId)
        {
            _isDelted = true;
        }
    }

    private void RefreshMe()
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        _refreshModel.RefreshRequested -= RefreshMe;
        Global.TrainingDeletedAsync -= TrainingDeleted;
    }
}
