﻿using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class ScheduleCard : IDisposable
{
    [Inject] private IStringLocalizer<ScheduleCard> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IDialogService DialogProvider { get; set; } = default!;
    [Inject] private CustomStateProvider AuthenticationStateProvider { get; set; } = default!;
    [CascadingParameter] public DrogeCodeGlobal Global { get; set; } = default!;
    [CascadingParameter] public MainLayout MainLayout { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter, EditorRequired] public PlannedTraining Planner { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeUser>? Users { get; set; }
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; }
    [Parameter, EditorRequired] public List<DrogeVehicle>? Vehicles { get; set; }
    [Parameter, EditorRequired] public List<PlannerTrainingType>? TrainingTypes { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string Width { get; set; } = "100%";
    [Parameter] public string? MinWidth { get; set; }
    [Parameter] public string? MaxWidth { get; set; }
    [Parameter] public bool ReplaceEmptyName { get; set; }
    [Parameter] public bool ShowDate { get; set; }
    [Parameter] public bool ShowDayOfWeek { get; set; }
    [Parameter] public bool ShowPastBody { get; set; } = true;
    private RefreshModel _refreshModel = new();
    private Guid _userId;
    private bool _updating;
    private bool _isDeleted;
    private bool _showHistory;
    private bool _showPastBody = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _refreshModel.RefreshRequested += RefreshMe;
            Global.TrainingDeletedAsync += TrainingDeleted;
            _showHistory = await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_scheduler_history);
            await SetUser();
            StateHasChanged();
        }
    }

    protected override void OnInitialized()
    {
        _showPastBody = ShowPastBody;
    }
    
    

    private async Task<bool> SetUser()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userClaims = authState.User;
        if (userClaims?.Identity?.IsAuthenticated ?? false)
        {
            if (!Guid.TryParse(userClaims.Identities.FirstOrDefault()!.Claims.FirstOrDefault(x => x.Type.Equals("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value, out _userId))
                return false;
        }
        else
        {
            // Should never happen.
            return false;
        }
        return true;
    }

    private Task OpenScheduleDialog()
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

        var options = new DialogOptions()
        {
            MaxWidth = MudBlazor.MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        return DialogProvider.ShowAsync<ScheduleDialog>(L["Schedule people for this training"], parameters, options);
    }

    private Task OpenConfigDialog()
    {
        var parameters = new DialogParameters<EditTrainingDialog>
        {
            { x => x.Planner, Planner },
            { x => x.Refresh, _refreshModel },
            { x => x.Vehicles, Vehicles },
            { x => x.Global, Global },
            { x => x.TrainingTypes, TrainingTypes }
        };
        var options = new DialogOptions()
        {
            MaxWidth = MudBlazor.MaxWidth.Large,
            CloseButton = true,
            FullWidth = true
        };
        return DialogProvider.ShowAsync<EditTrainingDialog>(L["Configure training"], parameters, options);
    }

    private Task OpenHistoryDialog()
    {
        var parameters = new DialogParameters<TrainingHistoryDialog>
        {
            { x => x.TrainingId, Planner.TrainingId },
            { x => x.Users, Users },
        };
        var options = new DialogOptions()
        {
            MaxWidth = MudBlazor.MaxWidth.Large,
            CloseButton = true,
            FullWidth = true
        };
        return DialogProvider.ShowAsync<TrainingHistoryDialog>(L["Edit history"], parameters, options);
    }

    private async Task TrainingDeleted(Guid id)
    {
        if (id == Planner.TrainingId)
        {
            _isDeleted = true;
        }
    }

    private void RefreshMe()
    {
        StateHasChanged();
        Refresh?.CallRequestRefresh();
    }

    public void Dispose()
    {
        _refreshModel.RefreshRequested -= RefreshMe;
        Global.TrainingDeletedAsync -= TrainingDeleted;
    }
}