﻿using Drogecode.Knrm.Oefenrooster.Client.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;

public sealed partial class Schedule : IDisposable
{
    [Inject] private IStringLocalizer<Schedule> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private TrainingTypesRepository TrainingTypesRepository { get; set; } = default!;
    [Inject] private UserRepository UserRepository { get; set; } = default!;
    [Inject] private FunctionRepository FunctionRepository { get; set; } = default!;
    [Inject] private VehicleRepository VehicleRepository { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter] public Guid CustomerId { get; set; } = Guid.Empty;
    [Parameter] public string? View { get; set; }
    private CancellationTokenSource _cls = new();
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private List<DrogeVehicle>? _vehicles;
    private List<PlannerTrainingType>? _trainingTypes;
    private bool _canEdit;
    private bool _editOnClick;
    private ScheduleView _view = ScheduleView.Calendar;

    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrEmpty(View) && View.Equals("Table"))
            _view = ScheduleView.Table;
        else
            _view = ScheduleView.Calendar;
        _users = await UserRepository.GetAllUsersAsync(false, false, false, _cls.Token);
        _functions = await FunctionRepository.GetAllFunctionsAsync(false, _cls.Token);
        _vehicles = await VehicleRepository.GetAllVehiclesAsync(false, _cls.Token);
        _trainingTypes = await TrainingTypesRepository.GetTrainingTypes(false, false, _cls.Token);
        if (AuthenticationState is not null)
        {
            var authState = await AuthenticationState;
            var user = authState?.User;
            if (user is not null)
            {
                _canEdit = user.IsInRole(AccessesNames.AUTH_scheduler_in_table_view);
            }
        }
    }
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(View) && View.Equals("Table"))
            _view = ScheduleView.Table;
        else
            _view = ScheduleView.Calendar;
    }

    public void ChangeView(ScheduleView newView)
    {
        if (_view == newView) return;
        _view = newView;
        Navigation.NavigateTo($"/planner/schedule/{newView}");
    }

    public Color GetColor(bool active)
    {
        if (active)
        {
            return Color.Default;
        }
        else
        {
            return Color.Dark;
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
