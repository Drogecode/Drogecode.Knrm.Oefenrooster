using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User.Components;

public sealed partial class DefaultDialog : IDisposable
{
    //[Inject] private IStringLocalizer<DefaultDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private DefaultScheduleRepository _defaultScheduleRepository { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public DefaultUserSchedule? DefaultUserSchedule { get; set; } = default!;
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public bool? IsNew { get; set; }
    [Parameter] public Guid? DefaultId { get; set; }
    private CancellationTokenSource _cls = new();
   private void Cancel() => MudDialog.Cancel();
    protected override void OnParametersSet()
    {
        if (IsNew == true || DefaultUserSchedule is null)
        {
            DefaultUserSchedule = new DefaultUserSchedule();
        }
        if (DefaultUserSchedule.ValidFromUser == DateTime.MaxValue)
            DefaultUserSchedule.ValidFromUser = null;
        if (DefaultUserSchedule.ValidUntilUser == DateTime.MaxValue)
            DefaultUserSchedule.ValidUntilUser = null;
    }
    private async Task Submit()
    {
        if (DefaultUserSchedule?.ValidFromUser is null || DefaultUserSchedule.ValidUntilUser is null) return;
        if (DefaultId is null)
        {
            Console.WriteLine("DefaultId is null");
            return;
        }
        var body = new PatchDefaultUserSchedule
        {
            DefaultId = DefaultId.Value,
            UserDefaultAvailableId = DefaultUserSchedule.UserDefaultAvailableId,
            ValidFromUser = DefaultUserSchedule.ValidFromUser,
            ValidUntilUser = DefaultUserSchedule.ValidUntilUser,
            Assigned = DefaultUserSchedule.Assigned,
            Availability = DefaultUserSchedule.Availability,
        };
        await _defaultScheduleRepository.PatchDefaultScheduleForUser(body, _cls.Token);
        if (Refresh is not null)
            await Refresh.CallRequestRefreshAsync();
        MudDialog.Close(DialogResult.Ok(true));
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
