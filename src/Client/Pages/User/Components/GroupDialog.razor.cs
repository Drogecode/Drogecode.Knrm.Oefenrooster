using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Microsoft.Extensions.Localization;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User.Components;

public sealed partial class GroupDialog : IDisposable
{
    [Inject] private IStringLocalizer<GroupDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private DefaultScheduleRepository _defaultScheduleRepository { get; set; } = default!;

    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public DefaultGroup? DefaultGroup { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public bool? IsNew { get; set; }

    [AllowNull] private MudForm _form;
    private CancellationTokenSource _cls = new();
    private bool _success;
    private string[] _errors = Array.Empty<string>();
    void Cancel() => MudDialog.Cancel();
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
    }
    private async Task Submit()
    {
        await _form.Validate();
        if (DefaultGroup is null)
            throw new ArgumentNullException("DefaultGroup");
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

    public void Dispose()
    {
        _cls.Cancel();
    }
}
