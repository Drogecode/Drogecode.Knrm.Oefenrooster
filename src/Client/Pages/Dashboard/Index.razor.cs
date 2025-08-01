﻿using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.AspNetCore.SignalR.Client;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard;

public sealed partial class Index : IDisposable
{
    [Inject] private IStringLocalizer<Index> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IStringLocalizer<DateToString> LDateToString { get; set; } = default!;
    [Inject] private FunctionRepository FunctionRepository { get; set; } = default!;
    [Inject] private ScheduleRepository ScheduleRepository { get; set; } = default!;
    [Inject] private TrainingTypesRepository TrainingTypesRepository { get; set; } = default!;
    [Inject] private UserRepository UserRepository { get; set; } = default!;
    [Inject] private VehicleRepository VehicleRepository { get; set; } = default!;
    [Inject] private DayItemRepository CalendarItemRepository { get; set; } = default!;
    [Inject] private HolidayRepository _holidayRepository { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private CustomStateProvider AuthenticationStateProvider { get; set; } = default!;
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    private readonly CancellationTokenSource _cls = new();
    private ClaimsPrincipal? _userClaims;
    private HubConnection? _hubConnection;
    private DrogeUser? _user;
    private List<DrogeFunction>? _functions;
    private GetScheduledTrainingsForUserResponse? _trainings;
    private List<Training>? _pinnedTrainings;
    private List<DrogeUser>? _users;
    private List<DrogeVehicle>? _vehicles;
    private List<PlannerTrainingType>? _trainingTypes;
    private List<RoosterItemDay>? _dayItems;
    private MultipleHolidaysResponse? _futureHolidays;
    private string? _name;
    private Guid _userId;
    private bool _loading = true;
    private bool _allHolidays;
    private const int TAKE = 15;
    private int _total = TAKE;
    private int _skip = 0;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (!await SetUser())
                return;
            await ConfigureHub();

            _users = await UserRepository.GetAllUsersAsync(false, false, true, _cls.Token);
            _vehicles = await VehicleRepository.GetAllVehiclesAsync(true, _cls.Token);
            _trainingTypes = await TrainingTypesRepository.GetTrainingTypes(false, true, _cls.Token);
            _functions = await FunctionRepository.GetAllFunctionsAsync(true, _cls.Token);
            _dayItems = (await CalendarItemRepository.GetDayItemDashboardAsync(_userId, true, _cls.Token))?.DayItems;
            if (await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_dashboard_holidays))
                _futureHolidays = await _holidayRepository.GetAllFuture(_userId, true, 30, _cls.Token);
            _pinnedTrainings = (await ScheduleRepository.GetPinnedTrainingsForUser(_userId, true, _cls.Token))?.Trainings;
            _trainings = await ScheduleRepository.GetScheduledTrainingsForUser(_userId, true, TAKE, _skip * TAKE, _cls.Token);

            Global.VisibilityChangeAsync += VisibilityChanged;
            _loading = false;
            StateHasChanged();
        }
    }

    private async Task<bool> SetUser()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        _userClaims = authState.User;
        if (_userClaims?.Identity?.IsAuthenticated ?? false)
        {
            _name = _userClaims.Identities.FirstOrDefault()!.Claims.FirstOrDefault(x => x.Type.Equals("FullName"))?.Value ?? string.Empty;
            if (!Guid.TryParse(_userClaims.Identities.FirstOrDefault()!.Claims.FirstOrDefault(x => x.Type.Equals("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value, out _userId))
            {
                DebugHelper.WriteLine("Dashboard - Failed to get user id");
                return false;
            }
        }
        else
        {
            // Should never happen.
            DebugHelper.WriteLine("Dashboard - Failed to get user claims");
            return false;
        }

        _user = await UserRepository.GetCurrentUserAsync();
        StateHasChanged();
        return true;
    }

    private async Task ConfigureHub()
    {
        try
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/hub/refresh"))
                .WithAutomaticReconnect()
                .Build();
            _hubConnection.On<ItemUpdated>($"Refresh_{_userId}", async (type) =>
            {
                if (_cls.Token.IsCancellationRequested)
                    return;
                var stateHasChanged = true;
                switch (type)
                {
                    case ItemUpdated.FutureTrainings:
                        _trainings = await ScheduleRepository.GetScheduledTrainingsForUser(_userId, false, TAKE, _skip * TAKE, _cls.Token);
                        break;
                    case ItemUpdated.AllUsers:
                        _users = await UserRepository.GetAllUsersAsync(false, false, false, _cls.Token);
                        break;
                    case ItemUpdated.AllVehicles:
                        _vehicles = await VehicleRepository.GetAllVehiclesAsync(false, _cls.Token);
                        break;
                    case ItemUpdated.AllTrainingTypes:
                        _trainingTypes = await TrainingTypesRepository.GetTrainingTypes(false, false, _cls.Token);
                        break;
                    case ItemUpdated.AllFunctions:
                        _functions = await FunctionRepository.GetAllFunctionsAsync(false, _cls.Token);
                        break;
                    case ItemUpdated.DayItemDashboard:
                        _dayItems = (await CalendarItemRepository.GetDayItemDashboardAsync(_userId, false, _cls.Token))?.DayItems;
                        break;
                    case ItemUpdated.PinnedDashboard:
                        _pinnedTrainings = (await ScheduleRepository.GetPinnedTrainingsForUser(_userId, false, _cls.Token))?.Trainings;
                        break;
                    case ItemUpdated.FutureHolidays:
                        if (await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_dashboard_holidays))
                            _futureHolidays = await _holidayRepository.GetAllFuture(_userId, false, 30, _cls.Token);
                        break;
                    default:
                        stateHasChanged = false;
                        break;
                }

                if (stateHasChanged)
                    StateHasChanged();
            });
            await _hubConnection.StartAsync(_cls.Token);
        }
        catch (HttpRequestException)
        {
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception e)
        {
            DebugHelper.WriteLine(e);
        }
    }

    private async Task VisibilityChanged()
    {
        try
        {
            _users = await UserRepository.GetAllUsersAsync(false, false, false, _cls.Token);
            _vehicles = await VehicleRepository.GetAllVehiclesAsync(true, _cls.Token);
            _trainingTypes = await TrainingTypesRepository.GetTrainingTypes(false, false, _cls.Token);
            _functions = await FunctionRepository.GetAllFunctionsAsync(true, _cls.Token);
            _dayItems = (await CalendarItemRepository.GetDayItemDashboardAsync(_userId, false, _cls.Token))?.DayItems;
            _pinnedTrainings = (await ScheduleRepository.GetPinnedTrainingsForUser(_userId, false, _cls.Token))?.Trainings;
            _trainings = await ScheduleRepository.GetScheduledTrainingsForUser(_userId, false, TAKE, _skip * TAKE, _cls.Token);
            StateHasChanged();
        }
        catch (Exception e)
        {
            DebugHelper.WriteLine("Failed to recache everything on dashboard after VisibilityChanged");
            DebugHelper.WriteLine(e);
        }
    }

    private async Task LoadMore()
    {
        _skip++;
        var newTrainings = (await ScheduleRepository.AllTrainingsForUser(_userId, TAKE, _skip * TAKE, _cls.Token));
        if (_trainings is null || newTrainings is null || newTrainings.TotalCount != _trainings.TotalCount)
        {
            _trainings = newTrainings;
            _total = TAKE;
            _skip = 0;
        }
        else
        {
            _trainings.Trainings.AddRange(newTrainings.Trainings);
            _total += TAKE;
        }

        StateHasChanged();
    }

    private async Task LoadYearHolidays()
    {
        _allHolidays = true;
        if (await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_dashboard_holidays))
            _futureHolidays = await _holidayRepository.GetAllFuture(_userId, false, 365, _cls.Token);
    }

    public void Dispose()
    {
        _cls.Cancel();
        Global.VisibilityChangeAsync -= VisibilityChanged;
    }
}