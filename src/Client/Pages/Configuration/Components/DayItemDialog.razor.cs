using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;

public sealed partial class DayItemDialog : IDisposable
{
    [Inject] private IStringLocalizer<DayItemDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IDayItemClient DayItemClient { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public RoosterItemDay? DayItem { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public List<DrogeUser>? Users { get; set; }
    [Parameter] public List<DrogeFunction>? Functions { get; set; }
    [Parameter] public bool? IsNew { get; set; }

    private IEnumerable<DrogeUser> _selectedUsersAction = new List<DrogeUser>();
    private CancellationTokenSource _cls = new();
    private RoosterItemDay? _originalDayItem { get; set; }
    private DateRange _dateRange = new DateRange();
    void Cancel() => MudDialog.Cancel();
    protected override async Task OnParametersSetAsync()
    {
        if (IsNew == true || DayItem is null)
        {
            DayItem = new RoosterItemDay();
        }
        _dateRange.Start = DayItem.DateStart;
        _dateRange.End = DayItem.DateEnd;
        _originalDayItem = (RoosterItemDay?)DayItem?.Clone();
        var user = Users?.FirstOrDefault(x => x.Id == DayItem?.LinkedUsers?.FirstOrDefault()?.UserId);
        if (user is not null)
            ((List<DrogeUser>)_selectedUsersAction).Add(user);
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        _selectedUsersAction = selection;
    }

    private async Task Submit()
    {
        if (DayItem is null || _dateRange.Start is null || DayItem.Text is null)
            return;
        DayItem.DateStart = DateTime.SpecifyKind(_dateRange.Start.Value, DateTimeKind.Utc);
        if (_dateRange.End is not null)
            DayItem.DateEnd = DateTime.SpecifyKind(_dateRange.End.Value, DateTimeKind.Utc);

        DayItem.LinkedUsers = [];
        foreach (var user in _selectedUsersAction)
        {
            DayItem.LinkedUsers.Add(new RoosterItemDayLinkedUsers
            {
                UserId = user.Id
            });
        }

        if (IsNew is true)
        {
            var isPut = await DayItemClient.PutDayItemAsync(DayItem, _cls.Token);
            if (isPut?.Success is true)
            {
                DayItem!.Id = isPut.NewId;
                IsNew = false;
            }
        }
        else
        {
            var isPatched = await DayItemClient.PatchDayItemAsync(DayItem, _cls.Token);
        }
        if (Refresh is not null) await Refresh.CallRequestRefreshAsync();
        MudDialog.Close(DialogResult.Ok(true));
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
