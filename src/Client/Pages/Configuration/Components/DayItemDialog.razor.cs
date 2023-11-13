using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;

public sealed partial class DayItemDialog : IDisposable
{
    [Inject] private IStringLocalizer<DayItemDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ICalendarItemClient CalendarItemClient { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public RoosterItemDay? DayItem{ get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public bool? IsNew { get; set; }

    private CancellationTokenSource _cls = new();
    private RoosterItemDay? _originalDayItem { get; set; }
    void Cancel() => MudDialog.Cancel();
    protected override async Task OnParametersSetAsync()
    {
        if (IsNew == true)
        {
            DayItem = new RoosterItemDay();
        }
        _originalDayItem = (RoosterItemDay?)DayItem?.Clone();
    }

    public string? ValidateStartDate(DateTime? newValue)
    {
        if (newValue == _originalDayItem?.DateStart) return null;
        if (newValue == null) return L["No value for start date"];
        if (newValue.Value.CompareTo(DateTime.UtcNow.Date) < 0) return L["Should not be in the past"];
        return null;
    }
    public string? ValidateTillDate(DateTime? newValue)
    {
        if (newValue == _originalDayItem?.DateEnd) return null;
        if (newValue == null || DayItem is null) return L["No value for till date"];
        if (newValue.Value.CompareTo(DateTime.UtcNow.Date) < 0) return L["Should not be in the past"];
        if (newValue.Value.CompareTo(DayItem.DateEnd) < 0) return L["Should not be before start date"];
        return null;
    }

    private async Task Submit()
    {
        // https://github.com/MudBlazor/MudBlazor/issues/4047
        /*if (DayItem is null || DayItem.Text is null) return;
        DayItem.DateStart = DateTime.SpecifyKind(Holiday.ValidFrom.Value, DateTimeKind.Local).ToUniversalTime();
        Holiday.ValidUntil = new DateTime(Holiday.ValidUntil.Value.Year, Holiday.ValidUntil.Value.Month, Holiday.ValidUntil.Value.Day, 23, 59, 59, DateTimeKind.Local).ToUniversalTime();
        if (IsNew == true)
        {
            var result = await _holidayRepository.PutHolidayForUser(Holiday, _cls.Token);
            if (result is null || !result.Success) return;
            Holiday = result.Put;
        }
        else
        {
            var result = await _holidayRepository.PatchHolidayForUser(Holiday, _cls.Token);
            if (result is null || !result.Success) return;
            Holiday = result.Patched;
        }*/
        if (Refresh is not null) await Refresh.CallRequestRefreshAsync();
        MudDialog.Close(DialogResult.Ok(true));
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
