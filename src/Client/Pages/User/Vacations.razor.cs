using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Microsoft.Extensions.Localization;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User;

public sealed partial class Vacations : IDisposable
{
    [Inject] private IStringLocalizer<Vacations> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private HolidayRepository _holidayRepository { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<Holiday>? _holidays { get; set; }
    private bool _updating;
    private bool _success;
    private string[] _errors = Array.Empty<string>();
    [AllowNull] private MudForm _form;

    protected override async Task OnParametersSetAsync()
    {
        _holidays = await _holidayRepository.GetAll(_cls.Token);
    }
    private async Task OnChange(Holiday holiday)
    {
        if (_updating) return;
        _updating = true;
        if (holiday is not null)
        {
            var patched = await _holidayRepository.PatchhHolidayForUser(holiday, _cls.Token);
        }
        _updating = false;
    }
    private async Task OnSubmit(Guid id)
    {
        _updating = true;
        var schedule = _holidays?.FirstOrDefault(s => s.Id == id);
        _form?.Validate();
        if (!_form?.IsValid == true || schedule is null)
        {
            _updating = false;
            return;
        }
        var patched = await _holidayRepository.PatchhHolidayForUser(schedule, _cls.Token);
        _updating = false;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
