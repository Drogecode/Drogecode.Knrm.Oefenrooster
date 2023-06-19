using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Microsoft.Extensions.Localization;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User.Components;

public sealed partial class VacationDialog : IDisposable
{
    [Inject] private HolidayRepository _holidayRepository { get; set; } = default!;
    [Inject] private IStringLocalizer<VacationDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public Holiday? Holiday { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public bool? IsNew { get; set; }

    private CancellationTokenSource _cls = new();
    private bool _success;
    private string[] _errors = Array.Empty<string>();
    [AllowNull] private MudForm _form;
    void Cancel() => MudDialog.Cancel();
    protected override async Task OnParametersSetAsync()
    {
        if (IsNew == true)
        {
            Holiday = new Holiday();
        }
    }

    public string? ValidateStartDate(DateTime? newValue)
    {
        if (newValue == null) return L["No value for start date"];
        if (newValue.Value.CompareTo(DateTime.UtcNow) < 0) return L["Should not be in the past"];
        return null;
    }
    public string? ValidateTillDate(DateTime? newValue)
    {
        if (newValue == null || Holiday is null) return L["No value for till date"];
        if (newValue.Value.CompareTo(DateTime.UtcNow) < 0) return L["Should not be in the past"];
        if (newValue.Value.CompareTo(Holiday.ValidFrom) < 0) return L["Should not be before start date"];
        return null;
    }
    private async Task Submit()
    {
        await _form.Validate();
        if (!_form.IsValid || Holiday is null) return;
        Holiday.Available = Availabilty.NotAvailable;
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
        }
        if (Refresh is not null) await Refresh.CallRequestRefreshAsync();
        MudDialog.Close(DialogResult.Ok(true));
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
