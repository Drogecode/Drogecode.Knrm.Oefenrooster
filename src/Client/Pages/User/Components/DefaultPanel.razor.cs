using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User.Components;

public sealed partial class DefaultPanel
{
    [Inject] private IStringLocalizer<Defaults> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    [Inject] private DefaultScheduleRepository _defaultScheduleRepository { get; set; } = default!;
    [Parameter][EditorRequired] public DefaultGroup Group { get; set; } = default!;
    private RefreshModel _refreshModel = new();
    private CancellationTokenSource _cls = new();
    private List<DefaultSchedule>? _defaultSchedules { get; set; }
    private bool _updating;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _defaultSchedules = (await _defaultScheduleRepository.GetAllByGroupId(Group.Id, _cls.Token))?.OrderBy(x => x.Order).ThenBy(x => x.TimeStart).ToList();
            _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
            StateHasChanged();
        }
    }
    private async Task RefreshMeAsync()
    {
        _defaultSchedules = (await _defaultScheduleRepository.GetAllByGroupId(Group.Id, _cls.Token))?.OrderBy(x => x.Order).ThenBy(x => x.TimeStart).ToList();
        StateHasChanged();
    }

    private void OpenGroupDialog(DefaultGroup? group, bool isNew)
    {
        var parameters = new DialogParameters<GroupDialog> {
            { x=>x.DefaultGroup, group},
            { x=> x.IsNew, isNew},
            { x=> x.Refresh, _refreshModel },
        };
        DialogOptions options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        _dialogProvider.Show<GroupDialog>(L["Add group"], parameters, options);
    }
    
    private async Task OnChange(Availability? value, DefaultUserSchedule? old, Guid defaultId, bool isNew)
    {
        if (_updating) return;
        _updating = true;
        if (old is not null && value is not null)
        {
            old.GroupId = Group.Id;
            old.Availability = value;
            var body = old.ToPatchDefaultUserSchedule();
            body.DefaultId = defaultId;
            var patched = await _defaultScheduleRepository.PatchDefaultScheduleForUser(body, _cls.Token);
            if (patched is not null)
            {
                old.UserDefaultAvailableId = patched.UserDefaultAvailableId;
            }
            if (isNew)
            {
                await RefreshMeAsync();
            }
        }
        _updating = false;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
