using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard;
public sealed partial class Index : IDisposable
{
    [Inject] private IStringLocalizer<Index> L { get; set; } = default!;
    [Inject] private FunctionRepository _functionRepository { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private TrainingTypesRepository _trainingTypesRepository { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private VehicleRepository _vehicleRepository { get; set; } = default!;
    [Inject] private DayItemRepository _calendarItemRepository { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private CustomStateProvider AuthenticationStateProvider { get; set; } = default!;
    private ClaimsPrincipal _userClaims;
    private HubConnection? _hubConnection;
    private CancellationTokenSource _cls = new();
    private DrogeUser? _user;
    private List<DrogeFunction>? _functions;
    private List<PlannedTraining>? _futureTrainings;
    private List<Training>? _pinnedTrainings;
    private List<DrogeUser>? _users;
    private List<DrogeVehicle>? _vehicles;
    private List<PlannerTrainingType>? _trainingTypes;
    private List<RoosterItemDay>? _dayItems;
    private string _name = string.Empty;
    private Guid _userId;
    protected override async Task OnParametersSetAsync()
    {
        if (!await SetUser())
            return;
        await ConfigureHub();

        _users = await _userRepository.GetAllUsersAsync(false, false, true, _cls.Token);
        _vehicles = await _vehicleRepository.GetAllVehiclesAsync();
        _trainingTypes = await _trainingTypesRepository.GetTrainingTypes(false, _cls.Token);
        _functions = await _functionRepository.GetAllFunctionsAsync();
        _dayItems = (await _calendarItemRepository.GetDayItemDashboardAsync(_userId, _cls.Token))?.DayItems;
        StateHasChanged();
        _pinnedTrainings = (await _scheduleRepository.GetPinnedTrainingsForUser(_cls.Token))?.Trainings;
        StateHasChanged();
        _futureTrainings = (await _scheduleRepository.GetScheduledTrainingsForUser(_userId, true, _cls.Token))?.Trainings;
    }

    private async Task<bool> SetUser()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        _userClaims = authState.User;
        if (_userClaims?.Identity?.IsAuthenticated ?? false)
        {
            _name = _userClaims.Identities.FirstOrDefault()!.Claims.FirstOrDefault(x => x.Type.Equals("FullName"))?.Value ?? string.Empty;
            if (!Guid.TryParse(_userClaims.Identities.FirstOrDefault()!.Claims.FirstOrDefault(x => x.Type.Equals("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value, out _userId))
                return false;
        }
        else
        {
            // Should never happen.
            return false;
        }
        _user = await _userRepository.GetCurrentUserAsync();
        return true;
    }

    private async Task ConfigureHub()
    {
        _hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/hub/refresh"))
                .Build();
        _hubConnection.On<ItemUpdated>($"Refresh_{_userId}", async (type) =>
        {
            if (_cls.Token.IsCancellationRequested)
                return;
            switch (type)
            {
                case ItemUpdated.FutureTrainings:
                    _futureTrainings = (await _scheduleRepository.GetScheduledTrainingsForUser(_userId, false, _cls.Token))?.Trainings;
                    StateHasChanged();
                    break;
                case ItemUpdated.AllUsers:
                    _users = await _userRepository.GetAllUsersAsync(false, false, false, _cls.Token);
                    break;
            }
        });
        await _hubConnection.StartAsync(_cls.Token);
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
