using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User;

public sealed partial class Profile
{
    [Inject] private IStringLocalizer<Profile> L { get; set; } = default!;
    [Inject] private ConfigurationRepository ConfigurationRepository { get; set; } = default!;
    [Inject] private IUserSettingsClient UserSettingsClient { get; set; } = default!;

    private VersionDetailResponse? _updateDetails;
    private bool? _settingTrainingToCalendar;

    protected override async Task OnParametersSetAsync()
    {
        _settingTrainingToCalendar = await UserSettingsClient.GetTrainingToCalendarAsync();
        _updateDetails = await ConfigurationRepository.NewVersionAvailable();
    }

    private async Task PatchTrainingToCalendar(bool isChecked)
    {
        _settingTrainingToCalendar = isChecked;
        await UserSettingsClient.PatchTrainingToCalendarAsync(isChecked);
    }
}
