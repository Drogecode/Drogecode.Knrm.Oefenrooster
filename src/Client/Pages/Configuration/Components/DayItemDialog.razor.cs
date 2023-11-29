using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;

public sealed partial class DayItemDialog : IDisposable
{
    [Inject] private IStringLocalizer<DayItemDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ICalendarItemClient CalendarItemClient { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public RoosterItemDay? DayItem { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public List<DrogeUser>? Users { get; set; }
    [Parameter] public List<DrogeFunction>? Functions { get; set; }
    [Parameter] public bool? IsNew { get; set; }

    private IEnumerable<DrogeUser> _selectedUsersAction = new List<DrogeUser>();
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
        var user = Users.FirstOrDefault(x => x.Id == DayItem?.UserIds?.FirstOrDefault());
        if (user is not null)
            ((List<DrogeUser>)_selectedUsersAction).Add(user);
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
    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        _selectedUsersAction = selection;
    }

    private async Task Submit()
    {
        if (DayItem is null)
            return;
        if (DayItem.DateStart is not null)
            DayItem.DateStart = DateTime.SpecifyKind(DayItem.DateStart.Value, DateTimeKind.Utc);
        if (DayItem.DateEnd is not null)
            DayItem.DateEnd = new DateTime(DayItem.DateEnd.Value.Year, DayItem.DateEnd.Value.Month, DayItem.DateEnd.Value.Day, 23, 59, 59, DateTimeKind.Utc);

        DayItem.UserIds = [];
        Console.WriteLine($"Count is {_selectedUsersAction.Count()}");
        foreach (var user in _selectedUsersAction)
        {
            Console.WriteLine(user.Name);
            DayItem.UserIds.Add(user.Id);
        }

        if (IsNew is true)
        {
            var isPut = await CalendarItemClient.PutDayItemAsync(DayItem, _cls.Token);
            if (isPut?.Success is true)
            {
                DayItem!.Id = isPut.NewId;
                IsNew = false;
            }
        }
        else
        {
            var isPatched = await CalendarItemClient.PatchDayItemAsync(DayItem, _cls.Token);
        }
        if (Refresh is not null) await Refresh.CallRequestRefreshAsync();
        MudDialog.Close(DialogResult.Ok(true));
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
