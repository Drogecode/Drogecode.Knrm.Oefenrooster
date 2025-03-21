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

    protected override async Task OnParametersSetAsync()
    {
        _settingTrainingToCalendar = (await UserSettingsClient.GetBoolSettingAsync(SettingName.TrainingToCalendar)).Value;
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

    private async Task PatchTrainingToCalendar(bool isChecked)
    {
        _settingTrainingToCalendar = isChecked;
        await UserSettingsClient.PatchBoolSettingAsync(new PatchSettingBoolRequest(SettingName.TrainingToCalendar, isChecked));
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