﻿using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

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
    private List<PlannedTraining>? _futureTrainings;
    private List<Training>? _pinnedTrainings;
    private List<DrogeUser>? _users;
    private List<DrogeVehicle>? _vehicles;
    private List<PlannerTrainingType>? _trainingTypes;
    private List<RoosterItemDay>? _dayItems;
    private List<Holiday>? _futureHolidays;
    private string? _name;
    private Guid _userId;
    private bool _loading = true;

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
                _futureHolidays = (await _holidayRepository.GetAllFuture(_userId, true, _cls.Token))?.Holidays;
            StateHasChanged();
            _pinnedTrainings = (await ScheduleRepository.GetPinnedTrainingsForUser(_userId, true, _cls.Token))?.Trainings;
            StateHasChanged();
            _futureTrainings = (await ScheduleRepository.GetScheduledTrainingsForUser(_userId, true, _cls.Token))?.Trainings;

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
                return false;
        }
        else
        {
            // Should never happen.
            return false;
        }

        _user = await UserRepository.GetCurrentUserAsync();
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
                try
                {

                    if (_cls.Token.IsCancellationRequested)
                        return;
                    switch (type)
                    {
                        case ItemUpdated.FutureTrainings:
                            _futureTrainings = (await ScheduleRepository.GetScheduledTrainingsForUser(_userId, false, _cls.Token))?.Trainings;
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
                                _futureHolidays = (await _holidayRepository.GetAllFuture(_userId, false, _cls.Token))?.Holidays;
                            break;
                        default:
                            DebugHelper.WriteLine("Missing type, ignored");
                            break;
                    }
                    StateHasChanged();
                }
                catch (HttpRequestException)
                {
                    DebugHelper.WriteLine("c HttpRequestException");
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception e)
                {
                    DebugHelper.WriteLine("On message received from hub");
                    DebugHelper.WriteLine(e);
                    throw;
                }
            });
            await _hubConnection.StartAsync(_cls.Token);
        }
        catch (HttpRequestException)
        {
            DebugHelper.WriteLine("d HttpRequestException");
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception e)
        {
            DebugHelper.WriteLine("Failed to setup hub");
            DebugHelper.WriteLine(e);
        }
    }

    private async Task VisibilityChanged()
    {
        try
        {
            DebugHelper.WriteLine("VisibilityChanged dashboard");
            _users = await UserRepository.GetAllUsersAsync(false, false, false, _cls.Token);
            _vehicles = await VehicleRepository.GetAllVehiclesAsync(true, _cls.Token);
            _trainingTypes = await TrainingTypesRepository.GetTrainingTypes(false, false, _cls.Token);
            _functions = await FunctionRepository.GetAllFunctionsAsync(true, _cls.Token);
            _dayItems = (await CalendarItemRepository.GetDayItemDashboardAsync(_userId, false, _cls.Token))?.DayItems;
            _pinnedTrainings = (await ScheduleRepository.GetPinnedTrainingsForUser(_userId, false, _cls.Token))?.Trainings;
            _futureTrainings = (await ScheduleRepository.GetScheduledTrainingsForUser(_userId, false, _cls.Token))?.Trainings;
            DebugHelper.WriteLine("Dashboard reloaded");
            StateHasChanged();
        }
        catch (Exception e)
        {
            DebugHelper.WriteLine("Failed to recache everything on dashboard after VisibilityChanged");
            DebugHelper.WriteLine(e);
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
        Global.VisibilityChangeAsync -= VisibilityChanged;
    }
}