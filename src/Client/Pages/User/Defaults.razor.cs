using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.User.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using static MudBlazor.CategoryTypes;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User;

public sealed partial class Defaults : IDisposable
{
    [Inject] private IStringLocalizer<Defaults> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    [Inject] private DefaultScheduleRepository _defaultScheduleRepository { get; set; } = default!;
    private RefreshModel _refreshModel = new();
    private CancellationTokenSource _cls = new();
    private List<DefaultSchedule>? _defaultSchedules { get; set; }
    private bool _updating;
    private bool _success;
    private string[] _errors = Array.Empty<string>();
    [AllowNull] private MudForm _form;

    protected override async Task OnParametersSetAsync()
    {
        _defaultSchedules = (await _defaultScheduleRepository.GetAll(_cls.Token))?.OrderBy(x => x.Order).ThenBy(x => x.TimeStart).ToList();
        _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
    }
    private async Task OnChange(DefaultUserSchedule old, Guid defaultId)
    {
        if (_updating) return;
        _updating = true;
        if (old is not null)
        {
            var body = old.ToPatchDefaultUserSchedule();
            body.DefaultId = defaultId;
            var patched = await _defaultScheduleRepository.PatchDefaultScheduleForUser(body, _cls.Token);
            /*if (patched is not null)
            {
                schedule.UserDefaultAvailableId = patched.UserDefaultAvailableId;
            }*/
        }
        _updating = false;
    }
    private async Task OnSubmit(DefaultUserSchedule old, Guid defaultId)
    {
        _updating = true;
        var schedule = _defaultSchedules?.FirstOrDefault(s => s.Id == defaultId);
        _form?.Validate();
        if (!_form?.IsValid == true || schedule is null)
        {
            _updating = false;
            return;
        }
        var body = old.ToPatchDefaultUserSchedule();
        body.DefaultId = defaultId;
        var patched = await _defaultScheduleRepository.PatchDefaultScheduleForUser(body, _cls.Token);
        /*if (patched is not null)
        {
            schedule.UserDefaultAvailableId = patched.UserDefaultAvailableId;
        }*/
        _updating = false;
    }

    private void OpenEditDefaultUserScheduleDialog(DefaultUserSchedule? defaultUserSchedule, Guid defaultId, bool isNew)
    {
        var parameters = new DialogParameters<DefaultDialog> {
            { x=> x.DefaultUserSchedule, defaultUserSchedule },
            { x=> x.IsNew, isNew},
            { x=> x.Refresh, _refreshModel },
            { x=> x.DefaultId, defaultId},
        };
        DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        _dialogProvider.Show<DefaultDialog>(L["Edit default"], parameters, options);
    }
    private async Task RefreshMeAsync()
    {
        _defaultSchedules = (await _defaultScheduleRepository.GetAll(_cls.Token))?.OrderBy(x => x.Order).ThenBy(x => x.TimeStart).ToList();
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
