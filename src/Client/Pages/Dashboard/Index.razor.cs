using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
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
    protected override async Task OnParametersSetAsync()
    {
        _user = await _userRepository.GetCurrentUserAsync();
        await ConfigureHub();

        _users = await _userRepository.GetAllUsersAsync(false, false, _cls.Token);
        _vehicles = await _vehicleRepository.GetAllVehiclesAsync();
        _trainingTypes = await _trainingTypesRepository.GetTrainingTypes(false, _cls.Token);
        _functions = await _functionRepository.GetAllFunctionsAsync();
        _dayItems = (await _calendarItemRepository.GetDayItemDashboardAsync(_user?.Id, _cls.Token))?.DayItems;
        StateHasChanged();
        _pinnedTrainings = (await _scheduleRepository.GetPinnedTrainingsForUser(_cls.Token))?.Trainings;
        StateHasChanged();
        _futureTrainings = (await _scheduleRepository.GetScheduledTrainingsForUser(_user?.Id, true, _cls.Token))?.Trainings;
    }

    private async Task ConfigureHub()
    {
        if (_user?.Id is not null)
        {
            _hubConnection = new HubConnectionBuilder()
                    .WithUrl(Navigation.ToAbsoluteUri("/hub/refresh"))
                    .Build();
            _hubConnection.On<ItemUpdated>($"Refresh_{_user.Id}", async (type) =>
            {
                switch (type)
                {
                    case ItemUpdated:
                        _futureTrainings = (await _scheduleRepository.GetScheduledTrainingsForUser(_user?.Id, false, _cls.Token))?.Trainings;
                        StateHasChanged();
                        break;
                }
            });
            await _hubConnection.StartAsync(_cls.Token);
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
