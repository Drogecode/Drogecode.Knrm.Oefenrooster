using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User.Components;

public sealed partial class DefaultPanel : IDisposable
{
    [Inject] private IStringLocalizer<Defaults> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    [Inject] private DefaultScheduleRepository _defaultScheduleRepository { get; set; } = default!;
    [Parameter][EditorRequired] public DefaultGroup Group { get; set; } = default!;
    private readonly RefreshModel _refreshModel = new();
    private readonly CancellationTokenSource _cls = new();
    private List<DefaultSchedule>? _defaultSchedules;
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
        return _dialogProvider.ShowAsync<GroupDialog>(L["Add period"], parameters, options);
    }
    
    private async Task OnChange(Availability? value, DefaultUserSchedule? old, Guid defaultId)
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
        }
        _updating = false;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
