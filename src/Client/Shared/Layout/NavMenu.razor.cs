﻿using Blazored.LocalStorage;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Menu;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Web;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;

public sealed partial class NavMenu : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<NavMenu>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private NavigationManager? Navigation { get; set; }
    [Inject, NotNull] private IDialogService? DialogProvider { get; set; }
    [Inject, NotNull] private VehicleRepository? VehicleRepository { get; set; }
    [Inject, NotNull] private ConfigurationRepository? ConfigurationRepository { get; set; }
    [Inject, NotNull] private UserRepository? UserRepository { get; set; }
    [Inject, NotNull] private ScheduleRepository? ScheduleRepository { get; set; }
    [Inject, NotNull] private TrainingTypesRepository? TrainingTypesRepository { get; set; }
    [Inject, NotNull] private MenuRepository? MenuRepository { get; set; }
    [Inject, NotNull] private ILinkedCustomerClient? LinkedCustomerClient { get; set; }
    [Inject, NotNull] private ILocalStorageService? LocalStorage { get; set; }
    [Inject, NotNull] private IJSRuntime? JsRuntime { get; set; }
    [Inject, NotNull] private CustomStateProvider? AuthenticationStateProvider { get; set; }
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;

    private CancellationTokenSource _cls = new();
    private UserMenuSettings? _userMenuSettings;
    private List<DrogeMenu>? _menuItems;
    private List<LinkedCustomer>? _linkedCustomers;
    private string _uriCalendar = "/planner/calendar";
    private string _uriSchedule = "/planner/schedule";
    private string _uriPlannerUser = "/planner/user";
    private string _sharePointUrl = string.Empty;
    private string _lplhUrl = string.Empty;
    private Guid? _currentCustomer;
    private bool _useFullLinkExpanded;
    private bool _configurationExpanded;
    private bool _changingCustomer;
    private bool _addingTraining;

    protected override async Task OnInitializedAsync()
    {
        Navigation.LocationChanged += RefreshForSubMenu;
        await UserRepository.GetCurrentUserAsync(); //Force creation of user.
    }

    protected override async Task OnParametersSetAsync()
    {
        _menuItems = await MenuRepository.GetAllAsync(false, true, _cls.Token);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _userMenuSettings = (await LocalStorage.GetItemAsync<UserMenuSettings>("userMenuSettings")) ?? new UserMenuSettings();
            // Cannot use the object directly because it freezes the page.
            _useFullLinkExpanded = _userMenuSettings.UseFullLinkExpanded;
            _configurationExpanded = _userMenuSettings.ConfigurationExpanded;
            var linkedCustomers = await LinkedCustomerClient.GetAllCustomersLinkedToMeAsync();
            if (linkedCustomers?.UserLinkedCustomers?.Count > 1)
            {
                _linkedCustomers = linkedCustomers.UserLinkedCustomers;
                _currentCustomer = linkedCustomers.CurrentCustomerId;
            }

            await SetMenu();
            StateHasChanged();
        }
    }

    private string? CustomerIdToName(Guid? customerId)
    {
        return _linkedCustomers?.FirstOrDefault(x => x.CustomerId == customerId)?.Name;
    }

    private async Task OnCustomerChange(Guid? customerId)
    {
        var linkedCustomer = _linkedCustomers?.FirstOrDefault(x => x.CustomerId == customerId);
        if (customerId == _currentCustomer || linkedCustomer is null)
        {
            DebugHelper.WriteLine("Already in this customer");
            return;
        }

        _changingCustomer = true;
        StateHasChanged();
        await AuthenticationStateProvider.SwitchUser(new SwitchUserRequest()
        {
            UserId = linkedCustomer.UserId,
            CustomerId = linkedCustomer.CustomerId
        });
        Navigation.NavigateTo("/", true);
    }

    private async Task SetMenu()
    {
        var menu = await MenuRepository.GetAllAsync(false, false, _cls.Token);
        if (menu is null || !menu.Any()) return;
        _menuItems = menu;
    }

    private void RefreshForSubMenu(object? sender, LocationChangedEventArgs e)
    {
        StateHasChanged();
    }

    private async Task AddTraining()
    {
        if (_addingTraining) return;
        try
        {
            _addingTraining = true;
            StateHasChanged();
            var trainingTypes = await TrainingTypesRepository.GetTrainingTypes(false, false, _cls.Token);
            var vehicles = await VehicleRepository.GetAllVehiclesAsync(false, _cls.Token);
            var parameters = new DialogParameters<EditTrainingDialog>
            {
                { x => x.Planner, null },
                { x => x.Refresh, null },
                { x => x.Vehicles, vehicles },
                { x => x.Global, Global },
                { x => x.TrainingTypes, trainingTypes }
            };
            var options = new DialogOptions
            {
                MaxWidth = MaxWidth.Medium,
                CloseButton = true,
                FullWidth = true,
                BackdropClick = false
            };
            await DialogProvider.ShowAsync<EditTrainingDialog>(L["Add training"], parameters, options);
        }
        finally
        {
            _addingTraining = false;
            StateHasChanged();
        }
    }

    private async Task UseFullLinksExpandedChanged(bool newValue)
    {
        if (_userMenuSettings is null || _userMenuSettings.UseFullLinkExpanded == newValue) return;
        _userMenuSettings.UseFullLinkExpanded = newValue;
        _useFullLinkExpanded = newValue;
        await LocalStorage.SetItemAsync("userMenuSettings", _userMenuSettings);
    }

    private async Task ConfigurationExpandedChanged(bool newValue)
    {
        if (_userMenuSettings is null || _userMenuSettings.ConfigurationExpanded == newValue) return;
        _userMenuSettings.ConfigurationExpanded = newValue;
        _configurationExpanded = newValue;
        await LocalStorage.SetItemAsync("userMenuSettings", _userMenuSettings);
    }

    private async Task ClickExtraMenu(DrogeMenu menu)
    {
        string? url = null;
        string? loginHint = null;
        if (AuthenticationState is not null)
        {
            var authState = await AuthenticationState;
            loginHint = authState.User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;
        }

        if (menu.AddLoginHint != null && loginHint is not null)
        {
            switch (menu.AddLoginHint)
            {
                case '&':
                    url = $"{menu.Url}&login_hint={HttpUtility.UrlEncode(loginHint)}";
                    break;
                case '?':
                    url = $"{menu.Url}?login_hint={HttpUtility.UrlEncode(loginHint)}";
                    break;
            }
        }
        else
        {
            url = menu.Url;
        }

        if (url is not null)
        {
            await JsRuntime.InvokeVoidAsync("open", url, menu.TargetBlank ? "_blank" : "");
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}