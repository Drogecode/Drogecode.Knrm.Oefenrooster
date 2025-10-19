using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.User.Components;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User;

public sealed partial class Defaults : IDisposable
{
    [Inject] private IStringLocalizer<Defaults> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IDialogService DialogProvider { get; set; } = default!;
    [Inject] private DefaultScheduleRepository _defaultScheduleRepository { get; set; } = default!;
    private readonly RefreshModel _refreshModel = new();
    private readonly CancellationTokenSource _cls = new();
    private List<DefaultGroup>? _defaultGroups;

    protected override async Task OnParametersSetAsync()
    {
        _defaultGroups = (await _defaultScheduleRepository.GetAllGroups(_cls.Token))?.OrderBy(x => x.ValidFrom).ToList();
        _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
    }

    private string GetGroupTitle(DefaultGroup group)
    {
        var groupName = group.Name;
        if (group.IsDefault && string.IsNullOrEmpty(group.Name))
        {
            groupName = L["Default period name"];
        }
        if (group.IsDefault)
            return groupName;
        return $"{groupName} {group.ValidFrom!.Value.ToLocalTime().ToShortDateString()} {LApp["till"]} {group.ValidUntil!.Value.ToLocalTime().ToShortDateString()}";
    }

    private Task OpenGroupDialog(DefaultGroup? group, bool isNew)
    {
        var parameters = new DialogParameters<GroupDialog> {
            { x=>x.DefaultGroup, group},
            { x=> x.IsNew, isNew},
            { x=> x.Refresh, _refreshModel },
        };
        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        return DialogProvider.ShowAsync<GroupDialog>(L["Add period"], parameters, options);
    }

    private async Task RefreshMeAsync()
    {
        _defaultGroups = (await _defaultScheduleRepository.GetAllGroups(_cls.Token))?.OrderBy(x => x.ValidFrom).ToList();
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
