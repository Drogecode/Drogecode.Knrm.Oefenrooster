using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User;

public sealed partial class Defaults : IDisposable
{
    [Inject] private IStringLocalizer<Defaults> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private DefaultScheduleRepository _defaultScheduleRepository { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<DefaultSchedule>? _defaultSchedules { get; set; }
    private bool _updating;

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

    public void Dispose()
    {
        _cls.Cancel();
    }
}
