using Drogecode.Knrm.Oefenrooster.Client.Enums;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;

public sealed partial class Schedule : IDisposable
{
    [Inject] private IStringLocalizer<Schedule> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private TrainingTypesRepository _trainingTypesRepository { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private FunctionRepository _functionRepository { get; set; } = default!;
    [Inject] private VehicleRepository _vehicleRepository { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter] public Guid CustomerId { get; set; } = Guid.Empty;
    [Parameter] public string? View { get; set; }
    [Parameter] public int? Year { get; set; }
    [Parameter] public int? Month { get; set; }
    private CancellationTokenSource _cls = new();
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private List<DrogeVehicle>? _vehicles;
    private List<PlannerTrainingType>? _trainingTypes;
    private DateTime? _currentMonth;
    private bool _canEdit;
    private bool _editOnClick;
    private ScheduleView _view = ScheduleView.Calendar;

    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrEmpty(View) && View.Equals("Table"))
            _view = ScheduleView.Table;
        else
            _view = ScheduleView.Calendar;
        _users = await _userRepository.GetAllUsersAsync(false);
        _functions = await _functionRepository.GetAllFunctionsAsync();
        _vehicles = await _vehicleRepository.GetAllVehiclesAsync();
        _trainingTypes = await _trainingTypesRepository.GetTrainingTypes(_cls.Token);
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
        if (Year is not null && Month is not null)
            _currentMonth = new DateTime(Year.Value, Month.Value, 1);
        else
            _currentMonth = DateTime.Today;
    }

    public void ChangeView(ScheduleView newView)
    {
        if (_view == newView) return;
        _view = newView;
        var navTo = "/planner/schedule/{newView}";
        if (_currentMonth is not null)
            navTo += $"/{_currentMonth.Value.Year}/{_currentMonth.Value.Month}";
        Navigation.NavigateTo(navTo);
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
