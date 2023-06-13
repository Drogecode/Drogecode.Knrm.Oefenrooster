using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User;

public sealed partial class Defaults : IDisposable
{
    [Inject] private IStringLocalizer<Defaults> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private DefaultScheduleRepository _defaultScheduleRepository { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<DefaultSchedule>? _defaultSchedules { get; set; }
    private bool _updating;
    private bool _success;
    private string[] _errors = Array.Empty<string>();
    [AllowNull] private MudForm _form;

    protected override async Task OnParametersSetAsync()
    {
        _defaultSchedules = await _defaultScheduleRepository.GetAll(_cls.Token);
    }
    private async Task OnChange(Guid id)
    {
        if (_updating) return;
        _updating = true;
        var schedule = _defaultSchedules?.FirstOrDefault(s => s.Id == id);
        if (schedule is not null)
        {
            var patched = await _defaultScheduleRepository.PatchDefaultScheduleForUser(schedule, _cls.Token);
            if (patched is not null)
            {
                schedule.UserDefaultAvailableId = patched.UserDefaultAvailableId;
            }
        }
        _updating = false;
    }
    private async Task OnSubmit(Guid id)
    {
        _updating = true;
        var schedule = _defaultSchedules?.FirstOrDefault(s => s.Id == id);
        _form?.Validate();
        if (!_form?.IsValid == true || schedule is null)
        {
            _updating = false;
            return;
        }
        var patched = await _defaultScheduleRepository.PatchDefaultScheduleForUser(schedule, _cls.Token);
        if (patched is not null)
        {
            schedule.UserDefaultAvailableId = patched.UserDefaultAvailableId;
        }
        _updating = false;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
