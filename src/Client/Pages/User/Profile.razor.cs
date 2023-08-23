using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User;

public sealed partial class Profile
{
    [Inject] private IUserSettingsClient UserSettingsClient { get; set; } = default!;

    private bool _settingTrainingToCalendar;

    protected override async Task OnParametersSetAsync()
    {
        _settingTrainingToCalendar = await UserSettingsClient.GetTrainingToCalendarAsync();
    }

    private async Task PatchTrainingToCalendar(bool isChecked)
    {
        _settingTrainingToCalendar = isChecked;
        await UserSettingsClient.PatchTrainingToCalendarAsync(isChecked);
    }
}
