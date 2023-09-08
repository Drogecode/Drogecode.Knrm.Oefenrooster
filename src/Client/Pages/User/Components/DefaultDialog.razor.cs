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
    [Inject] private IStringLocalizer<DefaultDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public DefaultUserSchedule DefaultUserSchedule { get; set; } = default!;
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public bool? IsNew { get; set; }
    [AllowNull] private MudForm _form;
    private CancellationTokenSource _cls = new();
    private DefaultUserSchedule? _originalDefaultUserSchedule = null;
    private bool _success;
    private string[] _errors = Array.Empty<string>();
    void Cancel() => MudDialog.Cancel();
    protected override async Task OnParametersSetAsync()
    {
        if (IsNew == true)
        {
            DefaultUserSchedule = new DefaultUserSchedule();
        }
        _originalDefaultUserSchedule = (DefaultUserSchedule?)DefaultUserSchedule?.Clone();
    }
    private async Task Submit()
    {
        await _form.Validate();
        MudDialog.Close(DialogResult.Ok(true));
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
