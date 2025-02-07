using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User.Components;

public sealed partial class GroupDialog : IDisposable
{
    [Inject] private IStringLocalizer<GroupDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private DefaultScheduleRepository _defaultScheduleRepository { get; set; } = default!;

    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public DefaultGroup? DefaultGroup { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public bool? IsNew { get; set; }

    private CancellationTokenSource _cls = new();
    void Cancel() => MudDialog.Cancel();
    private DefaultGroup? _originalDefaultGroup;
    protected override async Task OnParametersSetAsync()
    {
        if (IsNew == true || DefaultGroup is null)
        {
            DefaultGroup = new DefaultGroup();
        }
        if (DefaultGroup.ValidFrom == DateTime.MaxValue)
            DefaultGroup.ValidFrom = null;
        if (DefaultGroup.ValidFrom == DateTime.MaxValue)
            DefaultGroup.ValidFrom = null;
        _originalDefaultGroup = (DefaultGroup?)DefaultGroup?.Clone();
    }
    private async Task Submit()
    {
        if (DefaultGroup is null)
            throw new ArgumentNullException("DefaultGroup");
        if (string.IsNullOrWhiteSpace( DefaultGroup.Name) || DefaultGroup.ValidFrom is null || DefaultGroup.ValidUntil is null) return; 
        var body = new DefaultGroup
        {
            Name = DefaultGroup.Name,
            ValidFrom = DefaultGroup.ValidFrom!.Value.ToUniversalTime(),
            ValidUntil = DefaultGroup.ValidUntil!.Value.ToUniversalTime(),
            IsDefault = false
        };
        var result = await _defaultScheduleRepository.PutGroup(body, _cls.Token);
        if (Refresh is not null)
            await Refresh.CallRequestRefreshAsync();
        MudDialog.Close(DialogResult.Ok(true));
    }

    public string? ValidateStartDate(DateTime? newValue)
    {
        if (IsNew != true && _originalDefaultGroup?.ValidFrom.Equals(DefaultGroup?.ValidFrom) == true) return null;
        if (newValue == null) return L["No value for start date"];
        if (newValue.Value.CompareTo(DateTime.UtcNow.Date) < 0) return L["Should not be in the past"];
        return null;
    }
    public string? ValidateTillDate(DateTime? newValue)
    {
        if (IsNew != true && _originalDefaultGroup?.ValidUntil.Equals(DefaultGroup?.ValidUntil) == true) return null;
        if (newValue == null || DefaultGroup is null) return L["No value for till date"];
        if (newValue.Value.CompareTo(DateTime.UtcNow.Date) < 0) return L["Should not be in the past"];
        if (newValue.Value.CompareTo(DefaultGroup.ValidFrom) < 0) return L["Should not be before start date"];
        return null;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
