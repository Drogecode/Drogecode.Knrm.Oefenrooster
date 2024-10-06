using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.User.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User;

public sealed partial class Profile : IDisposable
{
    [Inject] private IStringLocalizer<Profile> L { get; set; } = default!;
    [Inject] private IDialogService DialogProvider { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ConfigurationRepository ConfigurationRepository { get; set; } = default!;
    [Inject] private IUserSettingsClient UserSettingsClient { get; set; } = default!;
    [Inject] private IUserLinkedMailsClient UserLinkedMailsClient { get; set; } = default!;

    private VersionDetailResponse? _updateDetails;
    private AllUserLinkedMailResponse? _allUserLinkedMailResponse;
    private RefreshModel _refreshModel = new();
    private readonly CancellationTokenSource _cls = new();
    private bool? _settingTrainingToCalendar;

    protected override async Task OnParametersSetAsync()
    {
        _settingTrainingToCalendar = await UserSettingsClient.GetTrainingToCalendarAsync();
        _updateDetails = await ConfigurationRepository.NewVersionAvailable();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _allUserLinkedMailResponse = await UserLinkedMailsClient.AllUserLinkedMailAsync(10, 0);
            _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
            StateHasChanged();
        }
    }

    private async Task PatchTrainingToCalendar(bool isChecked)
    {
        _settingTrainingToCalendar = isChecked;
        await UserSettingsClient.PatchTrainingToCalendarAsync(isChecked);
    }

    private async Task OpenActivationDialog(Guid emailId)
    {
        var parameters = new DialogParameters<ActivateEmailDialog>
        {
            { x => x.EmailId, emailId },
            { x => x.Refresh, _refreshModel }
        };
        var options = new DialogOptions
        {
            MaxWidth = MudBlazor.MaxWidth.Small,
            CloseButton = true,
            FullWidth = true
        };
        await DialogProvider.ShowAsync<ActivateEmailDialog>(L["Activate email"], parameters, options);
    }

    private async Task IsEnabledChanged(bool newValue, Guid emailId)
    {
        var changeResponse = await UserLinkedMailsClient.IsEnabledChangedAsync(new IsEnabledChangedRequest() { IsEnabled = newValue, UserLinkedMailId = emailId});
        if (changeResponse.Success)
        {
            await RefreshMeAsync();
        }
    }

    private async Task RefreshMeAsync()
    {
        _allUserLinkedMailResponse = await UserLinkedMailsClient.AllUserLinkedMailAsync(10, 0);
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
        _refreshModel.RefreshRequestedAsync -= RefreshMeAsync;
    }
}