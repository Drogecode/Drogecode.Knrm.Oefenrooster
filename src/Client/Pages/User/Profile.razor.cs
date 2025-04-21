using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.User.Components;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Setting;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User;

public sealed partial class Profile : IDisposable
{
    [Inject] private IStringLocalizer<Profile> L { get; set; } = default!;
    [Inject] private IDialogService DialogProvider { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ConfigurationRepository ConfigurationRepository { get; set; } = default!;
    [Inject] private IUserSettingsClient UserSettingsClient { get; set; } = default!;
    [Inject] private IUserLinkedMailsClient UserLinkedMailsClient { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }

    private VersionDetailResponse? _updateDetails;
    private AllUserLinkedMailResponse? _allUserLinkedMailResponse;
    private RefreshModel _refreshModel = new();
    private readonly CancellationTokenSource _cls = new();
    private bool? _settingTrainingToCalendar;
    private bool? _settingPreComSyncCalendar;
    private bool? _settingSyncPreComDeleteOld;
    private string? _preComAvailableText;

    protected override async Task OnParametersSetAsync()
    {
        _settingTrainingToCalendar = (await UserSettingsClient.GetBoolSettingAsync(SettingName.TrainingToCalendar)).Value;
        _settingPreComSyncCalendar = (await UserSettingsClient.GetBoolSettingAsync(SettingName.SyncPreComWithCalendar)).Value;
        _settingSyncPreComDeleteOld = (await UserSettingsClient.GetBoolSettingAsync(SettingName.SyncPreComDeleteOld)).Value;
        _preComAvailableText = (await UserSettingsClient.GetStringSettingAsync(SettingName.PreComAvailableText)).Value;
        _updateDetails = await ConfigurationRepository.NewVersionAvailable();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_mail_invite_external))
            {
                _allUserLinkedMailResponse = await UserLinkedMailsClient.AllUserLinkedMailAsync(10, 0);
            }

            _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
            StateHasChanged();
        }
    }

    private async Task PatchSetting(bool isChecked, SettingName settingName)
    {
        switch (settingName)
        {
            case SettingName.TrainingToCalendar:
                _settingTrainingToCalendar = isChecked;
                break;
            case SettingName.SyncPreComWithCalendar:
                _settingPreComSyncCalendar = isChecked;
                break;
            case SettingName.SyncPreComDeleteOld:
                _settingSyncPreComDeleteOld = isChecked;
                break;
        }
        await UserSettingsClient.PatchBoolSettingAsync(new PatchSettingBoolRequest(settingName, isChecked));
    }
    
    private async Task PatchPreComAvailableText(string newValue)
    {
        _preComAvailableText = newValue;
        var body = new PatchSettingStringRequest(SettingName.PreComAvailableText, newValue);
        await UserSettingsClient.PatchStringSettingAsync(body);
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
        var changeResponse = await UserLinkedMailsClient.IsEnabledChangedAsync(new IsEnabledChangedRequest() { IsEnabled = newValue, UserLinkedMailId = emailId });
        if (changeResponse.Success)
        {
            await RefreshMeAsync();
        }
    }

    private async Task RefreshMeAsync()
    {
        if (await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_mail_invite_external))
        {
            _allUserLinkedMailResponse = await UserLinkedMailsClient.AllUserLinkedMailAsync(10, 0);
        }

        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
        _refreshModel.RefreshRequestedAsync -= RefreshMeAsync;
    }
}