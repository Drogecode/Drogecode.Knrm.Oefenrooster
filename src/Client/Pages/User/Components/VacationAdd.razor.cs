using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User.Components;

public sealed partial class VacationAdd : IDisposable
{
    [Inject] private HolidayRepository _holidayRepository { get; set; } = default!;
    [Inject] private IStringLocalizer<VacationAdd> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Parameter] public Guid? Id { get; set; }

    private CancellationTokenSource _cls = new();
    private Holiday? _holiday;
    private Holiday? _originalHoliday;
    private bool _isNew = true;
    private string? _error;
    protected override async Task OnParametersSetAsync()
    {
        if (Id is null)
        {
            _holiday = new Holiday();
        }
        else
        {
            _holiday = await _holidayRepository.Get(Id.Value, _cls.Token);
            _isNew = false;
        }
        _originalHoliday = (Holiday?)_holiday?.Clone();
    }

    public string? ValidateStartDate(DateTime? newValue)
    {
        if (newValue == _originalHoliday?.ValidFrom) return null;
        if (newValue == null) return L["No value for start date"];
        if (newValue.Value.CompareTo(DateTime.UtcNow.Date) < 0) return L["Should not be in the past"];
        return null;
    }
    public string? ValidateTillDate(DateTime? newValue)
    {
        if (newValue == _originalHoliday?.ValidUntil) return null;
        if (newValue == null || _holiday is null) return L["No value for till date"];
        if (newValue.Value.CompareTo(DateTime.UtcNow.Date) < 0) return L["Should not be in the past"];
        if (newValue.Value.CompareTo(_holiday.ValidFrom) < 0) return L["Should not be before start date"];
        return null;
    }

    private async Task Cancel()
    {
        Navigation.NavigateTo("/user/vacations");
    }
    private async Task Submit()
    {
        try
        {
            // https://github.com/MudBlazor/MudBlazor/issues/4047
            if (_holiday?.Description is null || _holiday.ValidUntil is null || _holiday.ValidFrom is null) return;
            _holiday.Availability = Availability.NotAvailable;
            _holiday.ValidFrom = DateTime.SpecifyKind(_holiday.ValidFrom.Value, DateTimeKind.Utc);
            _holiday.ValidUntil = new DateTime(_holiday.ValidUntil.Value.Year, _holiday.ValidUntil.Value.Month, _holiday.ValidUntil.Value.Day, 23, 59, 59, DateTimeKind.Utc);
            if (_isNew == true)
            {
                var result = await _holidayRepository.PutHolidayForUser(_holiday);
                if (result is null || !result.Success) return;
                _holiday = result.Put;
                _isNew = false;
            }
            else
            {
                var result = await _holidayRepository.PatchHolidayForUser(_holiday);
                if (result is null || !result.Success) return;
                _holiday = result.Patched;
            }

            Navigation.NavigateTo("/user/vacations");
        }
        catch (Exception ex)
        {
            DebugHelper.WriteLine(ex);
            _error = L["Failed to save"];
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
